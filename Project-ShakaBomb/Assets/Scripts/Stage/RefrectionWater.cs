//==============================================================================================
/// File Name	: ReflectionWater.cs
/// Summary		: 反射する水面
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
[ExecuteInEditMode]
public class RefrectionWater : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private new Renderer renderer = null;           // レンダラー
    private UnityEngine.Camera mainCamera = null;   // メインカメラ
    [SerializeField]
    private Vector2 tiling = Vector2.one;           // タイリング
    [SerializeField]
    private float scrollSpeed = 0.5f;               // スクロール速度
    [Header("テクスチャに乗算する色")]
    [SerializeField]
    private Color filterColor = Color.white;        // テクスチャに乗算する色
    [Header("テクスチャにブレンドする色")]
    [SerializeField]
    private Color blendColor = Color.clear;         // テクスチャにブレンドする色
    [Header("彩度の倍率")]
    [SerializeField]
    private float saturationRate = 1.0f;            // 彩度の倍率
    [Header("明度の倍率")]
    [SerializeField]
    private float brightnessRate = 1.0f;            // 明度の倍率

    private float offset = 0.0f;                    // 揺らぎテクスチャのオフセット

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        mainCamera = UnityEngine.Camera.main;
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        offset += Time.deltaTime * scrollSpeed;
        float minWorldSpaceY = renderer.bounds.max.y;
        float minScreenSpaceY = mainCamera.WorldToScreenPoint(new Vector3(0, minWorldSpaceY, 0)).y;
        float topEdge = minScreenSpaceY / mainCamera.pixelHeight;

        renderer.sharedMaterial.SetColor("_Tint", filterColor);
        renderer.sharedMaterial.SetColor("_BlendColor", blendColor);
        renderer.sharedMaterial.SetFloat("_SaturationRate", saturationRate);
        renderer.sharedMaterial.SetFloat("_BrightnessRate", brightnessRate);
        renderer.sharedMaterial.SetFloat("_TopEdgePosition", topEdge);
        renderer.sharedMaterial.SetTextureScale("_Displacement", tiling);
        renderer.sharedMaterial.SetTextureOffset("_Displacement", new Vector2(offset, 0));
    }
}
