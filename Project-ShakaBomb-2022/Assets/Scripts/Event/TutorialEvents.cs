//==============================================================================================
/// File Name	: TutorialEvents.cs
/// Summary		: 
//==============================================================================================
using System.Collections;
using UnityEngine;
using Common;
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
            var inputA = Input.GetButtonDown(ConstGamePad.BUTTON_A);
            if (count >= ConstStage.NOT_INPUT_COUNT && inputA)
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
        playOn = true;
        FadeManager.fadeColor = new Color(0, 0, 0, 0.75f);
        FadeManager.FadeOut(0.7f);
        routine = Fade();
        StartCoroutine(routine);
        video.clip = clip;
        video.Play();
        pauseManager.SetFilterColor(Color.clear);
        pauseManager.Pause(fadeTime);
        GameObject.Find(ConstPlayer.NAME).GetComponentInChildren<PlayerAnimator>().StopAnimation();
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        pauseManager.Resume();
        playOn = false;
        FadeManager.FadeIn(0.5f);
        GameObject.Find(ConstPlayer.NAME).GetComponentInChildren<PlayerAnimator>().ResumeAnimation();
        routine = Fade();
        StartCoroutine(routine);
        StartCoroutine(ControlDelay());
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

            yield return null;
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
