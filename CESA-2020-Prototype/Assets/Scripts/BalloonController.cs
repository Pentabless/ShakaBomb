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

    // バルーンジェネレータ
    BalloonGenerator balloonG;

    private int m_balloonCount = -1;

    // 消えるかどうか
    bool isDestroy;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find("Player");
        balloonG = GameObject.Find("BalloonGenerator").GetComponent<BalloonGenerator>();
        line = GetComponent<LineRenderer>();

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = 2;
        //line.SetColors(Color.white, Color.white);

        if (balloonG.GetMaxBalloons() == 1)
        {
            m_balloonCount = 1;
        }
        if (balloonG.GetMaxBalloons() == 2)
        {
            m_balloonCount = 2;
        }
        if (balloonG.GetMaxBalloons() == 3)
        {
            m_balloonCount = 3;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DamageTile")
        {
            balloonG.BrokenBalloon(m_balloonCount);

            //if (balloonG.GetMaxBalloons() == 1&& m_balloonCount<=2)
            //{
            //    m_balloonCount = 1;
            //}
            //if (balloonG.GetMaxBalloons() == 2 && m_balloonCount <= 3)
            //{
            //    m_balloonCount = 2;
            //}
            //if (balloonG.GetMaxBalloons() == 3)
            //{
            //    m_balloonCount = 3;
            //}
        }
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }
}
