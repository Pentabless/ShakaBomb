//==============================================================================================
/// File Name	: BubbleBurstFX.cs
/// Summary		: 泡の破裂エフェクト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
[RequireComponent(typeof(ParticleSystem))]
//==============================================================================================
public class BubbleBurstFX : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // 設定パラメータ
    //------------------------------------------------------------------------------------------
    public class Param
    {
        public Color color;   // 色
        public Vector2 scale; // 元オブジェクトのサイズ

        public Param()
            : this(Color.white, Vector2.one) { }

        public Param(in Color color, in Vector2 scale)
        {
            this.color = color;
            this.scale = scale;
        }
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private bool                          isPlaying = false;    // 再生中かどうか
    private float                         lifeTime = 2.0f;      // オブジェクトを破棄するまでの時間
    private ParticleSystem                system = null;        // パーティクルシステム
    private ParticleSystem.MainModule     mainModule;           // メインモジュール
    private ParticleSystem.EmissionModule emissionModule;       // エミッションモジュール
    private Param                         param;                // 設定パラメータ
    [SerializeField]                                            
    private int                           baseBurstCount = 15;  // サイズ1時のパーティクルの発生量
    [SerializeField]                                                  
    private float                         baseSizeRate = 0.75f; // サイズ1時のパーティクル初期サイズ

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        // モジュールを取得する
        system = GetComponent<ParticleSystem>();
        mainModule = system.main;
        emissionModule = system.emission;
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
        mainModule.startColor = param.color;

        mainModule.startSize = baseSizeRate * param.scale.magnitude;

        var burst = emissionModule.GetBurst(0);
        burst.count = baseBurstCount * param.scale.magnitude;
        emissionModule.SetBurst(0, burst);
    }
}
