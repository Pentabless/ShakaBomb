//==============================================================================================
/// File Name	: StageSelectEvent.cs
/// Summary		: 
//==============================================================================================
using System.Collections;
using UnityEngine;
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
    [SerializeField, TextArea(15,3)]
    string text;
    [SerializeField]
    float fadeTime = 1.0f;
    [SerializeField]
    float interval = 3.0f;

    PauseManager pauseManager = null;
    EventObject eventObj;
    IEnumerator routine;
    float count;
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

            if (Input.GetButtonDown(ConstGamePad.BUTTON_A) && count >= interval)
            {
                count = 0.0f;
                eventObj.EndEvent();
            }
        }
        else
        {
            if (routine != null && screen.alpha <= 0.0f)
            {
                StopCoroutine(routine);
                routine = null;
                screen.alpha = 0.0f;
                screen.gameObject.SetActive(false);
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        if (Data.stage_number != 0)
            return;

        playOn = true;
        routine = Fade();
        canvasText.text = text;
        pauseManager.Pause(fadeTime);
        StartCoroutine(routine);
        pauseManager.SetFilterColor(Color.clear);
        GameObject.Find(ConstPlayer.NAME).GetComponentInChildren<PlayerAnimator>().StopAnimation();
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
        GameObject.Find(ConstPlayer.NAME).GetComponentInChildren<PlayerAnimator>().ResumeAnimation();
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
        var player = GameObject.Find(ConstPlayer.NAME).GetComponent<PlayerController>();
        player.EnableControl(false);
        yield return new WaitForSeconds(0.2f);
        player.EnableControl(true);
    }
}
