/*--------------------------------------*/
/*--ファイル名：StageDataController.cs--*/
/*--概要：ステージの詳細を描画する------*/
/*--------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI

public class StageDataController : MonoBehaviour
{
    /*-----------*/
    /*--private--*/
    /*-----------*/
    //テキストオブジェクト
    GameObject text_obj;
    //ドアオブジェクト
    GameObject door_obj;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //オブジェクト取得
        text_obj = transform.GetChild(0).gameObject;
        GameObject parent_obj = transform.parent.gameObject;
        door_obj = parent_obj.transform.GetChild(0).gameObject;


        //自身のCanvasの設定をする
        SharedData.instance.SetCanvasOption(GetComponent<Canvas>());
        //自身のCanvasScalerの設定をする
        SharedData.instance.SetCanvasScaleOption(GetComponent<CanvasScaler>());

        //ステージデータの初期化をする(初期化していたら何もしない)
        SharedData.instance.SetStageDataSize(4);    //とりあえず4ステージ

        //親オブジェクト(Door)からステージ番号をもらう
        int stage_number = door_obj.GetComponent<DoorToStage>().GetStageNumber();
        //ステージ番号を使って浄化率をもらう
        int purification = SharedData.instance.GetPurification(stage_number);
        //テキスト内容を変える
        text_obj.GetComponent<Text>().text = purification.ToString() + "%";

        //プレイできるものは赤色にできないものは黒色に
        if(SharedData.instance.GetCanPlay(stage_number))
        {
            door_obj.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            door_obj.GetComponent<SpriteRenderer>().color = Color.black;
        }
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
        //座標移動(扉の下に移動する)
        text_obj.transform.position = new Vector3(door_obj.transform.position.x, door_obj.transform.position.y, text_obj.transform.position.z);
        text_obj.transform.position += new Vector3(0.0f, -2.0f, 0.0f);
    }
    /*--終わり：Update--*/

}
