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
    float fadeTime = 1.0f;
    [SerializeField]
    float changeTime;

    PauseManager pauseManager = null;
    EventObject eventObj;
    IEnumerator routine;

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
            if (routine != null && screen.alpha >= 1.0f)
            {
                StopCoroutine(routine);
                routine = null;
                screen.alpha = 1.0f;
            }
        }
        else
        {
            if (routine != null&&screen.alpha <= 0.0f)
            {
                StopCoroutine(routine);
                routine = null;
                screen.alpha = 0.0f;
            }
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
        routine = Fade();
        StartCoroutine(routine);
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
        routine = Fade();
        StartCoroutine(routine);
        FadeManager.FadeIn(fadeTime);
    }

    private IEnumerator Fade()
    {
        while (true)
        {
            if(playOn)
            {
                screen.alpha +=  0.1f;
            }
            else
            {
                screen.alpha -= 0.1f;
            }

            yield return new WaitForSeconds(fadeTime);
        }
    }

}
