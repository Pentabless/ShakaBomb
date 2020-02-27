using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    CameraShake cameraShake;
    [SerializeField]
    GameObject player;
    [SerializeField]
    float yRange;
    // Start is called before the first frame update
    void Start()
    {
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
        // ジャンプでは動かさない
        if (Mathf.Abs(playerPos.y) <= yRange)
        {
            playerPos.y = camera.transform.position.y;
        }

        // プレイヤーに追従
        playerPos.z = camera.transform.position.z;
        camera.transform.position = Vector3.Lerp(camera.transform.position, playerPos, 0.05f);

        // デバック処理
        if (Input.GetKeyDown(KeyCode.A))
            cameraShake.Shake(0.5f, 0.3f);
    }
}
