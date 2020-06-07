using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BubbleController : MonoBehaviour
{
    // プレイヤーの情報
    GameObject playerObj;

    BalloonGenerator balloonG;

    // 動く力の大きさ
    Vector2 move_force;
    // 左右に円運動するための角度
    float angle;

    // 消えるかどうか
    bool isDestroy;
    //初期の色
    Vector4 start_color;

    //泡同士が当たっている
    bool isTouchBubble;
    //泡の粘着範囲同士が当たっている
    bool isTouchSticky;

    //現在の大きさ
    Vector3 now_scale;
    //目的の大きさ
    Vector3 target_scale;

    // 保持状態か否か
    bool ret_flag;

    // 保持できるか否か
    bool test_flag;

    // 泡のカウント
    private int catchCount = 0;
    // 消滅までのカウント
    private int deleteCount = 0;

    // 出現してからの時間
    private float time = 0.0f;
    // バルーンと接触してからの時間
    private float hitedTime = 0.0f;
    [SerializeField]
    // バルーンと合体できるようになるまでの時間
    private float canCatchTime = 0.3f;
    // 合体先のオブジェクト
    private GameObject mergeTarget = null;
    // 追跡を止めるかどうか
    private bool stopChaise = false;
    // 前回の移動速度
    private float preVelovity = 0.0f;


    //目的の大きさになるまでの時間(フレーム数)
    int target_scale_time;

    void Start()
    {
        move_force = new Vector2(0.05f, Random.Range(1.0f, 3.0f) * 0.025f);
        angle = 0.0f;
        isDestroy = false;

        start_color = GetComponent<Renderer>().material.color;
        isTouchBubble = false;
        isTouchSticky = false;

        now_scale = transform.localScale;
        target_scale = now_scale;

        ret_flag = false;
        test_flag = true;

        target_scale_time = 60;

        playerObj = GameObject.Find(Player.NAME);
        //balloonG = GameObject.Find(Balloon.GENERATOR).GetComponent<BalloonGenerator>();
    }


    void Update()
    {
        catchCount++;
        deleteCount++;
        time += Time.deltaTime;

        if (!ret_flag)
        {
            NormalUpdate();
        }
        else
        {
            RetUpdate();
        }
    }

    // 通常時の泡の動き
    void NormalUpdate()
    {
        var velovity = new Vector3(Mathf.Sin(angle) * move_force.x, move_force.y, 0.0f);
        // 移動する
        if (!mergeTarget)
        {
            transform.Translate(velovity);
        }
        // 合体先のオブジェクトがある場合
        else
        {
            hitedTime += Time.deltaTime;

            var targetVec = Vector3.Lerp(transform.position, mergeTarget.transform.position, Mathf.Min(hitedTime / canCatchTime, 1.0f));
            targetVec -= transform.position;
            velovity = Vector3.Lerp(velovity, targetVec, Mathf.Min(time / canCatchTime, 1.0f));
            float magnitude = Mathf.Clamp(velovity.magnitude, preVelovity - 0.05f, preVelovity + 0.05f);
            velovity = velovity.normalized * magnitude;
            transform.Translate(velovity);
        }
        preVelovity = velovity.magnitude;
        angle += 0.1f;

        //バブルの消滅(カウント)
        if (deleteCount >= Bubble.EXTINCTION_TIME)
        {
            //Vector3 position = new Vector3(transform.position.x, -0.25f, 0.0f);
            //transform.position = position;
            //angle = 0.0f;

            GenerateBurstEffect();

            isDestroy = true;
        }

        ////色の変更
        //if (isTouchBubble)   //本体同士が当たっていたら
        //{
        //    GetComponent<Renderer>().material.color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        //}
        //else
        //{
        //    if (isTouchSticky)   //本体の粘着範囲と相手の本体が当たっていたら
        //    {
        //        GetComponent<Renderer>().material.color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        //    }
        //    else      //何も当たっていなかったら
        //    {
        //        GetComponent<Renderer>().material.color = start_color;
        //    }
        //}

        ////大きさを変更する
        //if (now_scale != target_scale) //今の大きさと目的の大きさが違っていたら
        //{
        //    now_scale += (target_scale - now_scale) / target_scale_time;

        //    //目的の大きさより大きくなったら
        //    if (now_scale.x >= target_scale.x)
        //    {
        //        now_scale = target_scale;
        //    }

        //    if (Bubble.MAX_SIZE > now_scale.x)
        //    {
        //        transform.localScale = now_scale;
        //    }
        //}


        //if (Data.num_balloon >= Balloon.MAX)
        //{
        //    test_flag = false;
        //}

        //if (catchCount > Balloon.COUNT)
        //{
        //    test_flag = false;
        //}

        //// 保持状態に切り替え
        //if (transform.localScale.x >= Bubble.MAX_SIZE * 0.9f && Data.num_balloon < Balloon.MAX && test_flag)
        //{
        //    ret_flag = true;
        //    //balloonG.CreateBalloon(this.transform.position);
        //    isDestroy = true;
        //}

        // 消えようとしていたら
        if (isDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    // 保持状態の泡の動き
    void RetUpdate()
    {
        Vector3 playerPos = playerObj.transform.position;

        playerPos.x += 0.5f;
        playerPos.y += 0.8f;

        transform.position = Vector3.Lerp(transform.position, playerPos, 0.03f);

        //// 移動する
        //transform.Translate(Mathf.Sin(angle) * move_force.x, 0.0f, 0.0f);
        //angle += 0.1f;

        //大きさを変更する
        if (now_scale != target_scale) //今の大きさと目的の大きさが違っていたら
        {
            now_scale += (target_scale - now_scale) / target_scale_time;

            //目的の大きさより大きくなったら
            if (now_scale.x >= target_scale.x)
            {
                now_scale = target_scale;
            }
        }
    }


    public bool GetDestroy()
    {
        return isDestroy;
    }

    public void Destroy()
    {
        isDestroy = true;
    }

    //本体が当たった瞬間
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //// バルーンに触れたら追従するようにする
        //if(collision.tag == Balloon.NAME)
        //{
        //    mergeTarget = collision.gameObject;
        //}

        //if (collision.tag == Bubble.NAME)
        //{
        //    //Debug.Log("ParentHit");
        //    isTouchBubble = true;
        //}
    }

    //本体が当たり終わった瞬間
    public void OnTriggerExit2D(Collider2D collision)
    {
        // バルーンから離れたら追従するようにする
        if (collision.tag == Balloon.NAME && !stopChaise)
        {
            mergeTarget = collision.gameObject;
        }
        //stopChaise = false;
        //if (collision.tag == Bubble.NAME)
        //{
        //    isTouchBubble = false;
        //}
    }

    //粘着範囲が当たった瞬間
    public void StickyTriggerEnter(Collider2D collision)
    {
        ////Debug.Log("StickyHit");

        //isTouchSticky = true;

        ////当たった泡が消えようとしていたら
        //if (collision.GetComponentInParent<BubbleController>().GetDestroy())
        //{
        //    //自身の大きさに当たった泡の大きさ分を目的の大きさに設定する
        //    target_scale += collision.transform.localScale;
        //}
        //else
        //{
        //    //自身が消えるようにする
        //    isDestroy = true;
        //}
    }

    //粘着範囲が当たり終わった瞬間
    public void StickyTriggerExit(Collider2D collision)
    {
        isTouchSticky = false;
    }

    //------------------------------------------------------------------------------------------
    // 触れらるかどうか取得する
    //------------------------------------------------------------------------------------------
    public bool CanCatch()
    {
        return time >= canCatchTime;
    }

    //------------------------------------------------------------------------------------------
    // ターゲットの追跡をやめる
    //------------------------------------------------------------------------------------------
    public void StopChase()
    {
        mergeTarget = null;
        stopChaise = true;
    }

    //------------------------------------------------------------------------------------------
    // 破裂エフェクトの生成
    //------------------------------------------------------------------------------------------
    private void GenerateBurstEffect()
    {
        EffectGenerator.BubbleBurstFX(
            new BubbleBurstFX.Param(GetComponent<SpriteRenderer>().color, transform.localScale),
            transform.localPosition,
            null);
    }
}
