using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
public class WaterWaver : MonoBehaviour
{
    [SerializeField]
    private int resolution = 20;

    [SerializeField]
    private float surfaceHeight = 0.2f;

    [SerializeField]
    private float waveConstant = 1.0f;

    [SerializeField]
    private float wavePowerMultiplier = 0.2f;
    [SerializeField]
    private float interactMultiplier = 0.2f;
    [SerializeField]
    private float interactHorizontalMultiplier = 1.0f;
    [SerializeField]
    private LayerMask layersToInteract;
    [SerializeField]
    private int waveUpdateLoop;
    [SerializeField]
    private float decay = 0.998f;

    [SerializeField]
    private AudioClip inoutSE;

    private int bufferLength;
    private readonly int bufferCount = 4;
    private NativeArray<float>[] waveBuffer;
    private int currentBuffer;

    private NativeArray<Vector3> Vertices;
    private JobHandle deformWaveJobHandles;
    private JobHandle updateWaveJobHandles;

    private Mesh mesh;

    // 初期化処理
    private void Start()
    {
        // メッシュとバッファを初期化する
        Setup();
    }

    
    // 無効化処理
    private void OnDisable()
    {
        // タスクを終了させる
        deformWaveJobHandles.Complete();
        
    }

    // 破棄処理
    private void OnDestroy()
    {
        // 配列を開放する
        Vertices.Dispose();
        for (int i = 0; i < waveBuffer.Length; i++)
        {
            waveBuffer[i].Dispose();
        }
    }

    private void Setup()
    {
        // メッシュを初期化する
        // オブジェクトの横幅と係数からメッシュの分割数を計算する
        bufferLength = Mathf.RoundToInt(transform.lossyScale.x * resolution);
        // メッシュの生成
        mesh = BuildMesh(bufferLength - 1);
        // メッシュを割り当てる
        GetComponent<MeshFilter>().sharedMesh = mesh;

        // メッシュの頂点情報を保持する
        List<Vector3> vlist = new List<Vector3>(mesh.vertexCount);
        mesh.GetVertices(vlist);
        Vertices = new NativeArray<Vector3>(vlist.ToArray(), Allocator.Persistent);

        // バッファを初期化する
        waveBuffer = new NativeArray<float>[bufferCount];
        for (int i = 0; i < waveBuffer.Length; i++)
        {
            waveBuffer[i] = new NativeArray<float>(bufferLength, Allocator.Persistent);
        }
    }

    private Mesh BuildMesh(int divisions)
    {
        if (divisions < 1) divisions = 1;
        divisions++;
        Vector3[] vertices = new Vector3[divisions * 2];
        Vector2[] uvs = new Vector2[divisions * 2];
        int[] tris = new int[3 * 2 * (divisions - 1)];
        Vector3 origin = new Vector3(-0.5f, -0.5f, 0f);

        for (int i = 0; i < divisions; i++)
        {
            float r = (float)i / (divisions - 1);
            vertices[i * 2] = origin + new Vector3(r, 0f);
            vertices[i * 2 + 1] = origin + new Vector3(r, 1f+surfaceHeight/transform.lossyScale.y);
            uvs[i * 2] = new Vector2(r, 0f);
            uvs[i * 2 + 1] = new Vector2(r, 1f);
        }

        for (int i = 0; i < divisions - 1; i++)
        {
            // カメラの方向に注意

            //tris[i * 6 + 0] = i * 2;
            //tris[i * 6 + 1] = i * 2 + 3;
            //tris[i * 6 + 2] = i * 2 + 1;
            //tris[i * 6 + 3] = i * 2;
            //tris[i * 6 + 4] = i * 2 + 2;
            //tris[i * 6 + 5] = i * 2 + 3;

            tris[i * 6 + 0] = i * 2;
            tris[i * 6 + 1] = i * 2 + 1;
            tris[i * 6 + 2] = i * 2 + 3;
            tris[i * 6 + 3] = i * 2;
            tris[i * 6 + 4] = i * 2 + 3;
            tris[i * 6 + 5] = i * 2 + 2;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }

    private RaycastHit2D[] hits = new RaycastHit2D[20];
    //private Collider2D[] hits = new Collider2D[20];
    private void LateUpdate()
    {
        // メッシュの上辺で当たり判定を行う
        Vector2 surface = new Vector2(0f, surfaceHeight);
        //Vector2 c = (Vector2)transform.position + Vector2.up * (transform.lossyScale.y * 1.0f - 0.125f);
        Vector2 s = new Vector2(transform.lossyScale.x, 0.25f);
        //int hitCount = Physics2D.OverlapBoxNonAlloc(c, s, 0.0f, hits);
        int hitCount = Physics2D.BoxCastNonAlloc(transform.position-Vector3.up*0.125f, s, 0.0f, new Vector2(0, 1),
            hits, transform.lossyScale.y*0.5f, layersToInteract);
        //int hitCount = Physics2D.LinecastNonAlloc(
        //    (Vector2)transform.position + transform.lossyScale * new Vector2(-0.5f, 0.5f) + surface,
        //    (Vector2)transform.position + transform.lossyScale * new Vector2(0.5f, 0.5f) + surface,
        //    hits, layersToInteract);

        // 1回のUpdateで処理する波の処理を計算する回数（おそらくシミュレーション速度）
        for (int loop = 0; loop < waveUpdateLoop; loop++)
        {
            // 使用するバッファの位置をずらす
            currentBuffer++;
            currentBuffer %= bufferCount;

            // バッファを更新する
            UpdateBuffer();

            for (int i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
               // var otherRigidbody = hit.rigidbody;
               // if (otherRigidbody == null) continue;
                var otherVelocity = hit.transform.GetComponentInChildren<WaterColliderScript>().GetFasterVelocity();
                var otherTransform = hit.transform;
                var width = hit.collider.bounds.size.x * 0.5f;
                //float otherHeight = hit.collider.bounds.size.y;
                //float otherCenterHeight = hit.collider.bounds.center.y - (transform.position.y + 0.5f * transform.lossyScale.y + surface.y);
                //float upperLength = otherHeight * 0.5f + otherCenterHeight;
                //float lowerLength = otherHeight * 0.5f - otherCenterHeight;

                var center = hit.collider.bounds.min.x + width;

                float centerLocal = (center - transform.position.x) / transform.lossyScale.x * 2f;

                // バッファ上での当たり判定が有った位置
                int bufferCenter = Mathf.FloorToInt(bufferLength * (centerLocal * 0.5f + 0.5f));
                // バッファ上での衝突した当たり判定の大きさ
                int bufferWidth = Mathf.FloorToInt(width / transform.lossyScale.x * bufferLength);
                for (int b = bufferCenter - bufferWidth; b <= bufferCenter + bufferWidth; b++)
                {
                    if (b < 0 || b >= bufferLength) continue;
                    waveBuffer[currentBuffer][b] = otherVelocity.y * interactMultiplier / width;
                    if (b < bufferCenter)
                    {
                        waveBuffer[currentBuffer][b] += interactHorizontalMultiplier * (-otherVelocity.x) * 2.0f;
                    }
                    else if (b > bufferCenter)
                    {
                        waveBuffer[currentBuffer][b] += interactHorizontalMultiplier * (otherVelocity.x);
                    }
                    // スケールで揺れ幅を調整する
                    waveBuffer[currentBuffer][b] /= transform.lossyScale.y;
                    //waveBuffer[currentBuffer][b] = Mathf.Clamp(waveBuffer[currentBuffer][b], -lowerLength * 0.5f, upperLength * 0.5f);
                }
            }
        }

        // メッシュを更新する
        DeformMesh();

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        HitCheck(col.gameObject.layer);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        HitCheck(col.gameObject.layer);
    }
    // 出入りするときに音を鳴らす
    private void HitCheck(int layer)
    {
        int layerValue = LayerMask.GetMask(LayerMask.LayerToName(layer));
        if ((layerValue & layersToInteract.value) > 0)
        {
            SoundPlayer.Play(inoutSE);
        }
    }

    // バッファを更新する
    private void UpdateBuffer()
    {
        // buffer to write
        int b0 = currentBuffer;
        // buffers to read
        int b1 = (currentBuffer - 1 + bufferCount) % bufferCount;
        int b2 = (currentBuffer - 2 + bufferCount) % bufferCount;


        updateWaveJobHandles = new UpdateWaveJob
        {
            currentBuffer = waveBuffer[b0],
            prevBuffer = waveBuffer[b1],
            prevPrevBuffer = waveBuffer[b2],
            waveConstant = waveConstant,
            decay = decay

        }.Schedule(waveBuffer[b0].Length, 0);
        JobHandle.ScheduleBatchedJobs();

        updateWaveJobHandles.Complete();
    }

    // メッシュを更新する
    private void DeformMesh()
    {
        //deform
        deformWaveJobHandles.Complete();

        // メッシュの更新を反映
#if UNITY_2019_3_OR_NEWER
            mesh.SetVertices(Vertices);
#else
        mesh.vertices = Vertices.ToArray();
#endif

        // メッシュを更新
        deformWaveJobHandles = new DeformWaveJob
        {
            vertices = Vertices,
            buffer = waveBuffer[currentBuffer],
            height = 0.5f+surfaceHeight/transform.lossyScale.y,
            multiplier = wavePowerMultiplier
        }.Schedule(Vertices.Length, 0);
        JobHandle.ScheduleBatchedJobs();
    }

    //[BurstCompile]
    public struct DeformWaveJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        [ReadOnly]
        public NativeArray<float> buffer;
        public float height;
        public float multiplier;

        public void Execute(int index)
        {
            if (index % 2 == 0) return;
            var v = vertices[index];
            v.y = height + buffer[index / 2] * multiplier;
            vertices[index] = v;
        }
    }

    //[BurstCompile]
    public struct UpdateWaveJob : IJobParallelFor
    {
        public NativeArray<float> currentBuffer;
        [ReadOnly]
        public NativeArray<float> prevBuffer;
        [ReadOnly]
        public NativeArray<float> prevPrevBuffer;

        public float waveConstant;
        public float decay;

        public void Execute(int index)
        {
            int bufferLength = currentBuffer.Length;
            float w0b1 = prevBuffer[index];
            float wlb1 = (index - 1 >= 0 && index - 1 < bufferLength ? prevBuffer[index - 1] : 0);
            float wrb1 = (index + 1 >= 0 && index + 1 < bufferLength ? prevBuffer[index + 1] : 0);
            float w0b2 = prevPrevBuffer[index];
            currentBuffer[index] = 2 * w0b1 - w0b2 + waveConstant * (wrb1 + wlb1 - 2 * w0b1);
            currentBuffer[index] *= decay;
        }
    }
}