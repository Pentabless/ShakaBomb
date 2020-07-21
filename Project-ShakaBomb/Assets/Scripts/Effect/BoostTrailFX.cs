//==============================================================================================
/// File Name	: BoostTrailFX.cs
/// Summary		: ブースト移動時の軌跡エフェクト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class BoostTrailFX : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // 設定パラメータ
    //------------------------------------------------------------------------------------------
    public class Param
    {
        public Color color;       // 色
        public float duration;    // パーティクルを出す時間
        public Rigidbody2D rigid; // 追従するオブジェクトのリジッドボディ

        public Param()
            : this(Color.white, 0.0f, null) { }

        public Param(Color color, float duration, Rigidbody2D rigid)
        {
            this.color = color;
            this.duration = duration;
            this.rigid = rigid;
        }
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private bool isPlaying = false;                 // 再生中かどうか
    private float lifeTime = 0.0f;                  // オブジェクトを破棄するまでの時間
    private ParticleSystem system = null;           // パーティクルシステム
    private ParticleSystem.MainModule mainModule;   // メインモジュール
    private Param param;                            // 設定パラメータ

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        // モジュールを取得する
        system = GetComponent<ParticleSystem>();
        mainModule = system.main;
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
        if (!isPlaying)
        {
            return;
        }

        if (param.rigid)
        {
            transform.position = param.rigid.position;
            float angle = Mathf.Atan2(param.rigid.velocity.y, param.rigid.velocity.x);
            transform.eulerAngles = new Vector3((-angle+Mathf.PI)*Mathf.Rad2Deg, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    //------------------------------------------------------------------------------------------
    // パーティクルの再生
    //------------------------------------------------------------------------------------------
    public void Play()
    {
        ApplyParam();
        system.Play();
        lifeTime = mainModule.duration + mainModule.startLifetime.constant;
        isPlaying = true;
    }

    //------------------------------------------------------------------------------------------
    // パラメータの設定
    //------------------------------------------------------------------------------------------
    public void SetParam(Param setParam)
    {
        param = setParam;
    }

    //------------------------------------------------------------------------------------------
    // パラメータの適用
    //------------------------------------------------------------------------------------------
    private void ApplyParam()
    {
        mainModule.duration = param.duration;
        mainModule.startColor = param.color;
    }
}
