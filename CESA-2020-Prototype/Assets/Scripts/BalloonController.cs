using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

public class BalloonController : MonoBehaviour
{
    // リジッドボディ
    Rigidbody2D rig;

    // ラインレンダラー 
    LineRenderer line;

    // プレイヤーの情報
    GameObject playerObj;

    // バルーンジェネレータ
    BalloonGenerator balloonG;

    // 消えるかどうか
    private bool m_isDestroy = false;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find(Player.NAME);
        balloonG = GameObject.Find(Balloon.NAME).GetComponent<BalloonGenerator>();
        line = GetComponent<LineRenderer>();

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = 2;
        //line.SetColors(Color.white, Color.white);
    }

    void Update()
    {
        Vector3 playerPos = playerObj.transform.position;
        playerPos.x += Balloon.DISTANCE_X;
        playerPos.y += Balloon.DISTANCE_Y;

        Vector3 move_force = playerPos - this.transform.position;
        rig.velocity = move_force * 2.0f;

        Vector3 thisPos = this.transform.position;

        line.SetPosition(0, thisPos);
        line.SetPosition(1, playerObj.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Stage.DAMAGE_TILE)
        {
            Destroy();
            balloonG.BrokenBalloon();
        }
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
