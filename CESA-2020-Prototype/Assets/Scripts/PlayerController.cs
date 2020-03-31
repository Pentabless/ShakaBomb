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

    // コントローラ
    [SerializeField]
    GameController gameController;

    // 移動力
    [SerializeField]
    float defaultPlayerSpeed;
    float playerSpeed;

    // ジャンプ力
    [SerializeField]
    float defaultJumpForce;
    float jumpForce;



    // 切り替えし猶予フレーム
    [SerializeField]
    int turnCount;

    // 接地フラグ
    bool isGround;
    bool bubbleGround;

    // プレイヤーの向き
    int dir;            // 現在
    int lastDir;      // 前フレーム
    int dirCount;       // 切り替えし用向き保持カウント

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        bubbleGround = false;
        dir = 0;
        lastDir = 0;
        dirCount = 0;
        jumpForce = defaultJumpForce;
        playerSpeed = defaultPlayerSpeed;
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
            if(bubbleGround)
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

        // プレイヤーの向き変更
        if (Data.playerDir != 1)
        {
            this.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
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

        // 重力の変更(バブルの個数に応じて)
        switch (Data.num_balloon)
        {
            case 0:
                this.rig.gravityScale = 5.0f;
                jumpForce = defaultJumpForce * 1.0f;
                playerSpeed = defaultPlayerSpeed * 1.0f;
                break;

            case 1:
                this.rig.gravityScale = 3.0f;
                jumpForce = defaultJumpForce * 0.9f;
                playerSpeed = defaultPlayerSpeed * 0.9f;
                break;

            case 2:
                this.rig.gravityScale = 2.0f;
                jumpForce = defaultJumpForce * 0.7f;
                playerSpeed = defaultPlayerSpeed * 0.85f;
                break;

            case 3:
                this.rig.gravityScale = 1.5f;
                jumpForce = defaultJumpForce * 0.6f;
                playerSpeed = defaultPlayerSpeed * 0.8f;
                break;

            default:
                break;
        }
    }

    private void FixedUpdate()
    {
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

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isGround = true;

        if (collision.tag == "GroundBubble")
        {
            bubbleGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGround = false;
        bubbleGround = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }
}

