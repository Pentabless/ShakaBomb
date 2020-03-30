using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // リジッドボディ
    Rigidbody2D rig;

    // バブルジェネレータ
    [SerializeField]
    BubbleGenerator bubbleG;

    // バレットジェネレータ
    [SerializeField]
    BulletGenerator bulletG;

    [SerializeField]
    float playerSpeed;
    [SerializeField]
    float jumpForce;

    // 切り替えし猶予フレーム
    [SerializeField]
    int turnCount;

    // 接地フラグ
    bool isGround;

    // プレイヤーの向き
    int dir;            // 現在
    int lastDir;      // 前フレーム
    int dirCount;       // 切り替えし用向き保持カウント

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        dir = 0;
        lastDir = 0;
        dirCount = 0;
    }

    void Update()
    {
        // Velocity最小化(一定数以下で0)
        if (Mathf.Abs(rig.velocity.x) <= 0.001f)
        {
            rig.velocity = new Vector2(0.0f, rig.velocity.y);
        }

        // 左右移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = -1;
            dirCount = turnCount;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = 1;
            dirCount = turnCount;
        }
        else
        {
            dir = 0;
        }

        // 切り替えし(地上にいるときのみ)
        if (isGround)
        {
            if (lastDir > 0.0f && dir == -1)
            {
                //Debug.Log("RtoL");
                bubbleG.BubbleCreate();
            }
            if (lastDir < 0.0f && dir == 1)
            {
                //Debug.Log("LtoR");
                bubbleG.BubbleCreate();
            }
        }

        // 最終入力方向
        dirCount--;
        if (dirCount <= 0 || dir != 0)
        {
            lastDir = dir;
        }

        // プレイヤーの向き保持
        if (dir != 0)
        {
            Data.playerDir = dir;
        }

        // 移動
        if (isGround)
        {
            rig.AddForce(new Vector2(playerSpeed * dir, 0));
        }
        else
        {
            rig.AddForce(new Vector2(playerSpeed / 3.0f * dir, 0));
        }

        // 速さ制限
        if (dir >= 1.0f && rig.velocity.x >= 5.0f)  // 右側
        {
            rig.velocity = new Vector2(5.0f, rig.velocity.y);
        }
        else if (dir <= -1.0f && rig.velocity.x <= -5.0f)   // 左側
        {
            rig.velocity = new Vector2(-5.0f, rig.velocity.y);
        }


        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            rig.AddForce(new Vector2(0, jumpForce));
            isGround = false;
            Debug.Log("");
        }

        // バレットの発射
        if (Input.GetKeyDown(KeyCode.C) && Data.num_balloon >= 1)
        {
            bulletG.BulletCreate(this.transform.position);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (collision.tag == "Ground")
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGround = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }
}

