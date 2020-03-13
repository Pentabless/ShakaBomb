using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    // リジッドボディ
    Rigidbody2D rig;

    // ラインレンダラー 
    LineRenderer line;

    // プレイヤーの情報
    GameObject playerObj;
    // 消えるかどうか
    bool isDestroy;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find("Player");
        line = GetComponent<LineRenderer>();

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = 2;
        //line.SetColors(Color.white, Color.white);
    }

    void Update()
    {
        Vector3 playerPos = playerObj.transform.position;
        playerPos.x += 1.2f;
        playerPos.y += 1.7f;

        Vector3 move_force = playerPos - this.transform.position;
        rig.velocity = move_force * 2.0f;

        Vector3 thisPos = this.transform.position;

        line.SetPosition(0, thisPos);
        line.SetPosition(1, playerObj.transform.position);
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }
}
