//==============================================================================================
/// File Name	: AreaNameScript.cs
/// Summary		: エリア名を表示する機能
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
//==============================================================================================
public class AreaNameScript : MonoBehaviour
{
    public static string NAME = "AreaNameCanvas";
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // キャンバスグループ
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    // 背景色
    private Image backColor = null;
    [SerializeField]
    // 表示名
    private Text showText = null;
    [SerializeField]
    // アウトライン
    private Outline outline = null;

    // 表示中かどうか
    private bool showing = false;
    // フェード時間
    private float fadeTime = 1;
    // 表示時間
    private float showTime = 1;
    // タイマー
    private float timer = 0;

    [SerializeField]
    [Header("基本的にアタッチしなくてよい")]
    EventObject eventObj;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();   
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        if (!showing)
        {
            return;
        }

        timer += Time.deltaTime;
        // 出現
        if(timer <= fadeTime)
        {
            canvasGroup.alpha = Mathf.SmoothStep(0, 1, timer / fadeTime);
        }
        // 表示
        else if(timer <= fadeTime + showTime)
        {

        }
        // 消滅
        else if(timer <= fadeTime * 2 + showTime)
        {
            float offset = fadeTime + showTime;
            canvasGroup.alpha = Mathf.SmoothStep(1, 0, (timer - offset) / fadeTime);
        }
        else
        {
            canvasGroup.alpha = 0;
            showing = false;
            if(Data.stage_number == 0)
            {
                eventObj.StartEvent();
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // エリア名の表示
    //------------------------------------------------------------------------------------------
    public void ShowAreaName(string text, float fadeTime, float showTime)
    {
        showText.text = text;
        this.fadeTime = fadeTime;
        this.showTime = showTime;
        showing = true;
    }

    //------------------------------------------------------------------------------------------
    // テキストカラーの設定
    //------------------------------------------------------------------------------------------
    public void SetTextColor(Color color)
    {
        showText.color = color;
    }

    //------------------------------------------------------------------------------------------
    // アウトラインカラーの設定
    //------------------------------------------------------------------------------------------
    public void SetOutlineColor(Color color)
    {
        outline.effectColor = color;
    }

    //------------------------------------------------------------------------------------------
    // 背景色の設定
    //------------------------------------------------------------------------------------------
    public void SetBackColor(Color color)
    {
        backColor.color = color;
    }

    //------------------------------------------------------------------------------------------
    // 背景サイズの設定
    //------------------------------------------------------------------------------------------
    public void SetBackScale(Vector3 scale)
    {
        backColor.rectTransform.localScale = scale;
    }
}
