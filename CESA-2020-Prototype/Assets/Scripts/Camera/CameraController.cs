using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // 背景情報
    [System.Serializable]
    public struct BGInfo
    {
        public GameObject obj;
        public Vector2 size;
    }

    // カメラ
    [SerializeField]
    Camera mainCamera;
    // プレイヤー
    [SerializeField]
    GameObject player;
    PlayerController pController;
    // カメラの視野角
    [SerializeField]
    float cameraViewRange;
    // カメラの挙動範囲
    [SerializeField]
    Rect cameraRange;
    [SerializeField]
    Vector3 initializePos;

    [SerializeField]
    float cellX;
    [SerializeField]
    float cellY;

    // デバックカメラ揺れ
    CameraShake cameraShake;
    // 次のブロックへのポジション
    Vector3 nextPos;
    // 現在のポジション
    Vector3 currentPos;
    // 追従フラグ
    bool followOn = true;
    // リスポーン位置記憶フラグ
    bool rememberPos = false;
    // リスポーンを覚えるまでのカウント
    float respawnCount;
    // プレイヤーロックまでのカウント
    float playerLockCount;
    // 移動距離
    float distance;
    // ラープの開始時間
    float startTime;

    // 視差背景
    [Header("size=pixel/PixelPerUnit*scale")]
    [SerializeField]
    List<BGInfo> backGrounds = new List<BGInfo>();
    // 背景の開始Y座標
    [SerializeField]
    float bgBottom = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraShake = mainCamera.transform.GetComponent<CameraShake>();
        mainCamera.orthographicSize = cameraViewRange;

        if(initializePos != Vector3.zero)
        {
            initializePos.z = Common.Camera.POSITION_Z;
            mainCamera.transform.position = initializePos;
            nextPos = initializePos;
        }
        else
        {
            nextPos = mainCamera.transform.position;
        }

        // プレイヤーのポジションを保存しておく
        Data.initialPlayerPos = player.transform.position;

        pController = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        var fourCorners = new Rect(GetScreenTopLeft().x, GetScreenBottomRight().y, GetScreenBottomRight().x, GetScreenTopLeft().y);

        Debug.Log(nextPos.y);
        if (!followOn)
        {
            // カメラ移動中
            followOn = FollowCamera(currentPos, nextPos);
            playerLockCount += Time.deltaTime;
            if (playerLockCount >= Common.Camera.CANNOT_FRAME)
            {
                pController.EnableControl(false);
                playerLockCount = 0.0f;
            }

            if (followOn)
            {
                pController.EnableControl(true);

                //if (nextPos.y >= 40.0f)
                //{
                //    cellY = Common.Camera.SECOND_CELL_Y;
                //}
                //else
                //{
                //    cellY = Common.Camera.FIRST_CELL_Y;
                //}
            }
        }
        else
        {
            // プレイヤーの向きによって画面を切り替えるかどうかを判断
            if (Data.playerDir < 0)
            {
                if (fourCorners.x > player.transform.position.x)
                {
                    followOn = false;
                    rememberPos = true;
                    nextPos.x -= cellX;
                }
            }
            else
            {
                if (fourCorners.width < player.transform.position.x)
                {
                    followOn = false;
                    rememberPos = true;
                    nextPos.x += cellX;
                }
            }

            if (fourCorners.height <= player.transform.position.y)
            {
                followOn = false;
                rememberPos = true;

                if (nextPos.y >= Common.Camera.FIRST_CELL_Y)
                    cellY = Common.Camera.SECOND_CELL_Y;
                nextPos.y += cellY;
            }
            if (fourCorners.y >= player.transform.position.y)
            {
                followOn = false;
                rememberPos = true;

                if (nextPos.y <= Common.Camera.FIRST_CELL_Y)
                    cellY = Common.Camera.FIRST_CELL_Y;
                nextPos.y -= cellY;
            }
            // カメラが移動していないときの設定
            startTime = Time.time;
            currentPos = mainCamera.transform.position;
            distance = Vector3.Distance(nextPos, currentPos);
        }

        // リスポーンポジションの記憶とカウント
        if (rememberPos)
        {
            respawnCount += Time.deltaTime;
            //　数フレームは記憶する
            if (respawnCount >= Common.Camera.REMEMBER_FRAME)
            {
                Data.initialPlayerPos = player.transform.position;
                rememberPos = false;
            }
        }
        else
        {
            respawnCount = 0.0f;
        }

        // カメラの範囲指定を適用
        mainCamera.transform.position = SetCameraRangePosition(mainCamera.transform.position.x, mainCamera.transform.position.y);

        // 背景の移動
        MoveBackGrounds();
    }


    /// <summary>
    /// カメラ追従
    /// Updateで呼ぶ
    /// </summary>
    /// <param name="playerPos">プレイヤーポジション</param>
    /// <returns>完了していればtrue</returns>
    private bool FollowCamera(Vector3 start, Vector3 end)
    {
        var percentage = ((Time.time - startTime) * Common.Camera.SPEED) / distance;

        mainCamera.transform.position = Vector3.Lerp(start, end, percentage);

        // 移動したらTrueを返す
        if (CheckMove(mainCamera.transform.position, end))
            return true;

        return false;
    }


    /// <summary>
    /// 背景の移動
    /// </summary>
    private void MoveBackGrounds()
    {
        foreach (var bg in backGrounds)
        {
            // デフォルトのオフセット位置を設定
            Vector3 offset = new Vector3(0, 0, bg.obj.transform.localPosition.z);
            if (cameraRange.width - cameraRange.x > Mathf.Epsilon)
            {
                float t = (mainCamera.transform.position.x - cameraRange.x) / (cameraRange.width - cameraRange.x);
                float width =
                    Mathf.Max(bg.size.x * bg.obj.transform.lossyScale.x - mainCamera.orthographicSize * mainCamera.aspect * 2.0f, 0.0f);
                offset.x -= Mathf.Lerp(-width * 0.5f, width * 0.5f, t);
            }
            if (cameraRange.height - cameraRange.y > Mathf.Epsilon)
            {
                if (mainCamera.transform.position.y - bgBottom < mainCamera.orthographicSize)
                {
                    float lim = (bgBottom + bg.size.y * bg.obj.transform.lossyScale.y * 0.5f) - mainCamera.transform.position.y;
                    offset.y = lim;
                }
                else
                {
                    float rangeY = bgBottom + mainCamera.orthographicSize;
                    float t = (mainCamera.transform.position.y - rangeY) / (cameraRange.height - rangeY);
                    float height = Mathf.Max(bg.size.y * bg.obj.transform.lossyScale.y - mainCamera.orthographicSize * 2.0f, 0.0f);
                    offset.y -= Mathf.Lerp(-height * 0.5f, height * 0.5f, t);

                }
            }
            bg.obj.transform.localPosition = offset;
        }
    }

    /// <summary>
    /// ラープ完了したかどうか
    /// </summary>
    /// <param name="start">スタートポジション</param>
    /// <param name="end">エンドポジション</param>
    /// <returns>完了していればtrue</returns>
    private bool CheckMove(Vector3 start, Vector3 end)
    {
        //if (Mathf.Approximately(start.y, end.y) && Mathf.Approximately(start.x, end.x))
        //    return true;
        if (CheckDifferences(start, end, 0.1f))
            return true;

        return false;
    }

    /// <summary>
    /// Floatの値を比べる
    /// 既存のものではTrueになるのが遅いため
    /// </summary>
    /// <param name="x">比べる値</param>
    /// <param name="y">比べる値</param>
    /// <param name="differences">許容値</param>
    /// <returns>近い値になったかどうか</returns>
    private bool CheckDifferences(Vector3 start, Vector3 end, float differences)
    {
        float temp = Vector3.Distance(start, end);

        if (temp <= differences)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 画面の範囲指定をする
    /// </summary>
    /// <param name="x">カメラのXポジション</param>
    /// <param name="y">カメラのYポジション</param>
    /// <returns></returns>
    private Vector3 SetCameraRangePosition(float x, float y)
    {
        Vector3 temp;
        temp = new Vector3(Mathf.Clamp(x, cameraRange.x, cameraRange.width)
                         , Mathf.Clamp(y, cameraRange.y, cameraRange.height)
                         , mainCamera.transform.position.z);
        return temp;
    }

    /// <summary>
    /// 画面の左上座標を取得
    /// </summary>
    /// <returns>画面の左上座標</returns>
    private Vector3 GetScreenTopLeft()
    {
        // 画面の左上を取得
        Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f));
        //topLeft.Scale(new Vector3(1f, -1f, 1f));
        return topLeft;
    }

    /// <summary>
    /// 画面の右下座標を取得
    /// </summary>
    /// <returns>画面の右下座標</returns>
    private Vector3 GetScreenBottomRight()
    {
        // 画面の右下を取得
        Vector3 bottomRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f));
        // 上下反転させる
        //bottomRight.Scale(new Vector3(1f, -1f, 1f));
        return bottomRight;
    }
}