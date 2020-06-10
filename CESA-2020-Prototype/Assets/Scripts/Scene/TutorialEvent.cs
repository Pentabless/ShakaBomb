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
public class TutorialEvent : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    EventObject eventObj;
    [SerializeField]
    GameObject movieScreen;
    [SerializeField]
    VideoPlayer video;
    [SerializeField]
    VideoClip clip;
    [SerializeField]
    float fadeTime = 1.0f;


    PauseManager pauseManager = null;

    bool playOn = false;

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
        if(video.isPlaying)
        {
            var inputA = Input.GetButtonDown(GamePad.BUTTON_A);
            if(inputA)
            {
                eventObj.EndEvent();
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        FadeManager.fadeColor = new Color(0, 0, 0, 0.75f);
        FadeManager.FadeOut(fadeTime);
        movieScreen.SetActive(true);
        video.clip = clip;
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
        FadeManager.FadeIn(fadeTime);
    }
}
