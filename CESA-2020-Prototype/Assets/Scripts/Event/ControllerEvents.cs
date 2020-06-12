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
public class ControllerEvents : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    List<Image> circle;
    [SerializeField]
    float blinkTime;
    IEnumerator routine;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        circle.ForEach(x => x.enabled = false);
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
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        circle.ForEach(x =>
        {
            x.enabled = true;
        });
        routine = OnBlink();
        StartCoroutine(routine);
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        circle.ForEach(x =>
        {
            x.enabled = false;
        });
        StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator OnBlink()
    {
        while (true)
        {
            circle.ForEach(x =>
            {
                if (x.gameObject.activeInHierarchy)
                {
                    x.gameObject.SetActive(false);
                }
                else
                {
                    x.gameObject.SetActive(true);
                }
            });

            yield return new WaitForSeconds(blinkTime);
        }
    }
}
