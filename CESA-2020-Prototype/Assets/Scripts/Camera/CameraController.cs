using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // シリアライズ変数
    // カメラ
    [SerializeField]
    Camera mainCamera;
    // プレイヤー
    [SerializeField]
    GameObject player;
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

    Vector3 nextPos;

    // デバックカメラ揺れ
    CameraShake cameraShake;
    bool followOn = true;

    // Start is called before the first frame update
    void Start()
    {
        cameraShake = mainCamera.transform.GetComponent<CameraShake>();
        mainCamera.orthographicSize = cameraViewRange;
        mainCamera.transform.position = initializePos;
        nextPos = initializePos;
    }

    private void FixedUpdate()
    {
        var fourCorners = new Rect(GetScreenTopLeft().x, GetScreenBottomRight().y , GetScreenBottomRight().x, GetScreenTopLeft().y);

        if (fourCorners.x >= player.transform.position.x)
        {
            followOn = false;
            nextPos.x -= cellX;
        }
        if (fourCorners.height <= player.transform.position.y)
        {
            followOn = false;
            nextPos.y += cellY;
        }
        if (fourCorners.width <= player.transform.position.x)
        {
            followOn = false;
            nextPos.x += cellX;
        }
        if (fourCorners.y >= player.transform.position.y)
        {
            followOn = false;
            nextPos.y -= cellY;
        }


        if (!followOn)
        {
            followOn = FollowCamera(nextPos);
        }

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
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, playerPos, Common.Camera.SPEED_PERCENTAGE);

        // 移動したらTrueを返す
        if (CheckMove(mainCamera.transform.position, playerPos))
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
        if (Mathf.Approximately(start.y, end.y) && Mathf.Approximately(start.x, end.x))
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
}
