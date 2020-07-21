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
public class BubbleChargeController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // BubbleGeneratorのオブジェクト
    GameObject bubbleGeneratorObject;
    [SerializeField]
    // Bubbleの生成個数
    int num_bubble;

    BubbleGenerator bubbleG;
    bool atOnce = false;

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
        bubbleG = bubbleGeneratorObject.GetComponent<BubbleGenerator>();
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Player.NAME && !atOnce)
        {
            atOnce = true;
            // バブルを生成(複数個)
            bubbleG.BubbleCreate(this.transform.position, num_bubble, false);
            Destroy(this.gameObject);
            Vector2 effectSize = Vector2.one * 1.5f;
            EffectGenerator.BubbleBurstFX(new BubbleBurstFX.Param(Color.white, effectSize), transform.position, null);
        }
    }

}
