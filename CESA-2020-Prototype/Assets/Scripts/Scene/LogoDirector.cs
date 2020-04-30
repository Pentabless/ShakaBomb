using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI

public class LogoDirector : MonoBehaviour
{
    //次のシーンに移るまでの時間(単位：秒)
    public float next_scene_second_time;
    //ロゴ
    GameObject go_logo;
    //スクリーンフェード
    FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    BackGroundDecorationGenerator sc_decoration_generator;
    //ロゴシーンが始まった時間
    float start_time;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_logo = GameObject.Find("LogoTex");
        //コンポーネントを探す
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //初期化
        start_time = Time.time;
        //次のシーンに移るまでの時間の1/4
        go_logo.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * 0.25f);
        sc_screen_fade.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * 0.5f);
        
        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
    }

    // Update is called once per frame
    void Update()
    {
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

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
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f), 10);
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
}
