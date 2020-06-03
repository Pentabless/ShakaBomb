/*----------------------------------------------------------------*/
/*--ファイル名：StageCreateController.cs--------------------------*/
/*--概要：ステージセレクトシーンでもらった番号からステージを作る--*/
/*----------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreateController : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    public GameObject[] stage_prefab;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //ステージセレクトシーンからもらった番号をもとにステージを作る
        GameObject stage = Instantiate(stage_prefab[Data.stage_number])as GameObject;
        //座標を設定する(原点)
        stage.transform.position = new Vector3(0.0f, 0.0f, 0.0f);


    }
    /*--終わり：Start--*/

    /*------------------*/
    /*--関数名：Update--*/
    /*--概要：更新------*/
    /*--引数：なし------*/
    /*--戻り値：なし----*/
    /*------------------*/
    void Update()
    {
        
    }
    /*--終わり：Update--*/
}
