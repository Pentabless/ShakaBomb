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

    // バルーンジェネレータ
    [SerializeField]
    BalloonGenerator balloonG;

    // バレットジェネレータ
    [SerializeField]
    BulletGenerator bulletG;

    // コントローラ
    [SerializeField]
    GameController gameController;
    bool checkController;

    // 移動力
    [SerializeField]
    float defaultPlayerSpeed;
    float playerSpeed;
    float stickSence;       // 入力感度

    // ジャンプ関連
    [SerializeField]
    float defaultJumpForce;
    float jumpForce;
    int jumpCount;
    bool jumpStopFlag;

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
        checkController = false;
        dir = 0;
        lastDir = 0;
        dirCount = 0;
        jumpForce = defaultJumpForce;
        jumpCount = 0;
        jumpStopFlag = false;
        playerSpeed = defaultPlayerSpeed;
    }

    void Update()
    {
        // Velocity最小化(一定数以下で0)
        if (Mathf.Abs(rig.velocity.x) <= 0.001f)
        {
            rig.velocity = new Vector2(0.0f, rig.velocity.y);
        }

        // プレイヤー操作系統 (入力が必要なもの)--------------------------------------------------

        // コントローラの接続チェック
        checkController = gameController.GetCheckGamepad();

        // コントローラ未接続時
        if (!checkController)
        {
            // 入力感度初期化
            stickSence = 1;

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

            // ジャンプ
            if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
            {
                rig.AddForce(new Vector2(0, jumpForce));
                isGround = false;
                jumpStopFlag = false;
            }
            if (Input.GetKeyUp(KeyCode.Z) && isGround == false && !jumpStopFlag)
            {
                rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y * 0.4f);
                jumpStopFlag = true;
            }


            // バレットの発射
            if (Input.GetKeyDown(KeyCode.C) && Data.num_balloon >= 1)
            {
                bulletG.BulletCreate(this.transform.position);
                balloonG.UsedBubble();
            }
        }

        // コントローラ接続時
        else if (checkController)
        {
            // 左右移動
            if (Input.GetAxis("Horizontal") < 0)
            {
                dir = -1;
                dirCount = turnCount;
                stickSence = Input.GetAxis("Horizontal");
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                dir = 1;
                dirCount = turnCount;
                stickSence = Input.GetAxis("Horizontal");
            }
            else
            {
                dir = 0;
            }
            // ジャンプ
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) && isGround == true)
            {
                rig.AddForce(new Vector2(0, jumpForce));
                isGround = false;
                jumpStopFlag = false;
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button0) && isGround == false && !jumpStopFlag)
            {
                rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y * 0.4f);
                jumpStopFlag = true;
            }

            // バレットの発射
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) && Data.num_balloon >= 1)
            {
                bulletG.BulletCreate(this.transform.position);
                balloonG.UsedBubble();
            }
        }

        // ------------------------------------------------------------------------------

        // 切り替えし(地上にいるときのみ)
        if (isGround)
        {
            if (bubbleGround)
            {
                if (lastDir > 0.0f && dir == -1)
                {
                    bubbleG.BubbleCreate();
                }
                if (lastDir < 0.0f && dir == 1)
                {
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

        // プレイヤーのX方向速度取得
        Data.playerVelX = this.rig.velocity.x;

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
                jumpForce = defaultJumpForce * 0.77f;
                playerSpeed = defaultPlayerSpeed * 0.9f;
                break;

            case 2:
                this.rig.gravityScale = 2.0f;
                jumpForce = defaultJumpForce * 0.67f;
                playerSpeed = defaultPlayerSpeed * 0.85f;
                break;

            case 3:
                this.rig.gravityScale = 1.5f;
                jumpForce = defaultJumpForce * 0.57f;
                playerSpeed = defaultPlayerSpeed * 0.8f;
                break;

            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        // 入力感度 絶対値化
        stickSence = Mathf.Abs(stickSence);

        // 移動
        if (isGround)
        {
            rig.AddForce(new Vector2(playerSpeed * dir * stickSence, 0));
        }
        else
        {
            rig.AddForce(new Vector2(playerSpeed / 3.0f * dir * stickSence, 0));
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
        //if (collision.gameObject.tag == "DamageTile")
        //{
        //    balloonG.UsedBubble();
        //}
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

