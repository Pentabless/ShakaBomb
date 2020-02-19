using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
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

    //目的の大きさになるまでの時間(フレーム数)
    int target_scale_time;

    //大きくなる限度
    Vector3 limit_scale;
    //限度を達成したか
    bool limit_check;

    // Start is called before the first frame update
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

        target_scale_time = 60;

        limit_check = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 移動する
        transform.Translate(Mathf.Sin(angle) * move_force.x, move_force.y, 0.0f);
        angle += 0.1f;
        //ある程度の高さまで来たら
        if (transform.position.y >= 10.0f)
        {
            //Vector3 position = new Vector3(transform.position.x, -0.25f, 0.0f);
            //transform.position = position;
            //angle = 0.0f;

            isDestroy = true;
        }

        //色の変更
        if (isTouchBubble)   //本体同士が当たっていたら
        {
            GetComponent<Renderer>().material.color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            if (isTouchSticky)   //本体の粘着範囲と相手の粘着範囲が当たっていたら
            {
                GetComponent<Renderer>().material.color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            }
            else      //何も当たっていなかったら
            {
                GetComponent<Renderer>().material.color = start_color;
            }
        }

        //大きさを変更する
        if (now_scale != target_scale) //今の大きさと目的の大きさが違っていたら
        {
            now_scale += (target_scale - now_scale) / target_scale_time;

            //目的の大きさに近づいたら
            if (Mathf.Abs(target_scale.x-now_scale.x)<=0.1f)
            {
                //目的の大きさにする
                now_scale = target_scale;
            }

            transform.localScale = now_scale;
        }

        //今の大きさが限度を超えていたら
        if((limit_check==false)&&(now_scale.x>=limit_scale.x))
        {
            limit_check = true;
        }

        //消えようとしていたら
        if (isDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    //<自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    //大きくなる限度を設定する
    public void SetLimitScale(Vector3 scale)
    {
        limit_scale = scale;
    }

    //大きくなる限度を渡す
    public Vector3 GetLimitScale()
    {
        return limit_scale;
    }

    //相手が消えようとしているかをもらう
    public bool GetDestroy()
    {
        return isDestroy;
    }

    //本体が当たった瞬間
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bubble")
        {
            Debug.Log("ParentHit");
            isTouchBubble = true;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Bubble")
        {
            isTouchBubble = true;
        }
    }

    //本体が当たり終わった瞬間
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Bubble")
        {
            isTouchBubble = false;
        }
    }

    //粘着範囲が当たった瞬間
    public void StickyTriggerEnter(Collider2D collision)
    {
        Debug.Log("StickyHit");

        isTouchSticky = true;

        //当たった泡
        BubbleController Hit_object = collision.GetComponentInParent<BubbleController>();

        //自身　かつ　当たった泡の　目標の大きさが　限度の大きさより小さかったら
        if ((target_scale.x < limit_scale.x) && (Hit_object.target_scale.x < Hit_object.limit_scale.x))
        {
            //自身より相手の泡が大きかったら
            if(transform.localScale.x<Hit_object.transform.localScale.x)
            {
                //自身が消えるようにする
                isDestroy = true;
            }
            //当たった泡が消えようとしていたら
            else if (Hit_object.GetDestroy())
            {
                //「自身の大きさ」に「当たった泡の目的の大きさ」分を「目的の大きさ」に設定する
                target_scale += collision.transform.localScale;

                //目的の大きさが限度の大きさより大きくなったら
                if (target_scale.x >= limit_scale.x)
                {
                    //目的の大きさを限度の大きさに設定しなおす
                    target_scale = limit_scale;
                }
            }
            else
            {
                //自身が消えるようにする
                isDestroy = true;
            }
        }
    }

    //粘着範囲が当たり終わった瞬間
    public void StickyTriggerExit(Collider2D collision)
    {
        isTouchSticky = false;
    }
}
