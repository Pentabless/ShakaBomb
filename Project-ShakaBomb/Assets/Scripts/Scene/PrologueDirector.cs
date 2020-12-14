//==============================================================================================
/// File Name	: PrologueDirector.cs
/// Summary		: プロローグシーンの管理を行うクラス
//==============================================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class PrologueDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    // カウントダウン
    private readonly int COUNT_MAX = (300);
    // ステート
    private enum PrologueState
    {
        START,
        SHOW,
        CROSS_FADE,
        END,
    }
    


    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // SlideObject(GameObject)
    [SerializeField, Header("Slide Object"), Tooltip("SlideObjectをアタッチする")]
    private SlideInObject slideObject = null;
    // 音声ファイル(SE)
    [SerializeField, Header("Pressed SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip pressedSE = null;
    // カウントダウン
    private int countDown = ConstInteger.ZERO;
    // イメージ
    [SerializeField]
    private List<Image> images = null;
    // イメージのフェード時間
    [SerializeField]
    private float fadeTime = ConstDecimal.ZERO;
    // イメージの表示時間
    [SerializeField]
    private float showTime = ConstDecimal.ZERO;
    // クロスフェードの時間
    [SerializeField]
    private float crossFadeTime = ConstDecimal.ZERO;
    // ステート
    private PrologueState state = PrologueState.START;
    private int index = ConstInteger.ZERO;
    private float timer = ConstDecimal.ZERO;
    // 通過確認
    private bool isStartPressed = false;
    private bool isAnyPressed = false;
    private bool isPassed = false;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        // 初期化処理
        Init();

        // フェードインを開始
        FadeManager.FadeIn(ConstScene.FADE_TIME);
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        // いずれかのボタンが押された
        IsPressedAnyButton();

        // ステートの切り替え処理
        SwitchingState();
    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        isStartPressed = false;
        isAnyPressed = false;
        countDown = COUNT_MAX;

        if(!isPassed)
        {
            foreach (var image in images)
            {
                // 配列内のイメージを非表示
                image.color = Color.clear;
            }
            // 1枚目のイメージを表示
            images[0].color = Color.white;

            // 通過確認
            isPassed = true;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ステート切り替え
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void SwitchingState()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case PrologueState.START:
                if (timer >= fadeTime)
                {
                    timer = 0;
                    state = PrologueState.SHOW;
                }
                break;
            case PrologueState.SHOW:
                if (timer >= showTime)
                {
                    timer = 0;
                    if (index < images.Count - 1)
                    {
                        state = PrologueState.CROSS_FADE;
                        index++;
                    }
                    else
                    {
                        state = PrologueState.END;
                        FadeManager.fadeColor = Color.black;
                        FadeManager.FadeOut(ConstScene.TUTORIAL, fadeTime);
                        SoundFadeController.SetFadeOutSpeed(1.0f / fadeTime / 60 * SoundFadeController.GetAudioSource().volume * 1.5f);
                    }
                }
                break;
            case PrologueState.CROSS_FADE:
                if (timer >= showTime)
                {
                    images[index].color = Color.white;
                    timer = 0;
                    state = PrologueState.SHOW;
                }
                else
                {
                    float alpha = Mathf.SmoothStep(0, 1, timer / crossFadeTime);
                    images[index].color = new Color(1, 1, 1, alpha);
                }
                break;
            case PrologueState.END:
                break;
            default:
                Debug.Log("PrologeState:error");
                break;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : いずれかのボタンが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedAnyButton()
    {
        if (!isAnyPressed && Input.anyKey)
        {
            // UIの表示状態の取得
            var isDisplay = slideObject.GetObjectState();

            if (!isDisplay)
            {
                // 通過確認
                isAnyPressed = true;

                // UIのスライドインを開始
                slideObject.PlayIn();
            }
        }

        // STARTボタンが押されたか
        IsPressedStartButton();
    }



    //------------------------------------------------------------------------------------------
    // summary : STARTボタンが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedStartButton()
    {
        if (isAnyPressed)
        {
            // UIの表示状態の取得
            var isDisplay = slideObject.GetObjectState();

            if(isDisplay)
            {
                // カウントダウン処理
                CountDown();
            }

            // STARTボタンが押されたか
            if (!isStartPressed && Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                // 通過確認
                isStartPressed = true;

                // SEを再生
                SoundPlayer.Play(pressedSE);

                // 遷移処理
                Transition();
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : カウントダウン処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void CountDown()
    {
        // カウントダウン
        countDown--;

        if (countDown < 0)
        {
            // UIのスライドアウトを開始
            slideObject.PlayOut();

            // 初期化処理
            Init();
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 遷移処理
    // remarks : チュートリアルシーンに遷移
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Transition()
    {
        // BGMのフェードアウトを開始
        SoundFadeController.SetFadeOutSpeed(ConstScene.SOUND_FADE_TIME);

        // フェードアウトを開始
        FadeManager.FadeOut(ConstScene.TUTORIAL, ConstScene.FADE_TIME);
    }
}
