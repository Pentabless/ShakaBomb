using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラ
    [SerializeField]
    Camera camera;
    // デバックカメラ揺れ
    CameraShake cameraShake;
    // プレイヤー
    [SerializeField]
    GameObject player;
    // カメラを動かすrange
    [SerializeField]
    float yRange;
    // カメラの挙動範囲
    [SerializeField]
    Vector2 behaviorRange;
    // カメラが縦に動いているかどうか
    bool moveFlag;
    // Start is called before the first frame update
    void Start()
    {
        moveFlag = false;
        cameraShake = camera.transform.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        //// プレイヤーのポジションとカメラのポジションを調べる
        //Vector3 playerPos = player.transform.position;
        //// ジャンプでは動かさない
        //if (yRange <= Mathf.Abs(camera.transform.position.y) || yRange >= camera.transform.position.y)
        //{
        //    playerPos.y = camera.transform.position.y;
        //}

        //// プレイヤーに追従
        //playerPos.z = camera.transform.position.z;
        //camera.transform.position = Vector3.Lerp(camera.transform.position, playerPos, 0.05f);

        //// デバック処理
        //if (Input.GetKeyDown(KeyCode.A))
        //    cameraShake.Shake(0.5f, 0.3f);
    }

    private void FixedUpdate()
    {
        // プレイヤーのポジションとカメラのポジションを調べる
        Vector3 playerPos = player.transform.position;
        float length = playerPos.y - camera.transform.position.y;

        // ジャンプでは動かさない
        if (Mathf.Abs(length) <= yRange && !moveFlag)
        {
            playerPos.y = camera.transform.position.y;
        }
        else
        {
            moveFlag = true;
        }

        // プレイヤーに追従
        playerPos.z = camera.transform.position.z;
        camera.transform.position = Vector3.Lerp(camera.transform.position, playerPos, 0.05f);

        // 上がりきるまで位置の修正をさせない
        if(Mathf.Approximately(playerPos.y,camera.transform.position.y))
        {
            moveFlag = false;
        }

        // 画面の範囲指定
        if (behaviorRange.y >= camera.transform.position.y)
            camera.transform.position = new Vector3(camera.transform.position.x, behaviorRange.y, -10);

        // デバック処理
        if (Input.GetKeyDown(KeyCode.A))
            cameraShake.Shake(0.5f, 0.3f);
    }
}
