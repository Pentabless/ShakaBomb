using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI
using System.Text.RegularExpressions;   //93行辺り　Regex

public class StageFrameController : MonoBehaviour
{
    //最大拡大率
    public Vector3 max_scale;
    //拡大速度
    public float scale_speed;
    //評価の星の数(仮)
    public int provisional_num_rank_star;
    //色のついた星のスプライト
    public Sprite color_star;
    //カメラオブジェクト
    GameObject go_camera;
    //ステージフレーム
    GameObject go_stage_frame;
    //評価の星
    GameObject[] go_rank_star = new GameObject[3];
    //初期拡大率
    Vector3 start_scale;
    //初期色
    Color start_color;
    //拡大率の割合(0.0f = 初期拡大率　　1.0f = 最大拡大率)
    float scale_rate;
    //現在選ばれているか
    bool now_select;
    //評価の星の数
    int num_rank_star;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_camera = GameObject.Find("Main Camera");
        go_stage_frame = transform.Find("StageFrame").gameObject;
        //評価の星
        for (int i = 0; i < go_rank_star.Length; i++)
        {
            go_rank_star[i] = transform.Find("Star" + i.ToString()).gameObject;
        }

        //初期情報を覚える
        start_scale = transform.localScale;
        start_color = transform.Find("StageFrame").GetComponent<SpriteRenderer>().color;
        scale_rate = 0.0f;
        num_rank_star = provisional_num_rank_star;

        //Textを確実に表示できるようにするための処理

        //Canvasを覚える
        GameObject go_canvas = transform.Find("Canvas").gameObject;
        //Canvasの初期化をする
        InitializeCanvas(go_canvas);

        //Text(StageName)を覚える
        GameObject go_stage_name = go_canvas.transform.Find("StageName").gameObject;
        //Text(StageName)の初期化をする
        InitializeStageName(go_stage_name);

        //仮　ステージの画像の変更
        transform.Find("StageTex").GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        transform.Find("StageTex").GetComponent<SpriteRenderer>().color += new Color(0.1f, 0.1f, 0.1f);

        for (int i = 0; i < go_rank_star.Length; i++)
        {
            //評価の星の数の方が大きかったら
            if (i < num_rank_star)
            {
                go_rank_star[i].GetComponent<SpriteRenderer>().sprite = color_star;
                go_rank_star[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
            //小さかったらやめる
            else
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //選ばれていなかったら
        if (!now_select)
        {
            //元の大きさに戻っていなかったら
            if (transform.localScale != start_scale)
            {
                //割合を減らす
                scale_rate -= scale_speed;
                //元の拡大率に滑らかに戻る
                transform.localScale = Vector3.Lerp(start_scale, max_scale, scale_rate);

                //割合が0.0f以下だったら
                if (scale_rate <= 0.0f)
                {
                    //微調整する
                    transform.localScale = start_scale;
                    scale_rate = 0.0f;
                }
            }
        }
        //選ばれていたら
        else
        {
            //最大拡大率になっていなかったら
            if (transform.localScale != max_scale)
            {
                //割合を増やす
                scale_rate += scale_speed;
                //元の拡大率に滑らかに戻る
                transform.localScale = Vector3.Lerp(start_scale, max_scale, scale_rate);

                //割合が1.0f以上だったら
                if (scale_rate >= 1.0f)
                {
                    //微調整する
                    transform.localScale = max_scale;
                    scale_rate = 1.0f;
                }
            }
        }
    }
    //Canvasの初期化 <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void InitializeCanvas(GameObject obj)
    {
        //Canvasの情報を設定する
        Canvas canvas = obj.GetComponent<Canvas>();
        //RenderModeをScreenSpace-Cameraにする  (カメラに追従する)
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //TextとCameraの距離を50にする          (テキストが見えるようになる)
        canvas.GetComponent<Canvas>().planeDistance = 50;
        //RenderModeをWorldSpaceにする          (カメラに追従しなくなる)
        canvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        //DynamicPixelsPerUnitを10にする        (文字の輪郭が鮮明になる)
        canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;

        //RectTransformの情報を設定する
        RectTransform rect_transform = GetComponentInChildren<RectTransform>();
        //RectTransformの座標を原点にする
        rect_transform.localPosition = Vector3.zero;
        //RectTransformの拡大率を設定する
        rect_transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
    }

    //ステージ名の初期化 <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void InitializeStageName(GameObject grand_child)
    {
        //テキストの情報を設定する
        Text stage_name = grand_child.GetComponent<Text>();
        //テキストの色を設定する
        stage_name.color = Color.black;
        //テキストのフォントサイズを設定する
        stage_name.fontSize = 9;
        //テキストの内容を設定する
        stage_name.text = SetStageNameEnglish();
        //RectTransformの座標を設定する
        grand_child.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 10.0f, 0.0f);
    }

    //英語の何番目の表記設定 <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    string SetStageNameEnglish()
    {
        //オブジェクト名から0～9以外を""にした数値だけの文字列にして数値化する
        int number = int.Parse(Regex.Replace(this.name, @"[^0-9]", ""));
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

    //選ばれているかを設定する
    public void SetNowSelect(bool select)
    {
        now_select = select;
    }
}
