using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BubbleGenerator : MonoBehaviour
{
    [SerializeField]
    // プレハブ
    private GameObject bubblePrefab = null;
    [SerializeField]
    // 足元にできるエフェクト
    private ParticleSystem sweepPartcle = null;
    [SerializeField]
    // プレイヤー
    private Transform playerTransform = null;

    // 限度の大きさ
    private Vector3 limit_scale = Vector3.one;
    // 色
    private Color color = Color.white;
    // ばらける半径
    private float spreadRadius = 1;


    [SerializeField]
    // SE
    private AudioClip m_sound;

    void Start()
    {
        playerTransform = GameObject.Find(Player.NAME).transform;

        limit_scale = new Vector3(5.0f, 5.0f, 5.0f);
        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

    }

    void Update()
    {

    }

    public void BubbleCreate(Vector3 pos, int num, bool emitSweepParticle)
    {
        if (num <= 0)
        {
            Debug.Log("0個以下のBubbleを生成しようとしています");
            return;
        }

        for (int i = 0; i < num; i++)
        {
            //プレハブと同じオブジェクトを作る
            GameObject go = Instantiate(bubblePrefab) as GameObject;
            //座標を設定する
            float angle = Random.Range(0.0f, Mathf.PI * 2);
            var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Random.Range(0.0f, spreadRadius);
            go.transform.position = pos + offset;
            //生成した泡を子オブジェクトに登録する
            go.transform.parent = this.transform;
            //色を設定する
            go.GetComponent<Renderer>().material.color = color;

            // 足元にエフェクトを発生させる
            if (emitSweepParticle)
            {
                sweepPartcle.transform.position = playerTransform.position;
                sweepPartcle.Emit(1);
            }
        }

        // 個数が増えると音量も上がる
        SoundPlayer.Play(m_sound, 1 + (num - 1) * 0.2f);
    }

    public void SetBubbleCreateInfo(Vector4 request_color, Vector3 request_limit_scale)
    {
        limit_scale = request_limit_scale;  //大きくなる限度
        color = request_color;              //色
    }
    

    //------------------------------------------------------------------------------------------
    // ターゲットの追跡をやめる
    //------------------------------------------------------------------------------------------
    public void StopChase()
    {
        var children = GetComponentsInChildren<BubbleController>();
        foreach(var child in children)
        {
            child.StopChase();
        }
    }
}
