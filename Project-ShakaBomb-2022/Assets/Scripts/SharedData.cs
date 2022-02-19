//==============================================================================================
/// File Name	: SharedData.cs（修正予定）
/// Summary		: 定数リスト
//==============================================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
public class SharedData : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    public static readonly SharedData instance = new SharedData();
    // プレイ中のステージ番号
    public int playStageNumber;
    // クリア時の制限時間
    public float playTimeLimit;
    // 前シーンの背景の泡の飾りのリスト
    public List<BackGroundDecorationController> backDecorationList = new List<BackGroundDecorationController>();
    // 前シーンの前景の泡の飾りのリスト
    public List<BackGroundDecorationController> frontDecorationList = new List<BackGroundDecorationController>();
    // カメラの映る範囲
    public Vector3[] cameraRange;
    // データの初期化をしたか
    public bool initializeStageData;
    // ステージの情報
    public StageData[] stageData;
    // ステージの情報
    public struct StageData
    {
        // クリアしているか
        public bool clear;
        // プレイできるか
        public bool canPlay;
        // 浄化率
        public int purificationRate;
    }



    //------------------------------------------------------------------------------------------
    // summary : シーンのCanvasの設定(泡の飾りのため)
    // remarks : none
    // param   : Canvas
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetCanvasOption(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvas.planeDistance = 50;
        canvas.sortingOrder = 5;
    }



    //------------------------------------------------------------------------------------------
    // summary : シーンのCanvasの設定をする(ボタンのため)
    // remarks : none
    // param   : CanvasScaler
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetCanvasScaleOption(CanvasScaler scaler)
    {
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }



    //------------------------------------------------------------------------------------------
    // summary : シーンのCameraが映す範囲を取得する
    // remarks : none
    // param   : Camera
    // return  : Vector3
    //------------------------------------------------------------------------------------------
    public Vector3[] GetCameraRange(Camera camera)
    {
        cameraRange = new Vector3[2];

        cameraRange[0] = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        cameraRange[1] = camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

        return cameraRange;
    }



    //------------------------------------------------------------------------------------------
    // summary : 保存してある飾りのリストの内容と同じ飾りを作る
    // remarks : none
    // param   : BackGroundDecorationGenerator、Vector2
    // return  : Vector3
    //------------------------------------------------------------------------------------------
    public void CreatePreviousSceneDecoration(BackGroundDecorationGenerator generator, Vector2 camera_position)
    {
        // 背景と前景の2回
        for (int i = 0; i < 2; i++)
        {
            List<BackGroundDecorationController> decorations;

            // 泡の飾りのリストをもらう
            switch (i)
            {
                case 0:
                    decorations = backDecorationList;
                    break;

                case 1:
                    decorations = frontDecorationList;
                    break;
                default:
                    decorations = new List<BackGroundDecorationController>();
                    break;
            }

            // 泡の飾りが一つでもあったら
            if (decorations.Count > 0)
            {
                // リストの大きさ分繰り返す
                for (int j = 0; j < decorations.Count; j++)
                {
                    // 記憶している情報からオブジェクトを生成してもらう
                    generator.CreateDecoration(decorations[j].GetPosition() + new Vector3(camera_position.x, camera_position.y, 0.0f), decorations[j].GetScale(), decorations[j].GetColor(), decorations[j].GetLayer());
                }
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : シーン内にある飾りをリストに入れる
    // remarks : none
    // param   : Vector2
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetDecorationList(Vector2 camera_position)
    {
        // リストをリセットする
        backDecorationList.Clear();
        frontDecorationList.Clear();

        // タグが「Decoration」のオブジェクトを全て取得する
        GameObject[] decoration_array = GameObject.FindGameObjectsWithTag("Decoration");

        for (int i = 0; i < decoration_array.Length; i++)
        {
            // 情報を記録する前に座標を覚える
            Vector3 pos = decoration_array[i].transform.position;
            // カメラから見た座標に調整する
            decoration_array[i].transform.position -= new Vector3(camera_position.x, camera_position.y, 0.0f);
            // リストに入る前に必要な情報を記憶する
            decoration_array[i].GetComponent<BackGroundDecorationController>().RememberInformation();
            // 調整前の座標に戻す
            decoration_array[i].transform.position = pos;

            // 前景だったら
            if (decoration_array[i].GetComponent<SpriteRenderer>().sortingOrder >= 0)
            {
                // 前景のリストに入れる
                frontDecorationList.Add(decoration_array[i].GetComponent<BackGroundDecorationController>());
            }
            // 背景だったら
            else
            {
                // 背景のリストに入れる
                backDecorationList.Add(decoration_array[i].GetComponent<BackGroundDecorationController>());
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージ番号を英語で文字列にする
    // remarks : none
    // param   : int
    // return  : string
    //------------------------------------------------------------------------------------------
    public string SetStageNameEnglish(int number)
    {
        string str_number = "";

        switch (number % 10)
        {
            // 「st」を付ける
            case 1:
                if (number != 11)
                {
                    str_number = number.ToString() + "st";
                }
                break;
            // 「nd」を付ける
            case 2:
                if (number != 12)
                {
                    str_number = number.ToString() + "nd";
                }
                break;
            // 「rd」を付ける
            case 3:
                if (number != 13)
                {
                    str_number = number.ToString() + "rd";
                }
                break;
        }
        // まだ何も設定されていなかったら
        if (str_number == "")
        {
            // 「th」を付ける
            str_number = number.ToString() + "th";
        }
        return str_number + " Stage";
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージデータのサイズを決める
    // remarks : none
    // param   : int
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetStageDataSize(int num)
    {
        // 初期化していなかったら
        if (initializeStageData == false)
        {
            stageData = new StageData[num];
            // 全て初期化
            for (int i = 0; i < num; i++)
            {
                stageData[i].clear = false;
                stageData[i].canPlay = false;
                stageData[i].purificationRate = 0;
            }
            // 一番最初のステージだけプレイできるようにする
            stageData[0].canPlay = true;
            // 初期化をしたことにする
            initializeStageData = true;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : パーセントからランクを決める
    // remarks : none
    // param   : int
    // return  : int
    //------------------------------------------------------------------------------------------
    public int GetPercentRank(int percent)
    {
        int rank = 0;
        // 29%以下だったら
        if (percent <= 29)
        {
            rank = 0;
        }
        // 69%以下だったら
        else if (percent <= 69)
        {
            rank = 1;
        }
        // 99%以下だったら
        else if (percent <= 99)
        {
            rank = 2;
        }
        // 100%だったら
        else
        {
            rank = 3;
        }
        return rank;
    }



    //------------------------------------------------------------------------------------------
    // summary : 浄化率をデータに記録する(クリア状況と次のステージがプレイできるようにする)
    // remarks : none
    // param   : int
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetPurificationRate(int purification)
    {
        // クリアしたことにする
        stageData[Data.stage_number-1].clear = true;
        // 今回の浄化率が前回の浄化率より大きかったら
        if (purification > stageData[Data.stage_number-1].purificationRate)
        {
            //浄化率を入れる
            stageData[Data.stage_number-1].purificationRate = purification;
        }
        // クリアした所の評価が2以上だったら
        if (GetPercentRank(purification) >= 2)
        {
            // クリアした所が一番最後のステージでなかったら
            if (Data.stage_number-1 != (stageData.Length - 1))
            {
                // 次のステージをプレイできるようにする
                stageData[Data.stage_number ].canPlay = true;
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージがクリア状況のデータを渡す
    // remarks : none
    // param   : int
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool GetClear(int stage_number)
    {
        return stageData[stage_number].clear;
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージがプレイできるかどうかのデータを渡す
    // remarks : none
    // param   : int
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool GetCanPlay(int stage_number)
    {
        return stageData[stage_number].canPlay;
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージ毎の浄化率のデータを渡す
    // remarks : none
    // param   : int
    // return  : int
    //------------------------------------------------------------------------------------------
    public int GetPurification(int stage_number)
    {
        return stageData[stage_number].purificationRate;
    }
}
