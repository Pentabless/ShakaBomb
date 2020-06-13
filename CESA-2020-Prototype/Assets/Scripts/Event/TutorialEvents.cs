//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using UnityEngine.UI;
using UnityEngine.Video;
//==============================================================================================
public class TutorialEvents: MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    CanvasGroup screen;
    [SerializeField]
    VideoPlayer video;
    [SerializeField]
    VideoClip clip;
    [SerializeField]
    Text canvasText;
    [SerializeField]
    string text;
    [SerializeField]
    float fadeTime = 1.0f;

    PauseManager pauseManager = null;
    EventObject eventObj;

    bool playOn = false;
    float count = 0.0f;

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

        eventObj = GetComponent<EventObject>();

        screen.alpha = 0.0f;
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (playOn)
        {
            count += Time.deltaTime;
            if (screen.alpha <= 1.0f)
            {
                screen.alpha += fadeTime / 100;
            }
        }
        else
        {
            count = 0.0f;
        }

        if (video.isPlaying)
        {
            var inputA = Input.GetButtonDown(GamePad.BUTTON_A);
            if (count >= Stage.NotInputCount && inputA)
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
        playOn = true;
        video.clip = clip;
        video.Play();
        canvasText.text = text;
        pauseManager.SetFilterColor(Color.clear);
        pauseManager.Pause(fadeTime);
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        pauseManager.Resume();
        playOn = false;
        screen.alpha = 0.0f;
        FadeManager.FadeIn(fadeTime);
    }

}
