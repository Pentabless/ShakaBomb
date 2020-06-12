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
        public float arc;     // 放出する角度
        public int orederInLayer; // 描画順

        public Param()
            : this(Color.white, Vector2.one, 360.0f, -99) { }

        public Param(Color color, Vector2 scale, float arc = 360.0f, int orderInLayer = -99)
        {
            this.color = color;
            this.scale = scale;
            this.arc = arc;
            this.orederInLayer = orderInLayer;
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
    private ParticleSystem.ShapeModule    shapeModule;          // シェイプモジュール
    private ParticleSystemRenderer renderModule;                // レンダラーモジュール
    private Param                         param;                // 設定パラメータ
    [SerializeField]                                            
    private int                           baseBurstCount = 15;  // サイズ1時のパーティクルの発生量
    [SerializeField]                                                  
    private float                         baseSizeRate = 0.75f; // サイズ1時のパーティクル初期サイズ
    [SerializeField]
    private float                         baseRadius = 0.7f;    // サイズ1時のエミット半径

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        // モジュールを取得する
        system = GetComponent<ParticleSystem>();
        mainModule = system.main;
        emissionModule = system.emission;
        shapeModule = system.shape;
        renderModule = GetComponent<ParticleSystemRenderer>();
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
    // パーティクルの発生
    //------------------------------------------------------------------------------------------
    public void Emit(in Vector3 pos)
    {
        transform.position = pos;
        ApplyParam();
        system.Emit((int)emissionModule.GetBurst(0).count.constant);
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

        float rate = param.scale.magnitude;

        mainModule.startSize = baseSizeRate * rate;

        var burst = emissionModule.GetBurst(0);
        burst.count = baseBurstCount * rate;
        emissionModule.SetBurst(0, burst);

        shapeModule.radius = baseRadius * rate;
        shapeModule.arc = param.arc;

        if(param.orederInLayer != -99)
        {
            renderModule.sortingOrder = param.orederInLayer;
        }
    }
}
