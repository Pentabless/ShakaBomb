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
    // たわむ力
    private float power = 0.01f;
    [SerializeField]
    // たわみの減衰
    private float attenuation = 0.99f;
    // たわみ
    private Vector2 warp = Vector2.zero;

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

    //------------------------------------------------------------------------------------------
    // たわみ処理
    //------------------------------------------------------------------------------------------
    private void Warp(Collision2D collision)
    {
        if(Math.Abs(collision.contacts[0].normal.x) > 0)
        {
            if(warp.y > power)
            {
                warp.y -= power;
            }
            else
            {
                warp.x = Math.Min(warp.x + power - warp.y, 1.0f);
                warp.y = 0;
            }
        }
        else if (Math.Abs(collision.contacts[0].normal.y) > 0)
        {
            if (warp.x > power)
            {
                warp.x -= power;
            }
            else
            {
                warp.y = Math.Min(warp.y + power - warp.x, 1.0f);
                warp.x = 0;
            }
        }
    }

}
