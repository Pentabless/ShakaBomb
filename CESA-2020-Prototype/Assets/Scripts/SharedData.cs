using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedData : MonoBehaviour
{
    public static readonly SharedData instance = new SharedData();
    //プレイ中のステージ番号
    public int play_stage_number;
    //クリア時の制限時間
    public float play_time_limit;
    //前シーンの背景の泡の飾りのリスト
    public List<BackGroundDecorationController> back_decoration_list = new List<BackGroundDecorationController>();
    //前シーンの前景の泡の飾りのリスト
    public List<BackGroundDecorationController> front_decoration_list = new List<BackGroundDecorationController>();

    //シーンのCanvasの設定をする(泡の飾りのため)
    public void SetCanvasOption(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvas.planeDistance = 50;
        canvas.sortingOrder = 5;
    }
    
    //リストにあるデータと同じ飾りを作る
    public void CreatePreviousSceneDecoration(BackGroundDecorationGenerator generator)
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
                    Debug.Log("飾りの数=" + decorations.Count);
                    break;

                case 1:
                    decorations = front_decoration_list;
                    Debug.Log("飾りの数=" + decorations.Count);
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
                    generator.CreateDecoration(decorations[j].GetPosition(), decorations[j].GetScale(), decorations[j].GetColor(), decorations[j].GetLayer());
                }
            }
        }
    }

    //泡の飾りをリストに入れる
    public void SetDecorationList()
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
            //リストに入る前に必要な情報を記憶する
            decoration_array[i].GetComponent<BackGroundDecorationController>().RememberInformation();
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

    //ステージ番号を英語で文字列にする
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



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
