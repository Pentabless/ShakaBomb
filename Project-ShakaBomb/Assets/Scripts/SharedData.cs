/*----------------------------------------------------------*/
/*--ファイル名：SharedData.cs-------------------------------*/
/*--概要：シーン間で共有したいものや共通の関数を入れている--*/
/*----------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharedData : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    public static readonly SharedData instance = new SharedData();
    //プレイ中のステージ番号
    public int play_stage_number;
    //クリア時の制限時間
    public float play_time_limit;
    //前シーンの背景の泡の飾りのリスト
    public List<BackGroundDecorationController> back_decoration_list = new List<BackGroundDecorationController>();
    //前シーンの前景の泡の飾りのリスト
    public List<BackGroundDecorationController> front_decoration_list = new List<BackGroundDecorationController>();
    //カメラの映る範囲
    public Vector3[] camera_range;

    //データの初期化をしたか
    public bool initialize_stage_data;


    public struct StageData
    {
        public bool clear;             //クリアしているか
        public bool can_play;          //プレイできるか
        public int purification_rate;  //浄化率
    }

    //ステージの情報
    public StageData[] stage_data;

    /*----------------------------------------------------*/
    /*--関数名：SetCanvasOption(public)-------------------*/
    /*--概要：シーンのCanvasの設定をする(泡の飾りのため)--*/
    /*--引数：設定をするCanvas(Canvas)--------------------*/
    /*--戻り値：なし--------------------------------------*/
    /*----------------------------------------------------*/
    public void SetCanvasOption(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvas.planeDistance = 50;
        canvas.sortingOrder = 5;
    }
    /*--終わり：SetCanvasOption--*/

    /*--------------------------------------------------*/
    /*--関数名：SetCanvasScalerOption(public)-----------*/
    /*--概要：シーンのCanvasの設定をする(ボタンのため)--*/
    /*--引数：設定をするCanvasScaler(CanvasScaler)------*/
    /*--戻り値：なし------------------------------------*/
    /*--------------------------------------------------*/
    public void SetCanvasScaleOption(CanvasScaler scaler)
    {
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }
    /*--終わり：SetCanvasScalerOption--*/

    /*--------------------------------------------------*/
    /*--関数名：GetCameraRange(public)------------------*/
    /*--概要：シーンのCameraが映す範囲を取得する--------*/
    /*--引数：シーンのカメラ(Camera)--------------------*/
    /*--戻り値：映す範囲(Vector3 [0]…左下　[1]…右上)--*/
    /*--------------------------------------------------*/
    public Vector3[] GetCameraRange(Camera camera)
    {
        camera_range = new Vector3[2];

        camera_range[0] = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        camera_range[1] = camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

        return camera_range;
    }
    /*--終わり：GetCameraRange--*/

    /*------------------------------------------------------------------------------------------*/
    /*--関数名：CreatePreviousSceneDecoration(public)-------------------------------------------*/
    /*--概要：保存してある飾りのリストの内容と同じ飾りを作る------------------------------------*/
    /*--引数：シーンの飾りジェネレーター(BackGroundDecorationGenerator)、カメラの位置(Vector2)--*/
    /*--戻り値：なし----------------------------------------------------------------------------*/
    /*------------------------------------------------------------------------------------------*/
    public void CreatePreviousSceneDecoration(BackGroundDecorationGenerator generator, Vector2 camera_position)
    {
        //背景と前景の2回
        for (int i = 0; i < 2; i++)
        {
            List<BackGroundDecorationController> decorations;

            //泡の飾りのリストをもらう
            switch (i)
            {
                case 0:
                    decorations = back_decoration_list;
                    //Debug.Log("飾りの数=" + decorations.Count);
                    break;

                case 1:
                    decorations = front_decoration_list;
                    //Debug.Log("飾りの数=" + decorations.Count);
                    break;
                default:
                    decorations = new List<BackGroundDecorationController>();
                    break;
            }

            //泡の飾りが一つでもあったら
            if (decorations.Count > 0)
            {
                //リストの大きさ分繰り返す
                for (int j = 0; j < decorations.Count; j++)
                {
                    //記憶している情報からオブジェクトを生成してもらう
                    generator.CreateDecoration(decorations[j].GetPosition() + new Vector3(camera_position.x, camera_position.y, 0.0f), decorations[j].GetScale(), decorations[j].GetColor(), decorations[j].GetLayer());
                }
            }
        }
    }
    /*--終わり：CreatePreviousSceneDecoration--*/

    /*--------------------------------------------*/
    /*--関数名：SetDecorationList(public)---------*/
    /*--概要：シーン内にある飾りをリストに入れる--*/
    /*--引数：なし--------------------------------*/
    /*--戻り値：なし------------------------------*/
    /*--------------------------------------------*/
    public void SetDecorationList(Vector2 camera_position)
    {
        //リストをリセットする
        back_decoration_list.Clear();
        front_decoration_list.Clear();

        Debug.Log(back_decoration_list.Count);
        Debug.Log(front_decoration_list.Count);

        //タグが「Decoration」のオブジェクトを全て取得する
        GameObject[] decoration_array = GameObject.FindGameObjectsWithTag("Decoration");

        for (int i = 0; i < decoration_array.Length; i++)
        {
            //情報を記録する前に座標を覚える
            Vector3 pos = decoration_array[i].transform.position;
            //カメラから見た座標に調整する
            decoration_array[i].transform.position -= new Vector3(camera_position.x, camera_position.y, 0.0f);
            //リストに入る前に必要な情報を記憶する
            decoration_array[i].GetComponent<BackGroundDecorationController>().RememberInformation();
            //調整前の座標に戻す
            decoration_array[i].transform.position = pos;

            //前景だったら
            if (decoration_array[i].GetComponent<SpriteRenderer>().sortingOrder >= 0)
            {
                //前景のリストに入れる
                front_decoration_list.Add(decoration_array[i].GetComponent<BackGroundDecorationController>());
            }
            //背景だったら
            else
            {
                //背景のリストに入れる
                back_decoration_list.Add(decoration_array[i].GetComponent<BackGroundDecorationController>());
            }
        }

        Debug.Log(back_decoration_list.Count);
        Debug.Log(front_decoration_list.Count);

    }
    /*--終わり：SetDecorationList--*/

    /*------------------------------------------*/
    /*--関数名：SetStageNameEnglish(public)-----*/
    /*--概要：ステージ番号を英語で文字列にする--*/
    /*--引数：ステージ番号(int)-----------------*/
    /*--戻り値：英語にした文字列(string)--------*/
    /*------------------------------------------*/
    public string SetStageNameEnglish(int number)
    {
        string str_number = "";

        switch (number % 10)
        {
            //「st」を付ける
            case 1:
                if (number != 11)
                {
                    str_number = number.ToString() + "st";
                }
                break;
            //「nd」を付ける
            case 2:
                if (number != 12)
                {
                    str_number = number.ToString() + "nd";
                }
                break;
            //「rd」を付ける
            case 3:
                if (number != 13)
                {
                    str_number = number.ToString() + "rd";
                }
                break;
        }
        //まだ何も設定されていなかったら
        if (str_number == "")
        {
            //「th」を付ける
            str_number = number.ToString() + "th";
        }

        return str_number + " Stage";
    }
    /*--終わり：SetStageNameEnglish--*/

    /*----------------------------------------*/
    /*--関数名：SetStageDataSize(public)------*/
    /*--概要：ステージデータのサイズを決める--*/
    /*--引数：ステージの扉の数(int)-----------*/
    /*--戻り値：なし--------------------------*/
    /*----------------------------------------*/
    public void SetStageDataSize(int num)
    {
        //初期化していなかったら
        if (initialize_stage_data == false)
        {
            stage_data = new StageData[num];
            //全て初期化
            for (int i = 0; i < num; i++)
            {
                stage_data[i].clear = false;
                stage_data[i].can_play = false;
                stage_data[i].purification_rate = 0;
            }
            //一番最初のステージだけプレイできるようにする
            stage_data[0].can_play = true;
            //初期化をしたことにする
            initialize_stage_data = true;
        }
    }
    /*--終わり：SetStageDataSize--*/

    /*--------------------------------------------*/
    /*--関数名：GetPercentRank(private)-----------*/
    /*--概要：パーセントからランクを決める--------*/
    /*--引数：パーセント(int)---------------------*/
    /*--戻り値：ランク(int)-----------------------*/
    /*--------------------------------------------*/
    public int GetPercentRank(int percent)
    {
        int rank = 0;
        //29%以下だったら
        if (percent <= 29)
        {
            rank = 0;
        }
        //69%以下だったら
        else if (percent <= 69)
        {
            rank = 1;
        }
        //99%以下だったら
        else if (percent <= 99)
        {
            rank = 2;
        }
        //100%だったら
        else
        {
            rank = 3;
        }

        return rank;
    }
    /*--終わり：GetPercentRank--*/

    /*------------------------------------------------------------------------------------*/
    /*--関数名：SetPurificationRate(public)-----------------------------------------------*/
    /*--概要：浄化率をデータに記録する(クリア状況と次のステージがプレイできるようにする)--*/
    /*--引数：浄化率(int)-----------------------------------------------------------------*/
    /*--戻り値：なし----------------------------------------------------------------------*/
    /*------------------------------------------------------------------------------------*/
    public void SetPurificationRate(int purification)
    {
        //クリアしたことにする
        stage_data[Data.stage_number-1].clear = true;
        //今回の浄化率が前回の浄化率より大きかったら
        if (purification > stage_data[Data.stage_number-1].purification_rate)
        {
            //浄化率を入れる
            stage_data[Data.stage_number-1].purification_rate = purification;
        }
        //クリアした所の評価が2以上だったら
        if (GetPercentRank(purification) >= 2)
        {
            //クリアした所が一番最後のステージでなかったら
            if (Data.stage_number-1 != (stage_data.Length - 1))
            {
                //次のステージをプレイできるようにする
                stage_data[Data.stage_number ].can_play = true;
            }
        }
    }
    /*--終わり：SetPurificationRate--*/

    /*--------------------------------------------*/
    /*--関数名：GetClear(public)------------------*/
    /*--概要：ステージがクリア状況のデータを渡す--*/
    /*--引数：ステージ番号(int)-------------------*/
    /*--戻り値：クリアしているか(bool)------------*/
    /*--------------------------------------------*/
    public bool GetClear(int stage_number)
    {
        return stage_data[stage_number].clear;
    }
    /*--終わり：GetClear--*/

    /*------------------------------------------------------*/
    /*--関数名：GetCanPlay(public)--------------------------*/
    /*--概要：ステージがプレイできるかどうかのデータを渡す--*/
    /*--引数：ステージ番号(int)-----------------------------*/
    /*--戻り値：プレイできるか(bool)------------------------*/
    /*------------------------------------------------------*/
    public bool GetCanPlay(int stage_number)
    {
        return stage_data[stage_number].can_play;
    }
    /*--終わり：GetCanPlay--*/

    /*------------------------------------------*/
    /*--関数名：GetPurification(public)---------*/
    /*--概要：ステージ毎の浄化率のデータを渡す--*/
    /*--引数：ステージ番号(int) ----------------*/
    /*--戻り値：浄化率(int)---------------------*/
    /*------------------------------------------*/
    public int GetPurification(int stage_number)
    {
        return stage_data[stage_number].purification_rate;
    }
    /*--終わり：GetCanPlay--*/

}
