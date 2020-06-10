//==============================================================================================
/// File Name	: DirtController.cs
/// Summary		: 汚れ制御用スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class DirtController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // オーディオタイプ
    enum AudioType
    {
        Cleaning,
        End
    }
    // スプライトレンダラー
    SpriteRenderer spriteRenderer = null;
    // HSVシェーダー
    HSVShaderController hsvController = null;
    //　ダートマネージャ
    DirtManager dirtManager = null;

    [SerializeField]
    [Header("量と掃除回数にSacleを掛けるかどうか")]
    bool useScale = true;

    [SerializeField]
    [Header("汚れの量")]
    int amount = 10;

    [SerializeField]
    [Header("必要な掃除回数")]
    int sweepLevel = 4;

    // 掃除回数
    int sweepCount = 0;
    // 掃除されているかどうか
    bool beingSwept = false;
    // 掃除が完了したかどうか
    bool wasCleaned = false;

    // 初期の不透明度
    float defaultAlpha = 1;
    // 現在の不透明度
    float currentAlpha = 1;
    // 目的の不透明度
    float targetAlpha = 1;
    [SerializeField]
    // 透明度合い
    float alphaRate = 0.7f;


    [SerializeField]
    // 光の点滅加減
    private float brightnessMax = 1.6f;
    [SerializeField]
    // 光の点滅加減
    private float brightnessMin = 0.7f;

    [SerializeField]
    // 光の点滅速度
    private float blinkSpeed = 2.0f;

    private float timer = 0;

    [SerializeField]
    List<AudioClip> audios;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hsvController = GetComponent<HSVShaderController>();
        currentAlpha = targetAlpha = defaultAlpha = GetComponent<SpriteRenderer>().color.a;
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        if (useScale)
        {
            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            amount = Mathf.FloorToInt(amount * scale);
            sweepLevel = Mathf.FloorToInt(sweepLevel * scale);
        }
        dirtManager = GameObject.Find(Dirt.MANAGER).GetComponent<DirtManager>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        if (beingSwept && !wasCleaned)
        {
            sweepCount++;

            float a = Mathf.Max(1 - (sweepCount * 1.0f / sweepLevel), 0.0f);
            a = a * alphaRate + (1 - alphaRate);
            targetAlpha = defaultAlpha * a;

            if (sweepCount >= sweepLevel)
            {
                targetAlpha = 0;
                dirtManager.DirtCleaned(amount);
                wasCleaned = true;

                var collider = GetComponent<BoxCollider2D>();
                var size = collider.size * transform.lossyScale;
                var offset = collider.offset * transform.lossyScale;
                EffectGenerator.CleanFX(new CleanFX.Param(size), transform.position+(Vector3)offset);

                SoundPlayer.Play(audios[(int)AudioType.End],0.5f);
            }

            SoundPlayer.Play(audios[(int)AudioType.Cleaning]);
            beingSwept = false;
        }
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, 0.2f);
        var color = spriteRenderer.color;
        color.a = currentAlpha;
        spriteRenderer.color = color;


        timer += Time.deltaTime;
        hsvController.brightnessMultiply = Mathf.Lerp(brightnessMin, brightnessMax, Mathf.Sin(timer * Mathf.PI * 2 * blinkSpeed) * 0.5f + 0.5f);
    }

    //------------------------------------------------------------------------------------------
    // 掃除処理
    //------------------------------------------------------------------------------------------
    public void Swept()
    {
        beingSwept = true;
    }

    //------------------------------------------------------------------------------------------
    // 掃除割合
    //------------------------------------------------------------------------------------------
    public float CleaningRate()
    {
        return sweepCount / sweepLevel;
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerEnter2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Swept();
        }
    }
}

