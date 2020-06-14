//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using UnityEngine.SceneManagement;
using System.Linq;
//==============================================================================================
public class PrologueDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    List<FadeController> fadeControllers;
    [SerializeField]
    FadeController fadeScreen;
    [SerializeField]
    float fadeInterval;
    [SerializeField]
    float fadeValues;

    float count;
    int imageIndex = 0;
    bool startFadeOut = false;

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
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        count += Time.deltaTime;
        var i = imageIndex;
        i++;
        if(count >= fadeInterval && fadeControllers.Count() >= i)
        {
            fadeControllers[imageIndex].SetFadeValue(Common.Decimal.ZERO);
            fadeControllers[imageIndex].fade_type = false;

            if (imageIndex == 3)
            {
                Debug.Log("yes");
                if (!startFadeOut)
                {
                    fadeScreen.SetFadeType(true);
                    fadeScreen.SetFadeValue(0.0f);
                    SoundFadeController.SetFadeOutSpeed(0.01f);
                }
                startFadeOut = true;

                if (fadeScreen.GetFadeValue() == 1.0f)
                {
                    SceneManager.LoadScene("TutorialScene");
                }
            }

            if (fadeControllers.Count() > i)
            {
                imageIndex++;
                fadeControllers[imageIndex].fade_type = true;
            }

            count = Common.Decimal.ZERO;
        }
    }
}
