//==============================================================================================
/// File Name	: HSVShaderController.cs
/// Summary		: HSVShaderの操作用スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class HSVShaderController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private new Renderer renderer = null;  // レンダラー

    
    [Header("色相を固定するかどうか")]
    public bool useFixHue = false;
    [Header("固定する色相")]
    public Color fixHue = Color.red;
    [Header("色相の変移")]
    public float hueShift = 0;
    [Header("彩度の倍率")]
    public float saturationMultiply = 1;
    [Header("彩度の変移")]
    public float saturationShift = 0;
    [Header("明度の倍率")]
    public float brightnessMultiply = 1;
    [Header("彩度の変移")]
    public float brightnessShift = 0;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    public void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	public void Update()
    {
        var mat = renderer.material;
        mat.SetFloat("_UseFixHue", (useFixHue ? 1 : 0));
        mat.SetColor("_FixHue", fixHue);
        mat.SetFloat("_HueShift", hueShift);
        mat.SetFloat("_SaturationMultiply", saturationMultiply);
        mat.SetFloat("_SaturationShift", saturationShift);
        mat.SetFloat("_BrightnessMultiply", brightnessMultiply);
        mat.SetFloat("_BrightnessShift", brightnessShift);
    }
}
