﻿//==============================================================================================
/// File Name	: CleanFX.cs
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class CleanFX : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // 設定パラメータ
    //------------------------------------------------------------------------------------------
    public class Param
    {
        public Vector2 areaSize; // エフェクトの表示領域

        public Param()
            : this(Vector2.one) { }

        public Param(Vector2 areaSize)
        {
            this.areaSize = areaSize;
        }
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private bool isPlaying = false;    // 再生中かどうか
    private float lifeTime = 2.0f;      // オブジェクトを破棄するまでの時間
    private ParticleSystem system = null;        // パーティクルシステム
    private ParticleSystem.MainModule mainModule;           // メインモジュール
    private ParticleSystem.ShapeModule shapeModule;          // シェイプモジュール
    private Param param;                // 設定パラメータ

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        // モジュールを取得する
        system = GetComponent<ParticleSystem>();
        mainModule = system.main;
        shapeModule = system.shape;
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
        shapeModule.scale = new Vector3(param.areaSize.x, param.areaSize.y, 0);
    }
}
