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
    private float scrollSpeed = 0.5f;               // スクロール速度

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

        renderer.sharedMaterial.SetFloat("_TopEdgePosition", topEdge);
        renderer.sharedMaterial.SetTextureOffset("_Displacement", new Vector2(offset, 0));
    }
}
