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
using UnityEngine.UI;
using System.Linq;
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
    [SerializeField]
    List<GameObject> childe;
    [SerializeField]
    List<FadeController> fade;
    [SerializeField]
    string text;

    PauseManager pauseManager = null;

    bool playOn = false;
    float count = 0.0f;

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
            Debug.Log(count);
            count += Time.deltaTime;
            var inputA = Input.GetButtonDown(GamePad.BUTTON_A);
            if(count >= Stage.NotInputCount &&inputA)
            {
                video.Stop();
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
        var temp = childe.Where(x => x.GetComponent<Text>() != null).First().GetComponent<Text>();
        temp.text = text;
        fade.ForEach(x => x.fade_type = true);
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
        count = 0.0f;
        fade.ForEach(x =>
        {
            x.Alpha = 0.0f;
            x.SetFadeValue(0.0f);
        });
        childe.ForEach(x =>
        {
            var image = x.GetComponent<Image>();
            var rawImage = x.GetComponent<RawImage>();
            var text = x.GetComponent<Text>();

            if(image)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
            }
            else if(rawImage)
            {
                rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0.0f);
            }
            else
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            }
        });
        movieScreen.SetActive(false);
        FadeManager.FadeIn(fadeTime);
    }
}
