using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 定数
    // ラープの割合
    readonly float percentage = 0.05f;

    // シリアライズ変数
    // カメラ
    [SerializeField]
    Camera mainCamera;
    // デバックカメラ揺れ
    CameraShake cameraShake;
    // プレイヤー
    [SerializeField]
    GameObject player;
    // カメラの視野角
    [SerializeField]
    float cameraViewRange;
    // カメラの挙動範囲
    [SerializeField]
    Rect cameraRange;
    // カメラの移動：要素数　Up(0):Down(1)
    [SerializeField]
    float[] moveCameraAmount;

    // プレイヤーの追従On
    bool followCameraFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraShake = mainCamera.transform.GetComponent<CameraShake>();
        mainCamera.orthographicSize = cameraViewRange;
    }

    private void FixedUpdate()
    {
        // プレイヤーのポジションとカメラのポジションを調べる
        Vector3 playerPos = player.transform.position;

        // ジャンプだけではカメラを追従しない
        if (playerPos.y >= GetScreenTopLeft().y || playerPos.y <= GetScreenCenter().y)
        {
            followCameraFlag = false;
        }

        if (!followCameraFlag)
        {
            followCameraFlag = FollowCameraY(playerPos.y);
        }
        FollowCameraX(playerPos.x);

        // カメラの範囲指定を適用
        mainCamera.transform.position = SetCameraRangePosition(mainCamera.transform.position.x, mainCamera.transform.position.y);

        /// ここからデバック処理
        if (Input.GetKeyDown(KeyCode.A))
            cameraShake.Shake(0.5f, 0.3f);

        if (Input.GetKeyDown(KeyCode.W))
        {
            cameraViewRange++;
            mainCamera.orthographicSize = cameraViewRange;
            Debug.Log("描画範囲(+)：" + cameraViewRange);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cameraViewRange--;
            mainCamera.orthographicSize = cameraViewRange;
            Debug.Log("描画範囲(-)：" + cameraViewRange);
        }

        if (Input.GetKey(KeyCode.E))
        {
            MoveCamera();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveCamera(false);
        }
        if (Input.GetKeyDown(KeyCode.O))
            OriginCamera();
    }

    /// <summary>
    /// カメラを上下に移動
    /// </summary>
    /// <param name="up">↑移動or↓移動</param>
    /// <returns>Trueならば完了</returns>
    public bool MoveCamera(bool up = true)
    {
        Vector3 cameraPos = mainCamera.transform.position;
        followCameraFlag = true;

        cameraPos.y = moveCameraAmount[up ? 0 : 1];
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraPos, 0.05f);

        // 移動したらTrueを返す
        if (Mathf.Approximately(mainCamera.transform.position.y, cameraPos.y))
            return true;

        return false;
    }

    /// <summary>
    /// カメラを原点に戻す
    /// </summary>
    public void OriginCamera()
    {
        followCameraFlag = false;
    }

    /// <summary>
    /// カメラ追従
    /// Updateで呼ぶ
    /// </summary>
    /// <param name="playerPos">プレイヤーポジション</param>
    /// <returns>完了していればtrue</returns>
    private bool FollowCamera(Vector3 playerPos)
    {
        playerPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, playerPos, percentage);

        // 移動したらTrueを返す
        if (CheckMove(mainCamera.transform.position, playerPos))
            return true;

        return false;
    }

    /// <summary>
    /// 横方向にプレイヤーを追う
    /// </summary>
    /// <param name="x">目的位置</param>
    /// <returns>完了しているかどうか</returns>
    private bool FollowCameraX(float x)
    {
        Vector3 cameraPos = mainCamera.transform.position;

        mainCamera.transform.position = new Vector3(Mathf.Lerp(cameraPos.x, x, 0.1f), cameraPos.y, cameraPos.z);
        // 移動したらTrueを返す
        if (CheckDifferences(mainCamera.transform.position.x, x, 0.01f))
            return true;

        return false;
    }

    /// <summary>
    /// 縦方向にカメラを追う
    /// </summary>
    /// <param name="y">目的位置</param>
    /// <returns>完了したかどうか</returns>
    private bool FollowCameraY(float y)
    {
        Vector3 cameraPos = mainCamera.transform.position;

        mainCamera.transform.position = new Vector3(cameraPos.x, Mathf.Lerp(cameraPos.y, y, 0.05f), cameraPos.z);

        // 移動したらTrueを返す
        if (CheckDifferences(mainCamera.transform.position.y, cameraPos.y, 0.01f))
            return true;

        return false;
    }



    /// <summary>
    /// ラープ完了したかどうか
    /// </summary>
    /// <param name="start">スタートポジション</param>
    /// <param name="end">エンドポジション</param>
    /// <returns>完了していればtrue</returns>
    private bool CheckMove(Vector3 start, Vector3 end)
    {
        if (CheckDifferences(start.x, end.x, 0.1f) && CheckDifferences(start.y, end.y, 0.1f))
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
    private bool CheckDifferences(float x, float y, float differences)
    {
        float temp = Mathf.Abs(x) - Mathf.Abs(y);

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

    /// <summary>
    /// 画面の中央を取る
    /// </summary>
    /// <returns></returns>
    private Vector3 GetScreenCenter()
    {
        Vector3 center = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        return center;
    }
}
