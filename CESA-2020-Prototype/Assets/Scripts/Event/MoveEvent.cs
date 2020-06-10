//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using UnityEngine.Video;
//==============================================================================================
public class MoveEvent : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // エンドイベント
    EventObject eventObj;
    [SerializeField]
    // 時間を止める
    private bool pause = true;
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

    private VideoPlayer video;

    private bool playOn = false;
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
        video = GetComponent<VideoPlayer>();
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
        if(video.isPlaying)
        {
            playOn = true;
        }
        else if(!playOn)
        {
            eventObj.EndEvent();
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        pauseManager.SetFilterColor(Color.clear);
        pauseManager.Pause(fadeTime);

        video.Play();

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
