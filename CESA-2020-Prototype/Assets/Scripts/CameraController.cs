using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    [SerializeField]
    GameObject player;
    [SerializeField]
    float yRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーのポジションとカメラのポジションを調べる
        Vector3 playerPos = player.transform.position;
        // ジャンプでは動かさない
        if (yRange <= Mathf.Abs(camera.transform.position.y) || yRange >= camera.transform.position.y)
        {
            playerPos.y = camera.transform.position.y;
        }

        // プレイヤーに追従
        playerPos.z = camera.transform.position.z;
        camera.transform.position = Vector3.Lerp(camera.transform.position, playerPos, 0.05f);

    }
}
