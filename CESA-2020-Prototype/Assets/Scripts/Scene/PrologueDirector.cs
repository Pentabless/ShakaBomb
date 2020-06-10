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
    float fadeInterval;
    [SerializeField]
    float fadeValues;

    float count;

    int imageIndex = 0;

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

            if (fadeControllers.Count() > i)
            {
                imageIndex++;
                fadeControllers[imageIndex].fade_type = true;
            }

            count = Common.Decimal.ZERO;
        }
        else if (fadeControllers.Count() < i)
        {
            SceneManager.LoadScene("PlayScene");
        }
    }
}
