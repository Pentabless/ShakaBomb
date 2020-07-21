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
    public enum PostEffects
    {
        Wipe,
        Alert,
    }
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // ワイプエフェクト
    private Material wipeFilter = null;
    [SerializeField]
    // アラートエフェクト
    private Material alertFilter = null;
    // カメラ
    private new UnityEngine.Camera camera = null;
    // ターゲット
    private Vector3 targetPos = Vector3.zero;
    // タイマー
    private float timer = 0;

    // 現在のエフェクト
    public PostEffects currentEffect { private set; get; } = PostEffects.Wipe;

    // フェードインしているかどうか
    private bool fadeIn = false;
    // フェードアウトしているかどうか
    private bool fadeOut = false;
    // フェード時間
    private float fadeTime = 1.5f;

    // アラート中かどうか
    private bool alert = false;
    // アラートの不透明度
    private float alertAlpha = 0;

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

            wipeFilter.SetVector("_Center", (Vector4)screenPos);
            wipeFilter.SetFloat("_Radius", (fadeIn ? radius : 2.0f - radius));

            if (timer > fadeTime)
            {
                fadeIn = false;
            }
        }

        if (alert)
        {
            timer += Time.deltaTime;
            alertAlpha = Mathf.Abs(Mathf.Sin(Mathf.PI * timer));

            alertFilter.SetFloat("_Transparency", alertAlpha);
        }
        else
        {
            alertAlpha = Mathf.Lerp(alertAlpha, 0, 0.2f);
            alertFilter.SetFloat("_Transparency", alertAlpha);
        }
    }

    //------------------------------------------------------------------------------------------
    // OnRenderImage
    //------------------------------------------------------------------------------------------
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (currentEffect == PostEffects.Wipe)
        {
            Graphics.Blit(src, dest, wipeFilter);
        }
        else if(currentEffect == PostEffects.Alert)
        {
            Graphics.Blit(src, dest, alertFilter);
        }
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
        alert = false;
        currentEffect = PostEffects.Wipe;
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
        alert = false;
        currentEffect = PostEffects.Wipe;
    }

    //------------------------------------------------------------------------------------------
    // アラートエフェクトを開始する
    //------------------------------------------------------------------------------------------
    public void StartAlert()
    {
        timer = 0;
        fadeIn = false;
        fadeOut = false;
        alert = true;
        currentEffect = PostEffects.Alert;
    }

    //------------------------------------------------------------------------------------------
    // アラートエフェクトを終了する
    //------------------------------------------------------------------------------------------
    public void EndAlert()
    {
        alert = false;
        currentEffect = PostEffects.Alert;
    }
}
