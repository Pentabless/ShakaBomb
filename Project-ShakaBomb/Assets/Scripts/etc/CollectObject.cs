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
public class CollectObject : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------

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

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
        GenerateBurstEffect();
        Data.colletcObject++;
    }


    private void GenerateBurstEffect()
    {
        EffectGenerator.BubbleBurstFX(
            new BubbleBurstFX.Param(GetComponent<SpriteRenderer>().color, transform.localScale),
            transform.localPosition,
            null);
    }
}
