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

    [SerializeField]
    // 使用不可時のカラー
    private Color uselessColor = Color.white;
    [SerializeField]
    // 破裂寸前のカラー
    private Color cautionColor = Color.white;
    [SerializeField]
    // 破裂寸前判定の割合
    private float cautionRate = 0.8f;
    [SerializeField]
    // 破裂寸前の点滅速度
    private float cautionSpeed = 2.0f;
    // 破裂タイマー
    private float cautionTimer = 0;

    [SerializeField]
    // 破裂時にで吹き飛ぶ方向
    private Vector2 burstDirection = new Vector2(1, 0.0f);
    [SerializeField]
    // 破裂時のパワー
    private float burstForce = 1.5f;

    // バルーンの所持状態
    private bool hasBalloon = false;
    // バルーンが合体可能かどうか
    private bool enableMerge = true;
    [SerializeField]
    // バルーンの限界サイズ
    private float limitSize = 1.5f;
    // バルーンのサイズ倍率
    [SerializeField]
    private float sizeRate = 3.0f;
    [SerializeField]
    // 取得するバルーンサイズの減衰
    private float mergeDecay = 0.25f;
    // バルーンの現在サイズ
    private float balloonSize = 0.0f;
    [SerializeField]
    // バルーンの移動速度
    private float balloonLerpRate = 0.1f;

    [SerializeField]
    // 破裂時の効果音
    private AudioClip burstSE = null;

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
        int hitCount = Physics2D.CircleCastNonAlloc(transform.position, thisCollider.radius + 0.05f, targetVec.normalized,
            hits, distance, moveLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            distance = Mathf.Min(hits[i].distance, distance);
        }

        transform.position += targetVec.normalized * distance;


        if (Data.balloonSize < playerController.GetBoostCost())
        {
            renderer.color = Color.Lerp(renderer.color, uselessColor, 0.3f);
        }
        else
        {
            renderer.color = Color.Lerp(renderer.color, Color.white, 0.3f);
        }

        if (Data.balloonSize > limitSize * cautionRate)
        {
            cautionTimer += Time.deltaTime;
            renderer.color = Color.Lerp(Color.white, cautionColor, Mathf.Abs(Mathf.Sin(cautionTimer * Mathf.PI * 2 * cautionSpeed)));
        }
        else
        {
            cautionTimer = 0;
        }
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerStay2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enableMerge || collision.tag != "Bubble" || !collision.gameObject.GetComponent<BubbleController>().CanCatch())
        {
            return;
        }
        HitBubble(collision);
    }

    //------------------------------------------------------------------------------------------
    // 泡との接触処理
    //------------------------------------------------------------------------------------------
    private void HitBubble(Collider2D collision)
    {
        // バルーンを所持していない場合は初期化する
        if (!hasBalloon)
        {
            EnableBalloon(true);
        }

        // サイズ変更
        ChangeSize(Mathf.Abs(collision.transform.localScale.x*mergeDecay));

        // バブルの消滅
        collision.gameObject.GetComponent<BubbleController>().Destroy();
    }

    //------------------------------------------------------------------------------------------
    // バルーンの有効無効化
    //------------------------------------------------------------------------------------------
    private void EnableBalloon(bool enable)
    {
        renderer.enabled = enable;
        thisCollider.isTrigger = !enable;
        hasBalloon = enable;
    }

    //------------------------------------------------------------------------------------------
    // 合体の有効無効化
    //------------------------------------------------------------------------------------------
    public void EnableMerge(bool enable)
    {
        enableMerge = enable;
    }

    //------------------------------------------------------------------------------------------
    // バルーンのサイズ変更
    //------------------------------------------------------------------------------------------
    private void ChangeSize(float change_size)
    {
        // コライダーのサイズ変更
        float size = change_size * Mathf.Abs(change_size);

        bool burst = (balloonSize + size > limitSize * limitSize);

        balloonSize = Mathf.Clamp(balloonSize + size, 0, limitSize * limitSize);
        size = Mathf.Sqrt(balloonSize);
       
        thisCollider.radius = size * sizeRate/ 2;

        // 画像のサイズ変更
        renderObject.transform.localScale = Vector2.one * size * sizeRate;

        // 無くなったら無効化させる
        if (size <= Mathf.Epsilon)
        {
            EnableBalloon(false);
        }

        // サイズを更新
        Data.balloonSize = size;

        // サイズが大きくなりすぎたら破裂する
        if (burst)
        {
            SoundPlayer.Play(burstSE,0.7f);
            //Vector2 direction = playerController.transform.position - transform.position;
            //direction.x = (direction.x >= 0 ? burstDirection.x : -burstDirection.x);
            //direction.y = (direction.y >= 0 ? burstDirection.y : -burstDirection.y);
            //playerController.Boost(direction.normalized*burstForce);
            Burst();
        }
    }

    //------------------------------------------------------------------------------------------
    // バルーンの消費
    //------------------------------------------------------------------------------------------
    public bool UseBalloon(float useSize)
    {
        // サイズチェック
        if (Data.balloonSize < useSize)
        {
            return false;
        }

        // バルーンを減らす
        ChangeSize(-useSize);
        
        return true;
    }

    //------------------------------------------------------------------------------------------
    // ブースト移動
    //------------------------------------------------------------------------------------------
    public bool UseBoost(float useSize, float effectSize)
    {
        if (!UseBalloon(useSize))
        {
            return false;
        }
        
        EffectGenerator.BubbleBurstFX(
            new BubbleBurstFX.Param(Color.white, Vector2.one * effectSize),
            transform.position,
            null);

        return true;
    }

    //------------------------------------------------------------------------------------------
    // 破裂させる
    //------------------------------------------------------------------------------------------
    public void Burst()
    {
        GenerateBurstEffect();
        ChangeSize(-Data.balloonSize);
    }

    //------------------------------------------------------------------------------------------
    // 破裂エフェクトの生成
    //------------------------------------------------------------------------------------------
    private void GenerateBurstEffect()
    {
        EffectGenerator.BubbleBurstFX(
            new BubbleBurstFX.Param(Color.white, renderObject.transform.lossyScale),
            transform.position,
            null);
    }

}
