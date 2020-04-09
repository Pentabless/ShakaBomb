//==============================================================================================
/// File Name	: BalloonController.cs
/// Summary		: バルーン制御
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
//==============================================================================================
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
//==============================================================================================
public class BalloonController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // リジッドボディ
    private Rigidbody2D m_rigid2D = null;
    // ラインレンダラー 
    private LineRenderer m_line = null;
    // プレイヤーの情報
    private GameObject m_player = null;
    // バルーンジェネレータ
    private BalloonGenerator m_balloonG = null;

    // 消えるかどうか
    private bool m_isDestroy = false;

    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        Init();
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        Vector3 playerPos = m_player.transform.position;
        if (Data.playerDir > 0)
        {
            playerPos.x -= Balloon.DISTANCE_X;
        }
        else
        {
            playerPos.x += Balloon.DISTANCE_X;
        }

        playerPos.y += Balloon.DISTANCE_Y;

        Vector3 move_force = playerPos - this.transform.position;

        if(Vector3.Distance(playerPos,this.transform.position) >= 4)
        {
            m_rigid2D.velocity = move_force * 4.0f;
        }
        m_rigid2D.velocity = move_force * 2.0f;

        Vector3 thisPos = this.transform.position;

        m_line.SetPosition(0, thisPos);
        m_line.SetPosition(1, m_player.transform.position);
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerEnter2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Stage.DAMAGE_TILE)
        {
            Destroy();
            m_balloonG.BrokenBalloon();
        }
    }

    //------------------------------------------------------------------------------------------
    // 初期化
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        m_rigid2D = GetComponent<Rigidbody2D>();
        m_player = GameObject.Find(Player.NAME);
        m_balloonG = GameObject.Find(Balloon.NAME).GetComponent<BalloonGenerator>();
        m_line = GetComponent<LineRenderer>();

        m_line.startWidth = 0.05f;
        m_line.endWidth = 0.05f;
        m_line.positionCount = 2;
        //line.SetColors(Color.white, Color.white);
    }

    //------------------------------------------------------------------------------------------
    // Destroy
    //------------------------------------------------------------------------------------------
    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
