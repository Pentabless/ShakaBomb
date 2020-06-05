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

    [SerializeField]
    [Header("色相を固定するかどうか")]
    private bool useFixHue = false;
    [SerializeField]
    [Header("固定する色相")]
    private Color fixHue = Color.red;
    [SerializeField]
    [Header("色相の変移")]
    private float hueShift = 0;
    [SerializeField]
    [Header("彩度の倍率")]
    private float saturationMultiply = 1;
    [SerializeField]
    [Header("彩度の変移")]
    private float saturationShift = 0;
    [SerializeField]
    [Header("明度の倍率")]
    private float brightnessMultiply = 1;
    [SerializeField]
    [Header("彩度の変移")]
    private float brightnessShift = 0;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        var mat = renderer.material;
        mat.SetFloat("UseFixHue", (useFixHue ? 1 : 0));
        mat.SetColor("FixHue", fixHue);
        mat.SetFloat("HueShift", hueShift);
        mat.SetFloat("SaturationMultiply", saturationMultiply);
        mat.SetFloat("SaturationShift", saturationShift);
        mat.SetFloat("BrightnessMultiply", brightnessMultiply);
        mat.SetFloat("BrightnessShift", brightnessShift);
    }
}
