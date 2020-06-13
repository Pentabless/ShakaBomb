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
    [Header("1回目の点滅")]
    List<GameObject> first;
    [SerializeField]
    [Header("2回目の点滅")]
    List<GameObject> seconds;
    [SerializeField]
    [Header("カウントで変えるか")]
    bool onChange = false;

    [SerializeField]
    float blinkTime;

    [SerializeField]
    float changeCount;

    IEnumerator routine;
    float count;
    bool change = false;
    bool start = false;

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
        if (!start || !onChange)
            return;

        if(change)
        {
            count -= Time.deltaTime;
        }
        else
        {
            count += Time.deltaTime;
        }

        if (count >= changeCount)
        {
            if (!change)
                first.ForEach(x => x.SetActive(false));
            change = true;
        }

        if(count <= Common.Decimal.ZERO)
        {
            if (change)
                seconds.ForEach(x => x.SetActive(false));
            change = false;
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        start = true;
        if(onChange)
            seconds.ForEach(x => x.SetActive(false));
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
        start = false;
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
            if(!onChange)
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
            }
            else if(change)
            {
                seconds.ForEach(x => 
                {
                    if (x.activeInHierarchy)
                    {
                        x.SetActive(false);
                    }
                    else
                    {
                        x.SetActive(true);
                    }
                });
            }
            else
            {
                first.ForEach(x =>
                {
                    if (x.activeInHierarchy)
                    {
                        x.SetActive(false);
                    }
                    else
                    {
                        x.SetActive(true);
                    }
                });
            }

            yield return new WaitForSeconds(blinkTime);
        }
    }
}
