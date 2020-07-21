//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class BubbleCannon : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    public enum Status
    {
        None,
        Exist,
        Dead,
    }
    public Status currentStatus = Status.None;

    int deadCount = 0;
    float lerpScale = 0;

    [SerializeField]
    int restorationCount = 240;

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
        currentStatus = Status.Exist;
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (deadCount > 0)
        {
            deadCount--;
        }
        else if (deadCount <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void HitToPlayer()
    {
        currentStatus = Status.Dead;
        deadCount = restorationCount;
        Vector2 effectSize = Vector2.one * 1.5f;
        EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(Color.white, effectSize),
                transform.position,
                null);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
