//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
using UnityEngine.SceneManagement;
using System.Linq;
//==============================================================================================
public class PrologueDirector : MonoBehaviour
{
    enum PrologueState
    {
        Start,
        Show,
        CrossFade,
        End,
    }
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    //[SerializeField]
    //List<FadeController> fadeControllers;
    //[SerializeField]
    //FadeController fadeScreen;
    //[SerializeField]
    //float fadeInterval;
    //[SerializeField]
    //float fadeValues;

    //float count;
    //int imageIndex = 0;
    //bool startFadeOut = false;

    [SerializeField]
    private List<Image> images = null;
    [SerializeField]
    private float fadeTime = 0;
    [SerializeField]
    private float showTime = 0;
    [SerializeField]
    private float crossFadeTime = 0;

    private PrologueState state = PrologueState.Start;
    private int index = 0;
    private float timer = 0;

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
        foreach(var image in images)
        {
            image.color = Color.clear;
        }
        images[0].color = Color.white;
        FadeManager.fadeColor = Color.white;
        FadeManager.FadeIn(fadeTime);
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case PrologueState.Start:
                if(timer >= fadeTime)
                {
                    timer = 0;
                    state = PrologueState.Show;
                }
                break;
            case PrologueState.Show:
                if(timer >= showTime)
                {
                    timer = 0;
                    if(index < images.Count - 1)
                    {
                        state = PrologueState.CrossFade;
                        index++;
                    }
                    else
                    {
                        state = PrologueState.End;
                        FadeManager.fadeColor = Color.black;
                        FadeManager.FadeOut("TutorialScene", fadeTime);
                        SoundFadeController.SetFadeOutSpeed(1.0f / fadeTime / 60 * SoundFadeController.GetAudioSource().volume * 1.5f);
                    }
                }
                break;
            case PrologueState.CrossFade:
                if (timer >= showTime)
                {
                    images[index].color = Color.white;
                    timer = 0;
                    state = PrologueState.Show;
                }
                else
                {
                    float alpha = Mathf.SmoothStep(0, 1, timer / crossFadeTime);
                    images[index].color = new Color(1, 1, 1, alpha);
                }
                break;
            case PrologueState.End:
                break;
            default:
                Debug.Log("PrologeState:error");
                break;
        }

        //count += Time.deltaTime;
        //var i = imageIndex;
        //i++;
        //if(count >= fadeInterval && fadeControllers.Count() >= i)
        //{
        //    fadeControllers[imageIndex].SetFadeValue(Common.Decimal.ZERO);
        //    fadeControllers[imageIndex].fade_type = false;

        //    if (imageIndex == 3)
        //    {
        //        Debug.Log("yes");
        //        if (!startFadeOut)
        //        {
        //            fadeScreen.SetFadeType(true);
        //            fadeScreen.SetFadeValue(0.0f);
        //            SoundFadeController.SetFadeOutSpeed(0.01f);
        //        }
        //        startFadeOut = true;

        //        if (fadeScreen.GetFadeValue() == 1.0f)
        //        {
        //            SceneManager.LoadScene("TutorialScene");
        //        }
        //    }

        //    if (fadeControllers.Count() > i)
        //    {
        //        imageIndex++;
        //        fadeControllers[imageIndex].fade_type = true;
        //    }

        //    count = Common.Decimal.ZERO;
        //}
    }
}
