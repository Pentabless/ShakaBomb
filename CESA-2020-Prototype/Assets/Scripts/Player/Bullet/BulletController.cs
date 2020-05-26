using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // 浮力の種類
    enum FloatPowerType
    {
        None,       // 無し
        Sin,        // サイン
        AbsSin,     // サインの絶対値
    }

    // リジッドボディ
    Rigidbody2D rig = null;

    // 消えるかどうか
    bool isDestroy = false;
    // 発射したかどうか
    bool wasShoted = false;

    [SerializeField]
    // 発射速度
    float shotPower = 420.0f;
    [SerializeField]
    // 浮力の種類
    FloatPowerType floatPowerType = FloatPowerType.AbsSin;
    [SerializeField]
    // 発射時の浮力
    float floatPower = 200.0f;
    // 発射方向
    float shotAngle = 0.0f;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isDestroy = false;
        wasShoted = false;
    }

    void Update()
    {
        if (!wasShoted)
        {
            var force = new Vector2(Mathf.Cos(shotAngle), Mathf.Sin(shotAngle)) * shotPower;
            switch (floatPowerType)
            {
                case FloatPowerType.Sin:
                    force.y += (Mathf.Sin(shotAngle) + 1.0f) * floatPower;
                    break;
                case FloatPowerType.AbsSin:
                    force.y += (Mathf.Abs(Mathf.Sin(shotAngle)) + 1.0f) * floatPower;
                    break;
            }
            rig.AddForce(force);
            wasShoted = true;
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
    // 発射方向を設定する
    //------------------------------------------------------------------------------------------
    public void SetAngle(float angle)
    {
        shotAngle = angle;
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
