/*--------------------------------------------------*/
/*--ファイル名：LogoDirector.cs---------------------*/
/*--概要：ロゴシーンの処理(ロゴの描画やシーン遷移)--*/
/*--------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI


public class LogoDirector : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    //次のシーンに移るまでの時間(単位：秒)
    public float next_scene_second_time;
    //ロゴがフェードアウトする時間の割合(next_scene_second_timeを1とする)
    public float logo_fade_out_time_rate;
    //スクリーンフェードがフェードアウトする時間の割合(next_scene_second_timeを1とする)
    public float screen_fade_out_time_rate;

    /*-----------*/
    /*--private--*/
    /*-----------*/
    //ロゴ
    private GameObject go_logo;
    //スクリーンフェード
    private FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    //カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    //ロゴシーンが始まった時間
    private float start_time;


    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //オブジェクトを探す
        go_logo = GameObject.Find("LogoTex");
        //コンポーネントを探す
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //初期化
        start_time = Time.time;
        //フェードに必要な値を渡す
        go_logo.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * logo_fade_out_time_rate);
        sc_screen_fade.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * screen_fade_out_time_rate);

        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
        //Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(GameObject.Find("Main Camera").GetComponent<Camera>());

        //一番最初はプレイしていたステージ番号を0にする
        SharedData.instance.play_stage_number = 0;
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

        //次のシーンに移るまでの時間の2/4過ぎていて　次のシーンに移るまでの時間内だったら
        if (((Time.time - start_time >= next_scene_second_time * 0.5f) && (Time.time - start_time <= next_scene_second_time)) ||
            //Spaceキーを押したら
            (Input.GetKey(KeyCode.Space)) ||
            //Startボタンを押したら
            (Input.GetAxis("Start") > 0) ||
            //Aボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_A) > 0) ||
            //Bボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_B) > 0) ||
            //Xボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_X) > 0) ||
            //Yボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_Y) > 0))
        {
            //ロゴがフェードインする設定になっていたら
            if (go_logo.GetComponent<FadeController>().GetFadeType() == true)
            {
                //ロゴをフェードアウトする
                go_logo.GetComponent<FadeController>().SetFadeType(false);
                go_logo.GetComponent<FadeController>().SetFadeValue(0.0f);
                //スクリーンフェードをフェードインする
                sc_screen_fade.SetFadeType(true);
                sc_screen_fade.SetFadeValue(0.0f);
            }
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 3.0f);
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), 10);
        }

        //スクリーンフェードがフェードインし終わったら　or　次のシーンに移るまでの時間より大きくなったら
        if ((sc_screen_fade.GetFadeValue() == 1.0f) || (Time.time - start_time >= next_scene_second_time))
        {
            //SharedDataにあるリストに飾りを入れる
            SharedData.instance.SetDecorationList();
            //タイトルへ移る
            SceneManager.LoadScene("TitleScene");
        }
    }
    /*--終わり：Update--*/
}
