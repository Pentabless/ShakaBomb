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
    GameObject movieScreen;
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
        else if(playOn)
        {
            Debug.Log("yes");
            eventObj.EndEvent();
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        movieScreen.SetActive(true);
        video.Play();
        pauseManager.SetFilterColor(Color.clear);
        pauseManager.Pause(fadeTime);
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        pauseManager.Resume();
        movieScreen.SetActive(false);
        FadeManager.FadeOut(fadeTime);
    }

}
