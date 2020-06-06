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
            Debug.Log(imageIndex);
            fadeControllers[imageIndex].fade_type = false;

            if (fadeControllers.Count() > i)
            {
                imageIndex++;
                fadeControllers[imageIndex].SetFadeValue(0.1f);
                fadeControllers[imageIndex].fade_type = true;
            }

            count = Common.Decimal.ZERO;
        }
        else
        {
            // ToDo::次シーンに飛ばす
        }
    }
}
