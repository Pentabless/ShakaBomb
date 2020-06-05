//==============================================================================================
/// File Name	: DirtController.cs
/// Summary		: 汚れ制御用スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class DirtController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    HSVShaderController hsvController = null;

    [SerializeField]
    // 光の点滅加減
    private float brightnessMax = 1.6f;
    [SerializeField]
    // 光の点滅加減
    private float brightnessMin = 0.7f;

    [SerializeField]
    // 光の点滅速度
    private float blinkSpeed = 2.0f;

    private float timer = 0;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        hsvController = GetComponent<HSVShaderController>();
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
        timer += Time.deltaTime;
        hsvController.brightnessMultiply = Mathf.Lerp(brightnessMin, brightnessMax, Mathf.Sin(timer * Mathf.PI * 2 * blinkSpeed) * 0.5f + 0.5f);
    }
}
