/*----------------------------------------------------------*/
/*--ファイル名：TitleDirector.cs----------------------------*/
/*--概要：タイトルシーンの処理(タイトルの描画やシーン遷移)--*/
/*----------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI


public class TitleDirector : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    //選択するスピード
    public float select_frame_speed;

    /*-----------*/
    /*--private--*/
    /*-----------*/
    //ゲームを始めるボタン
    private GameObject go_start_button;
    //ゲームをやめるボタン
    private GameObject go_exit_button;
    //選択フレーム
    private GameObject go_select_tex;
    //画面フェード
    private FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    //カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    //距離
    private Vector3 button_distance;
    //覚える座標
    private Vector3 last_position;
    //選択フレーム用角度
    private float select_frame_angle;
    //フェード用角度
    private float fade_angle;
    //ゲームをやめるかどうか
    private bool select_eixt;
    //フェードアウトが始まっているか
    private bool start_fade_out;


    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
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
        //Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(GameObject.Find("Main Camera").GetComponent<Camera>());
        //飾りを作成する(背景と前景)
        SharedData.instance.CreatePreviousSceneDecoration(sc_decoration_generator, GameObject.Find("Main Camera").transform.position);
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
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

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
                //選択フレームの更新(移動と拡大率変更)
                UpdateSelectFrame(select_eixt);

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
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), 10);

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
    /*--終わり：Update--*/

    /*------------------------------------------*/
    /*--関数名：PreparaChangeButton(private)----*/
    /*--概要：選択しているボタンを変更する準備--*/
    /*--引数：ExitGameを選択しているか(bool)----*/
    /*--戻り値：なし----------------------------*/
    /*------------------------------------------*/
    private void PreparaChangeButton(bool select)
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
    /*--終わり：PreparaChangeButton--*/

    /*----------------------------------------------*/
    /*--関数名：UpdateSelectFrame(private)----------*/
    /*--概要：選択フレームの更新(移動と拡大率変更)--*/
    /*--引数：ExitGameを選択しているか(bool)--------*/
    /*--戻り値：なし--------------------------------*/
    /*----------------------------------------------*/
    private void UpdateSelectFrame(bool select)
    {
        //選択フレームを動かす
        go_select_tex.transform.position =
            last_position +
            new Vector3(
            (Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * (button_distance.x * 0.5f),
            (Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * (button_distance.y * 0.5f),
            0.0f);

        //始点
        Transform start_button;
        //終点
        Transform end_button;
        //ExitGameを選んでいたら
        if (select)
        {
            //始点…GameStartボタン
            start_button = go_start_button.transform;
            //終点…ExitGameボタン
            end_button = go_exit_button.transform;
        }
        else
        {
            //始点…ExitGameボタン
            start_button = go_exit_button.transform;
            //終点…GameStartボタン
            end_button = go_start_button.transform;
        }

        //大きさ変更
        go_select_tex.transform.localScale = Vector3.Lerp(
            start_button.transform.localScale,
            end_button.transform.localScale,
            ((Mathf.Sin(Mathf.Deg2Rad * (select_frame_angle - 90.0f)) + 1) * 0.5f));
        //ボタンより少し大きくする
        go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);

    }
    /*--終わり：UpdateSelectFrame--*/

}
