//==============================================================================================
/// File Name	: Blink.cs
/// Summary		: Image、Textを点滅させる
//==============================================================================================
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using System;
//using Common;
using UnityEngine.UI;
//==============================================================================================
public class Blink : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    private enum ObjType
    {
        TEXT,
        IMAGE
    }



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private float speed = 0.0f;
    private Text text = null;
    private Image image = null;
    private float time = 0.0f;
    private ObjType thisObjType = ObjType.TEXT;



	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        //アタッチしてるオブジェクトを判別
        if (this.gameObject.GetComponent<Image>())
        {
            thisObjType = ObjType.IMAGE;
            image = this.gameObject.GetComponent<Image>();
        }
        else if (this.gameObject.GetComponent<Text>())
        {
            thisObjType = ObjType.TEXT;
            text = this.gameObject.GetComponent<Text>();
        }
    }



	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        //オブジェクトのAlpha値を更新
        if (thisObjType == ObjType.IMAGE)
        {
            image.color = GetAlphaColor(image.color);
        }
        else if (thisObjType == ObjType.TEXT)
        {
            text.color = GetAlphaColor(text.color);
        }
    }



    //------------------------------------------------------------------------------------------
    // GetAlphaColor（Alpha値を更新してColorを返す）
    //------------------------------------------------------------------------------------------
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;

        return color;
    }
}
