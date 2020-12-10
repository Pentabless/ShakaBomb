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
    enum PrologueState
    {
        Start,
        Show,
        CrossFade,
        End,
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


    [SerializeField]
    private List<Image> images = null;
    [SerializeField]
    private float fadeTime = 0;
    [SerializeField]
    private float showTime = 0;
    [SerializeField]
    private float crossFadeTime = 0;

    private PrologueState state = PrologueState.Start;
    private int index = 0;
    private float timer = 0;

    // 通過確認
    private bool isStartPressed = false;
    private bool isAnyPressed = false;



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

        foreach(var image in images)
        {
            image.color = Color.clear;
        }
        images[0].color = Color.white;

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

        timer += Time.deltaTime;
        switch (state)
        {
            case PrologueState.Start:
                if(timer >= fadeTime)
                {
                    timer = 0;
                    state = PrologueState.Show;
                }
                break;
            case PrologueState.Show:
                if(timer >= showTime)
                {
                    timer = 0;
                    if(index < images.Count - 1)
                    {
                        state = PrologueState.CrossFade;
                        index++;
                    }
                    else
                    {
                        state = PrologueState.End;
                        FadeManager.fadeColor = Color.black;
                        FadeManager.FadeOut("TutorialScene", fadeTime);
                        SoundFadeController.SetFadeOutSpeed(1.0f / fadeTime / 60 * SoundFadeController.GetAudioSource().volume * 1.5f);
                    }
                }
                break;
            case PrologueState.CrossFade:
                if (timer >= showTime)
                {
                    images[index].color = Color.white;
                    timer = 0;
                    state = PrologueState.Show;
                }
                else
                {
                    float alpha = Mathf.SmoothStep(0, 1, timer / crossFadeTime);
                    images[index].color = new Color(1, 1, 1, alpha);
                }
                break;
            case PrologueState.End:
                break;
            default:
                Debug.Log("PrologeState:error");
                break;
        }
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
