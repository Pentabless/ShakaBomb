using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // リジッドボディ
    Rigidbody2D rig;

    // 消えるかどうか
    bool isDestroy;

    // 発射フラグ
    bool isShot;

    float shotDir;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isDestroy = false;
        isShot = false;
    }

    void Update()
    {
        if (!isShot)
        {
            if (Data.playerDir > 0)
            {
                rig.AddForce(new Vector2(400 + Data.playerVelX * 30.0f, 200));
            }
            else
            {
                rig.AddForce(new Vector2(-400 + Data.playerVelX * 30.0f, 200));
            }
            isShot = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Destroy();
        }
    }

    //------------------------------------------------------------------------------------------
    // 破裂処理
    //------------------------------------------------------------------------------------------
    private void Destroy()
    {
        GenerateBurstEffect();
        Destroy(this.gameObject);
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
