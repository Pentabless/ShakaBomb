﻿/*--------------------------------------*/
/*--ファイル名：SoundFadeController.cs--*/
/*--概要：サウンドのフェードを処理する--*/
/*--------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFadeController : MonoBehaviour
{
    //オーディオソース
    private static AudioSource audio_source;
    //フェードスピード
    private static float fade_speed;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        fade_speed = 0.0f;
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
        //ボリュームの限界になったらフェードを切る
        FadeSpeedReset();
        //ボリュームをフェードスピード分足していく
        audio_source.volume += fade_speed;
    }
    /*--終わり：Update--*/

    /*------------------------------------------------------*/
    /*--関数名：FadeSpeedReset(private)---------------------*/
    /*--概要：フェードの限界になったら自動でフェードを切る--*/
    /*--引数：なし------------------------------------------*/
    /*--戻り値：なし----------------------------------------*/
    /*------------------------------------------------------*/
    private void FadeSpeedReset()
    {
        //フェードインしている時
        if (fade_speed > 0)
        {
            //1.0になったら
            if (audio_source.volume >= 1.0f)
            {
                //フェードスピードを0にする
                fade_speed = 0.0f;
            }
        }
        //フェードアウトしている時
        else if (fade_speed < 0)
        {
            //0.0になったら
            if (audio_source.volume <= 0.0f)
            {
                //フェードスピードを0にする
                fade_speed = 0.0f;
            }
        }
    }
    /*--終わり：FadeSpeedReset--*/

    /*----------------------------------------------------*/
    /*--関数名：SetFadeInSpeed(public)--------------------*/
    /*--概要：フェードインをするためにスピードを設定する--*/
    /*--引数：フェードスピード(float)---------------------*/
    /*--戻り値：なし--------------------------------------*/
    /*----------------------------------------------------*/
    public static void SetFadeInSpeed(float speed)
    {
        //必ずプラスにする
        fade_speed = 1.0f * Mathf.Abs(speed);
    }
    /*--終わり：SetFadeInSpeed--*/

    /*------------------------------------------------------*/
    /*--関数名：SetFadeOutSpeed(public)---------------------*/
    /*--概要：フェードアウトをするためにスピードを設定する--*/
    /*--引数：フェードスピード(float)-----------------------*/
    /*--戻り値：なし----------------------------------------*/
    /*------------------------------------------------------*/
    public static void SetFadeOutSpeed(float speed)
    {
        //必ずマイナスにする
        fade_speed = -1.0f * Mathf.Abs(speed);
    }
    /*--終わり：SetFadeOutSpeed--*/

    /*----------------------------------*/
    /*--関数名：NotFadeSound(public)----*/
    /*--概要：フェードしないようにする--*/
    /*--引数：なし----------------------*/
    /*--戻り値：なし--------------------*/
    /*----------------------------------*/
    public static void NotSoundFade()
    {
        //フェードスピードを0にする
        fade_speed = 0.0f;
    }
    /*--終わり：NotFadeSound--*/

    /*----------------------------------*/
    /*--関数名：FadeCompleted(public)----*/
    /*--概要：フェード終了を感知--*/
    /*--引数：なし----------------------*/
    /*--戻り値：bool--------------------*/
    /*----------------------------------*/
    public static AudioSource GetAudioSource()
    {
        return audio_source;
    }
    /*--終わり：FadeCompleted--*/

}