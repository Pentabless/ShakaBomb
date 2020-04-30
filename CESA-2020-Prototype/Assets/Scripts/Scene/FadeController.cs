using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI

public class FadeController : MonoBehaviour
{
    //繰り返しフェードイン・フェードアウトするか
    public bool repeat_fade;
    //true…フェードイン　false…フェードアウト
    public bool fade_type;
    //フェードに必要な初期値(繰り返しなら角度　繰り返さなかったら1フレームごとの透明度の差)
    public float start_fade_value;
    //フェードに必要な値
    float fade_value;
    //オブジェクトの色
    Color obj_color;
    //透明度
    float alpha;
    //色変更するコンポーネント
    string componet_name;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトの色を覚える
        if (GetComponent<SpriteRenderer>())
        {
            //SpriteRendererの時
            obj_color = GetComponent<SpriteRenderer>().color;
            componet_name = "SpriteRenderer";
        }
        else
        {
            //Imageの時
            obj_color = GetComponent<Image>().color;
            componet_name = "Image";
        }

        //透明度を覚える
        alpha = obj_color.a;
        //初期化
        fade_value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //繰り返しフェードするか
        if (repeat_fade)
        {
            //フェードイン
            if (fade_type)
            {
                //0.0～1.0の間になるようにする
                alpha = ((Mathf.Sin(Mathf.Deg2Rad * fade_value)) + 1.0f) * 0.5f;
            }
            //フェードアウト
            else
            {
                //0.0～1.0の間になるようにする
                alpha = ((Mathf.Cos(Mathf.Deg2Rad * fade_value)) + 1.0f) * 0.5f;
            }
            //フェードに必要な値を足していく
            fade_value += start_fade_value;
        }
        else
        {
            //フェードイン
            if (fade_type)
            {
                //完全に不透明でなかったら
                if (alpha < 1.0f)
                {
                    alpha = fade_value;
                    //フェードに必要な値を足していく
                    fade_value += start_fade_value;

                    //透明度が1.0f以上になったら
                    if (alpha >= 1.0f)
                    {
                        //微調整する
                        alpha = 1.0f;
                    }
                }
            }
            //フェードアウト
            else
            {
                //完全に透明でなかったら
                if (alpha > 0.0f)
                {
                    alpha = 1.0f - fade_value;
                    //フェードに必要な値を足していく
                    fade_value += start_fade_value;
                    //透明度が0.0f以下になったら
                    if (alpha <= 0.0f)
                    {
                        //微調整する
                        alpha = 0.0f;
                    }
                }
            }
        }

        //オブジェクトに透明度を設定する
        if (componet_name == "SpriteRenderer")
        {
            GetComponent<SpriteRenderer>().color = new Color(obj_color.r, obj_color.g, obj_color.b, alpha);
        }
        else if (componet_name == "Image")
        {
            GetComponent<Image>().color = new Color(obj_color.r, obj_color.g, obj_color.b, alpha);
        }

    }

    //フェードに必要な値を設定する <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public void SetFadeValue(float value)
    {
        fade_value = value;
    }

    //繰り返しフェードするかを設定する <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public void SetRepeatFade(bool repeat)
    {
        repeat_fade = repeat;
    }

    //フェードイン・フェードアウトの設定 <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public void SetFadeType(bool type)
    {
        fade_type = type;
    }

    //フェードイン・フェードアウトの状態を渡す <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public bool GetFadeType()
    {
        return fade_type;
    }

    //フェードに必要な値を渡す(0～1までで返す)
    public float GetFadeValue()
    {
        return Mathf.Clamp(fade_value, 0.0f, 1.0f);
    }
}
