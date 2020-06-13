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
        MinusSin,   // マイナスサイン
        AbsSin,     // サインの絶対値
    }

    // リジッドボディ
    Rigidbody2D rig = null;

    // 発射したかどうか
    bool wasShoted = false;
    // 敵に当たったかどうか
    bool hitEnemy = false;

    [SerializeField]
    // 発射速度
    float shotPower = 14.0f;
    [SerializeField]
    // 浮力の種類
    FloatPowerType floatPowerType = FloatPowerType.Sin;
    [SerializeField]
    // 発射時の浮力
    float floatPower = 3.5f;
    // 発射方向
    float shotAngle = 0.0f;

    [SerializeField]
    // 破裂時の効果音
    private AudioClip burstSE = null;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        wasShoted = false;
    }
    float t = 0;Vector3 f;Vector3 p;
    void Update()
    {
        if (!wasShoted)
        {
            var force = CalcShotForce(shotAngle);
            rig.AddForce(force, ForceMode2D.Impulse);
            wasShoted = true;
            f = force;
            p = transform.position;
        }
        else
        {
            t += Time.deltaTime;
            //Debug.Log("real:" + transform.position + " t:"+t);
            //Debug.Log("calc:" + (p + f * t + Vector3.up * 0.5f * rig.gravityScale*Physics2D.gravity.y * t * t) + " t:" + t);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == Common.Enemy.HIT_STATE)
            {
                hitEnemy = true;
            }
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
    // 発射時の力を計算する
    //------------------------------------------------------------------------------------------
    public Vector2 CalcShotForce(float angle)
    {
        var force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * shotPower;
        switch (floatPowerType)
        {
            case FloatPowerType.Sin:
                force.y += (Mathf.Sin(angle) + 1.0f) * floatPower;
                break;
            case FloatPowerType.MinusSin:
                force.y += (-Mathf.Sin(angle) + 1.0f) * floatPower;
                break;
            case FloatPowerType.AbsSin:
                force.y += (Mathf.Abs(Mathf.Sin(angle)) + 1.0f) * floatPower;
                break;
        }

        return force;
    }

    //------------------------------------------------------------------------------------------
    // 破裂処理
    //------------------------------------------------------------------------------------------
    private void Destroy()
    {
        if (!hitEnemy)
        {
            GenerateBurstEffect();
            SoundPlayer.Play(burstSE, 0.3f);
        }
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
