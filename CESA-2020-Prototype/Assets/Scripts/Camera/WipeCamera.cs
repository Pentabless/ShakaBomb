//==============================================================================================
/// File Name	: WipeCamera.cs
/// Summary		: ワイプエフェクト制御スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class WipeCamera : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // ワイプエフェクト
    private Material filter = null;
    // カメラ
    private new UnityEngine.Camera camera = null;
    // ターゲット
    private Vector3 targetPos = Vector3.zero;
    // タイマー
    private float timer = 0;

    // フェードインしているかどうか
    private bool fadeIn = false;
    // フェードアウトしているかどうか
    private bool fadeOut = false;
    // フェード時間
    private float fadeTime = 1.5f;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        camera = GetComponent<UnityEngine.Camera>();
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

        var screenPos = RectTransformUtility.WorldToScreenPoint(camera, targetPos);
        screenPos /= new Vector2(camera.scaledPixelWidth, camera.scaledPixelHeight);

        if (fadeIn || fadeOut)
        {
            timer += Time.deltaTime;
            float radius = timer * 2.0f / fadeTime;

            filter.SetVector("_Center", (Vector4)screenPos);
            filter.SetFloat("_Radius", (fadeIn ? radius : 2.0f - radius));

            if (timer > fadeTime)
            {
                fadeIn = false;
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // OnRenderImage
    //------------------------------------------------------------------------------------------
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, filter);
    }

    //------------------------------------------------------------------------------------------
    // ワイプエフェクトでフェードインする
    //------------------------------------------------------------------------------------------
    public void StartFadeIn(Vector3 pos, float time)
    {
        targetPos = pos;
        fadeTime = time;
        timer = 0;
        fadeIn = true;
        fadeOut = false;
    }

    //------------------------------------------------------------------------------------------
    // ワイプエフェクトでフェードアウトする
    //------------------------------------------------------------------------------------------
    public void StartFadeOut(Vector3 pos, float time)
    {
        targetPos = pos;
        fadeTime = time;
        timer = 0;
        fadeIn = false;
        fadeOut = true;
    }

}
