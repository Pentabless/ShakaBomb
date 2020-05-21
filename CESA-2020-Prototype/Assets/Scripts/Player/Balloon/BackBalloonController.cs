//==============================================================================================
/// File Name	: BackBalloonController.cs
/// Summary		: バックバルーン
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
[RequireComponent(typeof(CircleCollider2D))]
public class BackBalloonController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // コライダー
    private CircleCollider2D thisCollider = null;
    [SerializeField]
    // 描画用子オブジェクト
    private GameObject renderObject = null;
    // レンダラー
    private new SpriteRenderer renderer = null;
    // プレイヤー
    private PlayerController playerController = null;
    

    [SerializeField]
    // オフセット位置
    private Vector2 offsetPos = Vector2.zero;
    [SerializeField]
    // オフセット方向
    private Vector2 offsetDirection = -Vector2.one;

    // バルーンの所持状態
    private bool hasBalloon = false;
    [SerializeField]
    // バルーンの限界サイズ
    private float limitSize = 10.0f;
    // バルーンの現在サイズ
    private float balloonSize = 0.0f;
    [SerializeField]
    // バルーンの移動速度
    private float balloonLerpRate = 0.1f;

    [SerializeField]
    // 移動判定用レイヤーマスク
    private LayerMask moveLayerMask = 0;
    // 移動判定用レイキャスト情報
    private RaycastHit2D[] hits = new RaycastHit2D[20];

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        thisCollider.isTrigger = true;
        renderer = renderObject.GetComponent<SpriteRenderer>();
        renderer.enabled = false;
        offsetDirection.Normalize();
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        playerController = GameObject.Find(Player.NAME).GetComponent<PlayerController>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        // 移動先を計算する
        var offset = offsetPos + offsetDirection * thisCollider.radius;
        offset.x *= Data.playerDir;
        var targetPos = playerController.gameObject.transform.position + (Vector3)offset;

        // バルーンを持ってない場合は位置を固定する
        if (!hasBalloon)
        {
            transform.position = targetPos;
            return;
        }

        // 移動判定を行う
        var targetVec = (targetPos - transform.position);
        float distance = targetVec.magnitude * balloonLerpRate;
        int hitCount = Physics2D.CircleCastNonAlloc(transform.position, thisCollider.radius + 0.1f, targetVec.normalized,
            hits, distance, moveLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            distance = Mathf.Min(hits[i].distance, distance);
        }

        transform.position += targetVec.normalized * distance;

        //// 衝突するオブジェクトがないなら移動させる
        //if (hitCount == 0)
        //{
        //    transform.position += targetVec.normalized * distance;
        //}
        //// 衝突していたなら、そこまで移動させる
        //else
        //{
            
        //}
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerEnter2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Bubble")
        {
            return;
        }

        if (!hasBalloon)
        {
            InitBalloon();
        }

        float size = collision.transform.localScale.x;
        balloonSize += size*size;
        size = Mathf.Min(Mathf.Sqrt(balloonSize), limitSize);
        thisCollider.radius = size/2;

        renderObject.transform.localScale = Vector2.one * size;

        collision.gameObject.GetComponent<BubbleController>().Destroy();
    }

    //------------------------------------------------------------------------------------------
    // バルーンの初期化
    //------------------------------------------------------------------------------------------
    private void InitBalloon()
    {
        renderer.enabled = true;
        thisCollider.isTrigger = false;
        hasBalloon = true;
    }
}
