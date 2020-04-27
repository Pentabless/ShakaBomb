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
    //背景の飾りジェネレーター
    GameObject go_decoration_generator;
    //スクリーンフェード
    Image image_screen_fade;
    //ロゴシーンが始まった時間
    float start_time;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_logo = GameObject.Find("LogoTex");
        go_decoration_generator = GameObject.Find("BackGroundDecorationGenerator");
        image_screen_fade = GameObject.Find("ScreenFade").GetComponent<Image>();
        //初期化
        start_time = Time.time;
        //次のシーンに移るまでの時間の1/4
        go_logo.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * 0.25f);
        image_screen_fade.GetComponent<FadeController>().start_fade_value = 1.0f / (next_scene_second_time * 60.0f * 0.5f);
        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
    }

    // Update is called once per frame
    void Update()
    {
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        go_decoration_generator.GetComponent<BackGroundDecorationGenerator>().CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

        //次のシーンに移るまでの時間の2/4過ぎていたら　or　特定のキー(Spaceキー)を押したら
        if (((Time.time - start_time >= next_scene_second_time * 0.5f) && (Time.time - start_time <= next_scene_second_time))||
            (Input.GetKey(KeyCode.Space)))
        {
            //ロゴがフェードインする設定になっていたら
            if (go_logo.GetComponent<FadeController>().GetFadeType() == true)
            {
                //ロゴをフェードアウトする
                go_logo.GetComponent<FadeController>().SetFadeType(false);
                go_logo.GetComponent<FadeController>().SetFadeValue(0.0f);
                //スクリーンフェードをフェードインする
                image_screen_fade.GetComponent<FadeController>().SetFadeType(true);
                image_screen_fade.GetComponent<FadeController>().SetFadeValue(0.0f);
            }
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 3.0f);
            go_decoration_generator.GetComponent<BackGroundDecorationGenerator>().CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f), 10);
        }

        //スクリーンフェードがフェードインし終わったら　or　次のシーンに移るまでの時間より大きくなったら
        if ((image_screen_fade.color.a>=1.0f)||(Time.time - start_time >= next_scene_second_time))
        {
            //SharedDataにあるリストに飾りを入れる
            SharedData.instance.SetDecorationList();
            //タイトルへ移る
            SceneManager.LoadScene("TitleScene");
        }
    }
}
