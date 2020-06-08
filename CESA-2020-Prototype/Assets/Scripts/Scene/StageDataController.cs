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
    /*----------*/
    /*--public--*/
    /*----------*/
    //浄化率のテキストオブジェクト
    public GameObject PurificationRateTextPrefab;
    /*-----------*/
    /*--private--*/
    /*-----------*/
    //ドアオブジェクト
    private GameObject[] door_obj;
    //テキストオブジェクト
    private GameObject[] text_obj;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //自身のCanvasの設定をする
        SharedData.instance.SetCanvasOption(GetComponent<Canvas>());
        //自身のCanvasScalerの設定をする
        SharedData.instance.SetCanvasScaleOption(GetComponent<CanvasScaler>());

        //シーン内にあるドアオブジェクトを全部探す
        door_obj = GameObject.FindGameObjectsWithTag("StageDoor");
        //ステージデータの初期化をする(初期化していたら何もしない)
        SharedData.instance.SetStageDataSize(door_obj.Length);    //ドアオブジェクトの数だけ

        //テキストオブジェクトのサイズを決める
        text_obj = new GameObject[door_obj.Length];

        Debug.Log(door_obj.Length);

        //ドアオブジェクトの数だけテキストオブジェクトを作る
        for (int i = 0; i < door_obj.Length; i++)
        {
            //プレファブを元にテキストオブジェクトを作る
            GameObject text = Instantiate(PurificationRateTextPrefab) as GameObject;
            //ドアオブジェクトからステージ番号をもらう
            int stage_number = door_obj[i].GetComponent<DoorToStage>().GetStageNumber();
            //ステージ番号を使って浄化率をもらう
            int purification = SharedData.instance.GetPurification(stage_number);
            //テキスト内容を変える
            text.GetComponent<Text>().text = purification.ToString() + "%";
            //テキストオブジェクトを覚える
            text_obj[i] = text;
            //作ったテキストオブジェクトを子オブジェクトに登録する
            text.transform.parent = transform;
            //拡大率
            text_obj[i].GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        //ドアの色を変える
        for (int i = 0; i < door_obj.Length; i++)
        {
            //プレイできるものは赤色にできないものは黒色に
            if (SharedData.instance.GetCanPlay(door_obj[i].GetComponent<DoorToStage>().GetStageNumber()))
            {
                door_obj[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                door_obj[i].GetComponent<SpriteRenderer>().color = Color.black;
            }
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
        //移動するドアのために毎回座標更新する
        for (int i = 0; i < door_obj.Length; i++)
        {
            text_obj[i].GetComponent<RectTransform>().position = new Vector3(door_obj[i].transform.position.x, door_obj[i].transform.position.y, text_obj[i].GetComponent<RectTransform>().position.z);
            text_obj[i].GetComponent<RectTransform>().position += new Vector3(0.0f, -2.0f, 0.0f);
        }
    }
    /*--終わり：Update--*/

}
