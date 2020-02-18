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
            Vector3 position = new Vector3(transform.position.x, -0.25f, 0.0f);
            transform.position = position;
            angle = 0.0f;
        }

        //色の変更
        if (isTouchBubble)   //本体同士が当たっていたら
        {
            GetComponent<Renderer>().material.color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            if (isTouchSticky)   //本体の粘着範囲と相手の本体が当たっていたら
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
            now_scale += (target_scale - now_scale)/target_scale_time;

            //目的の大きさより大きくなったら
            if (now_scale.x >= target_scale.x)
            {
                now_scale = target_scale;
            }

            transform.localScale = now_scale;
        }

        //消えようとしていたら
        if (isDestroy)
        {
            Destroy(this.gameObject);
        }
    }

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

        //当たった泡が消えようとしていたら
        if (collision.GetComponentInParent<BubbleController>().GetDestroy())
        {
            //自身の大きさに当たった泡の大きさ分を目的の大きさに設定する
            target_scale += collision.transform.localScale;
        }
        else
        {
            //自身が消えるようにする
            isDestroy = true;
        }
    }

    //粘着範囲が当たり終わった瞬間
    public void StickyTriggerExit(Collider2D collision)
    {
        isTouchSticky = false;
    }
}
