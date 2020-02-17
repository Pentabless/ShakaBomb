using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rig;

    // 接地フラグ
    bool isGround;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 速さ制限
        if (Mathf.Abs(rig.velocity.x) >= 5.0f)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rig.velocity = new Vector2(-5.0f, rig.velocity.y);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rig.velocity = new Vector2(5.0f, rig.velocity.y);
            }

        }

        // 左右移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rig.AddForce(new Vector2(-80.0f, 0));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rig.AddForce(new Vector2(80.0f, 0));
        }

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && isGround == true)
        {
            rig.AddForce(new Vector2(0, 700.0f));
            isGround = false;
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
