//==============================================================================================
/// File Name	: BubbleDistortionController.cs
/// Summary		: 泡の歪み制御
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class BubbleDistortionController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // リジッドボディ
    private Rigidbody2D rig = null;
    // レンダラー
    private new Renderer renderer = null;
    [SerializeField]
    [Header("物理判定をとる場合のみ")]
    // コライダー
    private new Collider2D collider = null;

    [SerializeField]
    // たわむ力
    private float warpPower = 0.01f;
    [SerializeField]
    // たわみの減衰
    private float attenuation = 0.99f;
    // たわみ
    private Vector2 warp = Vector2.zero;

    [SerializeField]
    // 反発力
    private float reflectPower = 50.0f;
    [SerializeField]
    // 物理判定をしない時間
    private float isTriggerTime = 0.1f;
    // 衝突している数
    private int hitCount = 0;

    // シェーダーのシード値
    private float seed = 0;
    // 生成されてからの時間
    private float time = 0;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        seed = UnityEngine.Random.Range(0.0f, 1.0f);
        rig = GetComponent<Rigidbody2D>();
        if (!rig)
        {
            rig = GetComponentInParent<Rigidbody2D>();
        }
        renderer = GetComponent<Renderer>();
        if (!renderer)
        {
            renderer = GetComponentInChildren<Renderer>();
        }

        // 生成された直後は物理判定をしない
        if (collider)
        {
            collider.enabled = false;
        }
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
     
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        time += Time.deltaTime;

        // たわみを減衰させる
        warp *= attenuation;

        renderer.material.SetFloat("_Seed", seed);
        renderer.material.SetFloat("_Timer", time);
        renderer.material.SetVector("_ScaleVec", warp);

        // 一定時間経過かつ衝突しているオブジェクトがない場合、物理判定を戻す
        if (collider && time >= isTriggerTime && hitCount == 0)
        {
            collider.enabled = true;
        }

    }

    //------------------------------------------------------------------------------------------
    // OnCollisionEnter2D
    //------------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TargetCheck(collision))
        {
            return;
        }

        Warp(collision);
    }

    //------------------------------------------------------------------------------------------
    // OnCollisionStay2D
    //------------------------------------------------------------------------------------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (TargetCheck(collision))
        {
            return;
        }

        Warp(collision);
    }

    //------------------------------------------------------------------------------------------
    // OnCollisionExit2D
    //------------------------------------------------------------------------------------------
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (TargetCheck(collision))
        {
            return;
        }

        // 離れた瞬間に大きく減衰させる
        warp *= attenuation * 0.7f;

    }

    //------------------------------------------------------------------------------------------
    // OnTriggerEnter2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TargetCheck(collision))
        {
            return;
        }
        hitCount += (collision.isTrigger ? 0 : 1);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collider || collider.isTrigger)
        {
            return;
        }

        if (collision.gameObject.tag == "Balloon")
        {
            Vector2 vec = (transform.position - collision.transform.position).normalized;
            rig.AddForce(vec * reflectPower);
        }
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerExit2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (TargetCheck(collision))
        {
            return;
        }
        hitCount -= (collision.isTrigger ? 0 : 1);
    }

    //------------------------------------------------------------------------------------------
    // 対象外ならtrueを返す
    //------------------------------------------------------------------------------------------
    private bool TargetCheck(Collision2D collision)
    {
        if (collision.gameObject.tag == "Balloon")
        {
            return true;
        }

        return false;
    }
    private bool TargetCheck(Collider2D collision)
    {
        if (collision.gameObject.tag == "Balloon")
        {
            return true;
        }

        return false;
    }

    //------------------------------------------------------------------------------------------
    // たわみ処理
    //------------------------------------------------------------------------------------------
    private void Warp(Collision2D collision)
    {
        if(Math.Abs(collision.contacts[0].normal.x) > 0)
        {
            if(warp.y > warpPower)
            {
                warp.y -= warpPower;
            }
            else
            {
                warp.x = Math.Min(warp.x + warpPower - warp.y, 1.0f);
                warp.y = 0;
            }
        }
        else if (Math.Abs(collision.contacts[0].normal.y) > 0)
        {
            if (warp.x > warpPower)
            {
                warp.x -= warpPower;
            }
            else
            {
                warp.y = Math.Min(warp.y + warpPower - warp.x, 1.0f);
                warp.x = 0;
            }
        }
    }

}
