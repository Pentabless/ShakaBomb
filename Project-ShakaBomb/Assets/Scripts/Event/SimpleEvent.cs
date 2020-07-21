//==============================================================================================
/// File Name	: SimpleEvent.cs
/// Summary		: 基本イベント
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class SimpleEvent : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // 時間を止める
    private bool pause = false;
    //[SerializeField]
    // UIを消す
    private bool hideUI = false;
    [SerializeField]
    // 画面を暗くする
    private bool backFade = false;

    [SerializeField]
    // フェードにかかる時間
    private float fadeTime = 1.0f;

    private PauseManager pauseManager = null;

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
        var go = GameObject.Find(PauseManager.NAME);
        if (!go)
        {
            Debug.Log("PauseManagerをシーンに追加してください");
        }
        pauseManager = go.GetComponent<PauseManager>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        if (pause)
        {
            pauseManager.SetFilterColor(Color.clear);
            pauseManager.Pause(fadeTime);
        }

        if (hideUI)
        {

        }

        if (backFade)
        {
            FadeManager.fadeColor = new Color(0, 0, 0, 0.75f);
            FadeManager.FadeOut(fadeTime);
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        if (pause)
        {
            pauseManager.Resume();
        }

        if (hideUI)
        {

        }

        if (backFade)
        {
            FadeManager.FadeIn(fadeTime);
        }
    }
}
