using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI

public class TitleDirector : MonoBehaviour
{
    //選択するスピード
    public float select_frame_speed;
    //ゲームを始めるボタン
    GameObject go_start_button;
    //ゲームをやめるボタン
    GameObject go_exit_button;
    //選択フレーム
    GameObject go_select_tex;
    //画面フェード
    FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    BackGroundDecorationGenerator sc_decoration_generator;
    //距離
    Vector3 button_distance;
    //覚える座標
    Vector3 last_position;
    //選択フレーム用角度
    float select_frame_angle;
    //フェード用角度
    float fade_angle;
    //ゲームをやめるかどうか
    bool select_eixt;
    //フェードアウトが始まっているか
    bool start_fade_out;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_start_button = GameObject.Find("StartButton");
        go_exit_button = GameObject.Find("ExitButton");
        go_select_tex = GameObject.Find("SelectTex");
        //コンポーネントを探す
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //座標変更
        go_select_tex.transform.position = go_start_button.transform.position;
        //拡大率変更
        go_select_tex.transform.localScale = go_start_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
        //初期化
        button_distance = Vector3.zero;
        last_position = Vector3.zero;
        select_frame_angle = 0.0f;
        fade_angle = 0.0f;
        select_eixt = false;
        start_fade_out = false;

        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());

        //飾りを作成する(背景と前景)
        SharedData.instance.CreatePreviousSceneDecoration(sc_decoration_generator);
    }

    // Update is called once per frame
    void Update()
    {
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

        //フェードアウトを始めていなかったら
        if (start_fade_out == false)
        {
            //選択している画像が動いていなかったら
            if (select_frame_angle == 0.0f)
            {
                //上矢印キーを押したら
                if ((Input.GetKeyDown(KeyCode.UpArrow)) ||
                    //十字上ボタンを押したら
                    (Input.GetAxis("cross Y") < 0) ||
                    //左スティックを上に傾けたら
                    (Input.GetAxis(Common.GamePad.VERTICAL) > 0))
                {
                    //EXIT GAMEを選択していたら
                    if (select_eixt == true)
                    {
                        //EXIT GAMEを選択していない事にする
                        select_eixt = false;
                        PreparaChangeButton(select_eixt);
                    }
                }
                //下矢印キーを押したら
                if ((Input.GetKeyDown(KeyCode.DownArrow)) ||
                    //十字下ボタンを押したら
                    (Input.GetAxis("cross Y") > 0) ||
                    //左スティックを下に傾けたら
                    (Input.GetAxis(Common.GamePad.VERTICAL) < 0))
                {
                    //EXIT GAMEを選択していなかったら
                    if (select_eixt == false)
                    {
                        //EXIT GAMEを選択している事にする
                        select_eixt = true;
                        PreparaChangeButton(select_eixt);
                    }
                }

                //フェードインしている途中でなかったら
                if ((sc_screen_fade.GetFadeType() == false) && (sc_screen_fade.GetFadeValue() == 1.0f))
                {
                    //Spaceキーを押したら
                    if ((Input.GetKeyDown(KeyCode.Space)) ||
                        //Aボタンを押したら
                        (Input.GetAxis(Common.GamePad.BUTTON_A) > 0))
                    {
                        //ExitGameを選択していなかったら
                        if (select_eixt == false)
                        {
                            //フェードアウトを始める
                            sc_screen_fade.SetFadeType(true);
                            sc_screen_fade.SetFadeValue(0.0f);
                            //フェードアウトが始まった事にする
                            start_fade_out = true;
                        }
                        else
                        {
                            //ゲームを閉じる
                        }
                    }
                }
            }
            //動いている途中だったら
            else
            {
                //選択フレームを動かす
                go_select_tex.transform.position =
                    last_position +
                    new Vector3(
                    (Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * (button_distance.x * 0.5f),
                    (Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * (button_distance.y * 0.5f),
                    0.0f);

                //選択フレームの大きさ変更
                if (select_eixt)
                {
                    go_select_tex.transform.localScale = Vector3.Lerp(
                        go_start_button.transform.localScale,
                        go_exit_button.transform.localScale,
                        ((Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * 0.5f));
                }
                else
                {
                    go_select_tex.transform.localScale = Vector3.Lerp(
                        go_exit_button.transform.localScale,
                        go_start_button.transform.localScale,
                        ((Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * 0.5f));
                }
                go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);

                //半周していなかったら
                if (select_frame_angle < 180.0f)
                {
                    select_frame_angle += select_frame_speed;
                }
                //半周していたら
                else
                {
                    //微調整する
                    if (select_eixt)
                    {
                        go_select_tex.transform.position = go_exit_button.transform.position;
                        go_select_tex.transform.localScale = go_exit_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
                    }
                    else
                    {
                        go_select_tex.transform.position = go_start_button.transform.position;
                        go_select_tex.transform.localScale = go_start_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
                    }
                    //円運動が終わった事にする
                    select_frame_angle = 0.0f;
                }
            }
        }
        //フェードアウトを始めていたら
        else
        {
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 3.0f);
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f), 10);

            //フェードアウトが終わったら
            if (sc_screen_fade.GetFadeValue() == 1.0f)
            {
                //SharedDataにあるリストに飾りを入れる
                SharedData.instance.SetDecorationList();
                //ステージ選択画面に移る
                SceneManager.LoadScene("StageSelectScene");
            }
        }
    }


    //選択しているボタンを変更する準備 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void PreparaChangeButton(bool select)
    {
        //円運動を始める
        select_frame_angle += select_frame_speed;
        //ゲームをやめるを選択していたら
        if (select)
        {
            //選択していたボタンの座標を覚える
            last_position = go_start_button.transform.position;
            //ゲームをやめるボタンとゲームを始めるボタンの距離を求める
            button_distance = go_exit_button.transform.position - go_start_button.transform.position;
        }
        else
        {
            //選択していたボタンの座標を覚える
            last_position = go_exit_button.transform.position;
            //ゲームを始めるボタンとゲームをやめるボタンの距離を求める
            button_distance = go_start_button.transform.position - go_exit_button.transform.position;
        }
    }
}
