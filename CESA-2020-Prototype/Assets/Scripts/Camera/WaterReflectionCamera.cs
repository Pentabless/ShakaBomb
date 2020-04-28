//==============================================================================================
/// File Name	: WaterReflectionCamera.cs
/// Summary		: 水面反射用カメラ
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
[ExecuteInEditMode]
public class WaterReflectionCamera : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private UnityEngine.Camera mainCamera = null;  // メインカメラ
    private GameObject refCameraObject = null;     // 反射用カメラオブジェクト
    private UnityEngine.Camera refCamera = null;   // 反射用カメラ

    private string globalTextureName = "_SSWaterReflectionsTex";    // シェーダーでテクスチャを扱う時の名前

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {

    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        mainCamera = GetComponent<UnityEngine.Camera>();
        InitRefCamera();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        if (!refCamera)
        {
            return;
        }

        // パラメータの更新
        refCamera.orthographicSize = mainCamera.orthographicSize;
    }

    //------------------------------------------------------------------------------------------
    // 反射用カメラの初期化
    //------------------------------------------------------------------------------------------
    void InitRefCamera()
    {
        // オブジェクトの生成
        if (refCameraObject)
        {
            Destroy(refCameraObject);
        }
        refCameraObject = new GameObject("RefCamera");
        refCameraObject.transform.parent = transform;
        refCameraObject.transform.localPosition = Vector3.zero;
        refCamera = refCameraObject.AddComponent<UnityEngine.Camera>();

        // コンポーネントの複製
        refCamera.cullingMask = mainCamera.cullingMask;
        refCamera.orthographic = true;
        refCamera.orthographicSize = mainCamera.orthographicSize;
        refCamera.nearClipPlane = mainCamera.nearClipPlane;
        refCamera.farClipPlane = mainCamera.farClipPlane;
        refCamera.depth = mainCamera.depth;

        // レンダーターゲットの生成
        refCamera.targetTexture = new RenderTexture(refCamera.pixelWidth, refCamera.pixelHeight, 16);
        refCamera.targetTexture.filterMode = FilterMode.Point;

        // スクリーンテクスチャの割り当て
        Shader.SetGlobalTexture(globalTextureName, refCamera.targetTexture);
    }
}

