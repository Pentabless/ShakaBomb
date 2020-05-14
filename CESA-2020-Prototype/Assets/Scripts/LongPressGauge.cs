/*------------------------------------------------------------*/
/*--ファイル名：LongPressGauge.cs-----------------------------*/
/*--概要：指定したボタンが長押し状況に応じてゲージを変形する--*/
/*------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI


public class LongPressGauge : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    //ゲージを付けるオブジェクトの名前
    public string object_name;
    //ゲージとオブジェクトの座標の差
    public Vector3 offset;
    //長押しされてるかを確認するボタンの名前
    public string check_button_name;
    //ゲージが満タンになるまでの時間(単位：秒)
    public float full_gauge_time_second;
    //満タンの時のゲージが消えるまでの時間(単位：秒)
    public float empty_gauge_time_second;

    /*-----------*/
    /*--private--*/
    /*-----------*/
    //ゲームオブジェクトの場所
    private Transform game_object_transform;
    //ゲージの形を変えるためのコンポーネント
    private Image gauge_image;
    //ゲージの位置を変えるためのコンポーネント
    private RectTransform rect_transform;
    //ゲージの割合
    private float gauge_rate;
    //押し始めた時間
    private float start_press_time;
    //ゲージの減少速度
    private float decrease_speed;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //オブジェクトを探す
        game_object_transform = GameObject.Find(object_name).transform;
        //コンポーネントを探す
        Canvas canvas = transform.parent.gameObject.GetComponent<Canvas>();
        gauge_image = GetComponent<Image>();
        rect_transform = transform.parent.gameObject.GetComponent<RectTransform>();

        //Canvasの設定
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvas.planeDistance = 50;
        canvas.sortingOrder = 15;
        canvas.renderMode = RenderMode.WorldSpace;
        //RectTransformの設定
        rect_transform.localPosition = new Vector3(game_object_transform.position.x + offset.x, game_object_transform.position.y + offset.y, 10.0f);

        //初期化
        gauge_rate = 0.0f;
        start_press_time = 0.0f;
        decrease_speed = 1.0f / (empty_gauge_time_second * Application.targetFrameRate);

        Debug.Log(decrease_speed);
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
        //押し始めた時間を覚えていなくて　指定されたボタンを押していたら
        if (start_press_time == 0.0f && (Input.GetAxis(check_button_name) > 0))
        {
            //押し始めた時間を記録する
            start_press_time = Time.time;
        }
        //押し始めた時間を覚えていて 押していなかったら
        else if (start_press_time != 0.0f && (Input.GetAxis(check_button_name) == 0))
        {
            //押し始めた時間を忘れる
            start_press_time = 0.0f;
        }


        //ゲージの割合が1未満で　押し始めた時間を記憶していたら
        if ((gauge_rate < 1.0f) && (start_press_time != 0.0f))
        {
            //割合を求める(長押ししている時間÷満タンになるまでの時間)
            gauge_rate = (Time.time - start_press_time) / full_gauge_time_second;

            if (gauge_rate >= 1.0f)
            {
                gauge_rate = 1.0f;
            }
        }
        //ゲージの割合が0より大きくて　記憶していなかったら
        else if ((gauge_rate > 0.0f) && (start_press_time == 0.0f))
        {
            //割合を求める(長押ししている時間÷満タンになるまでの時間)
            gauge_rate -= decrease_speed;

            if (gauge_rate <= 0.0f)
            {
                gauge_rate = 0.0f;
            }
        }
        //ゲージの割合を変更する
        gauge_image.fillAmount = gauge_rate;
        //Debug.Log("RATE:" + gauge_rate);

        //ゲージの場所を変更する
        rect_transform.localPosition = new Vector3(game_object_transform.position.x + offset.x, game_object_transform.position.y + offset.y, 10.0f);
    }
    /*--終わり：Update--*/
}
