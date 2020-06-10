//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Linq;
//==============================================================================================
public class CheckPointController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    DirtController dirt;
    [SerializeField]
    SpriteRenderer towerDirt;

    bool cleaned = false;

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
        var rate = dirt.CleaningRate();

        ChangeAlpha(1.0f - 1.0f * rate);
        if (rate >= 1 && !cleaned)
        {
            Data.initialPlayerPos = this.transform.position;
            EffectGenerator.CleanFX(new CleanFX.Param(new Vector2(1, 4)), transform.position);
            cleaned = true;
        }
    }

    //------------------------------------------------------------------------------------------
    // aの値を変更
    //------------------------------------------------------------------------------------------
    private void ChangeAlpha(float alpha)
    {
        var color = towerDirt.color;
        towerDirt.color = new Vector4(color.r, color.g, color.b, alpha);
    }
}
