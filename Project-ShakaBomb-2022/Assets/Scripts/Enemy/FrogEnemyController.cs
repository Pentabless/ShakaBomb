using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemyController : MonoBehaviour
{
    //
    Rigidbody2D rigid;
    //泡を出すインターバル
    int create_bubble_interval;
    //自身についている泡の色
    public Color attacks_color;
    //ジャンプしているか
    bool isJump;
    //地についているか
    bool isTouchGrond;
    //次にジャンプするまでのカウント
    int next_jump_count;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        create_bubble_interval = 0;
        isJump = false;
        isTouchGrond = false;
        next_jump_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ジャンプしているか
        if (isJump)
        {
            //地についているか
            if (isTouchGrond)
            {
                //ジャンプしていないことにする
                isJump = false;
                //次にジャンプするまでのカウントダウンを設定する
                next_jump_count = 300;
                //泡が出るインターバルをリセットする
                create_bubble_interval = 0;

                Debug.Log("カエル：ジャンプ終了");
            }
            else
            {
                Debug.Log("泡作るまで"+create_bubble_interval);
                create_bubble_interval--;
                if (create_bubble_interval <= 0)
                {
                    create_bubble_interval = 20;
                    //泡を発生させる
                    GameObject.Find("EnemyBubbleGenerator").GetComponent<EnemyBubbleGenerator>().CreateBubble(this.gameObject, Vector3.zero, attacks_color,GetComponent<EnemyController>().GetAttacksMoveForce(),true,false);
                }
            }
        }
        else
        {
            next_jump_count--;
            //地についていて次にジャンプする準備ができていてプレイヤーを見つけていたら
            if (isTouchGrond == true && next_jump_count <= 0&&GetComponent<EnemyController>().GetFindPlayer()==true)
            {
                //
                Debug.Log("カエル：準備完了");
                //ジャンプしている事にする
                isJump = true;
                //力を加える
                rigid.AddForce(GetComponent<EnemyController>().GetEnemyMoveForce());
            }
        }

        if (isTouchGrond)
        {
            Debug.Log("カエル着地状態：TRUE");
        }
        else
        {
            Debug.Log("カエル着地状態：FALSE");
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        //地についていたら
        if (collision.tag == "Ground")
        {
            //動いていなかったら
            if (rigid.velocity.y == 0.0f)
            {
                isTouchGrond = true;
                Debug.Log("カエル：着地！");
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //地についていたら
        if (collision.tag == "Ground")
        {
            isTouchGrond = false;
            Debug.Log("カエル：跳んでる！");
        }
    }
}