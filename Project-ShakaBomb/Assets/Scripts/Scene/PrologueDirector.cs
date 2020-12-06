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
    // 音声ファイル(SE)
    [SerializeField, Header("Pressed SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip pressedSE = null;


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
    private bool isPressed = false;



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

        //FadeManager.fadeColor = Color.white;

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
        // STARTボタンが押されたか
        IsPressedStartButton();

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
        isPressed = false;
    }



    //------------------------------------------------------------------------------------------
    // summary : STARTボタンが押された際に行う処理
    // remarks : Zキー（デバッグ用）
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedStartButton()
    {
        // STARTボタンが押されたか
        if (!isPressed &&
            Input.GetKeyDown(KeyCode.Joystick1Button7) ||
            Input.GetKeyDown(KeyCode.Z))
        {
            // 通過確認
            isPressed = true;

            // SEを再生
            SoundPlayer.Play(pressedSE);

            // 遷移処理
            Transition();
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
