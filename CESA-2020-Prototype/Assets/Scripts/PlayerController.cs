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

    // 切り替えし猶予フレーム
    [SerializeField]
    int turnCount;

    // 接地フラグ
    bool isGround;

    // プレイヤーの向き
    int dir;            // 現在
    float lastDir;      // 前フレーム
    int dirCount;       // 切り替えし用向き保持カウント

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        dir = 0;
        lastDir = 0.0f;
        dirCount = 0;
    }

    void Update()
    {
        // 速さ制限
        if (dir >= 1.0f && rig.velocity.x >= 5.0f)
        {
            rig.velocity = new Vector2(5.0f, rig.velocity.y);
        }
        else if (dir <= -1.0f && rig.velocity.x <= -5.0f)
        {
            rig.velocity = new Vector2(-5.0f, rig.velocity.y);
        }
        // Velocity最小化
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

        // 移動
        if (isGround)
        {
            rig.AddForce(new Vector2(160.0f * dir, 0));
        }
        else
        {
            rig.AddForce(new Vector2(40.0f * dir, 0));
        }

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            rig.AddForce(new Vector2(0, 680.0f));
            isGround = false;
            Debug.Log("");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGround = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }
}

