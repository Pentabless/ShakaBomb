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
//==============================================================================================
public class StageSelectEvent : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    CanvasGroup screen;
    [SerializeField]
    Text canvasText;
    [SerializeField]
    string text;
    [SerializeField]
    float fadeTime = 1.0f;

    PauseManager pauseManager = null;
    EventObject eventObj;
    IEnumerator routine;
    bool playOn = false;

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
            if (routine != null && screen.alpha >= 1.0f)
            {
                StopCoroutine(routine);
                routine = null;
                screen.alpha = 1.0f;
            }
        }
        else
        {
            if (routine != null && screen.alpha <= 0.0f)
            {
                StopCoroutine(routine);
                routine = null;
                screen.alpha = 0.0f;
            }
        }

        if(Input.GetButtonDown(GamePad.BUTTON_A))
        {
            eventObj.EndEvent();
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        //FadeManager.fadeColor = new Color(0, 0, 0, 0.75f);
        //FadeManager.FadeOut(fadeTime);
        playOn = true;
        routine = Fade();
        canvasText.text = text;
        pauseManager.Pause(fadeTime);
        StartCoroutine(routine);
        pauseManager.SetFilterColor(Color.clear);
        GameObject.Find(Player.NAME).GetComponentInChildren<PlayerAnimator>().StopAnimation();
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        playOn = false;
        pauseManager.Resume();
        routine = Fade();
        StartCoroutine(routine);
        FadeManager.FadeIn(fadeTime);
        GameObject.Find(Player.NAME).GetComponentInChildren<PlayerAnimator>().ResumeAnimation();
        StartCoroutine(ControlDelay());
    }

    private IEnumerator Fade()
    {
        while (true)
        {
            if (playOn)
            {
                screen.alpha += 0.1f;
            }
            else
            {
                screen.alpha -= 0.1f;
            }

            yield return new WaitForSeconds(fadeTime/100);
        }
    }

    private IEnumerator ControlDelay()
    {
        var player = GameObject.Find(Player.NAME).GetComponent<PlayerController>();
        player.EnableControl(false);
        yield return new WaitForSeconds(0.2f);
        player.EnableControl(true);
    }
}
