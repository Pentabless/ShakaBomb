using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rig;

    [SerializeField]
    BubbleGenerator bubbleG;

    // 接地フラグ
    bool isGround;

    // プレイヤーの向き
    float dir;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        dir = 0.0f;
    }

    // Update is called once per frame
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
            rig.velocity = new Vector2(0.0f, rig.velocity.y); ;
        }


        // 左右移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = -1.0f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = 1.0f;
        }
        else
        {
            dir = 0.0f;
        }

        // 切り替えし(地上にいるときのみ)
        if (isGround)
        {
            if (rig.velocity.x >= 0.0001f && Input.GetKey(KeyCode.LeftArrow))
            {
                Debug.Log("RtoL");
                bubbleG.BubbleCreate();
            }
            if (rig.velocity.x <= -0.0001f && Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log("LtoR");
                bubbleG.BubbleCreate();
            }
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
            rig.AddForce(new Vector2(0, 650.0f));
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
