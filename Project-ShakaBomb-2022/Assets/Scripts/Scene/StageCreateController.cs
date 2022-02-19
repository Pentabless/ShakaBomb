﻿//==============================================================================================
/// File Name	: StageCreateController.cs
/// Summary		: ステージ生成を行うクラス
//==============================================================================================
using UnityEngine;
//==============================================================================================
public class StageCreateController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    public GameObject[] stagePrefab = null;



    //------------------------------------------------------------------------------------------
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        // ステージセレクトシーンからもらった番号をもとにステージを作る
        GameObject stage = Instantiate(stagePrefab[Data.stage_number-1])as GameObject;
        // 座標を設定する(原点)
        stage.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
