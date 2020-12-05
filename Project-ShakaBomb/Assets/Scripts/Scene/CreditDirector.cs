//==============================================================================================
/// File Name	: CreditDirector.cs
/// Summary		: クレジットシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class CreditDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 音声ファイル(BGM)
    [SerializeField, Header("Credit BGM"), Tooltip("クレジットシーンで再生したい音声ファイルをアタッチ")]
    private AudioClip creditBGM = null;
    // 音声ファイル(SE)
    [SerializeField, Header("Pressed SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip pressedSE = null;
    // ボタンが押されたか
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

        // フェードインを開始
        FadeManager.FadeIn(ConstScene.FADE_TIME);

        // BGMを再生
        SoundPlayer.PlayBGM(creditBGM);
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
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Transition()
    {
        // BGMのフェードアウトを開始
        SoundFadeController.SetFadeOutSpeed(ConstScene.SOUND_FADE_TIME);

        // フェードアウトを開始
        FadeManager.FadeOut(ConstScene.STAGE_SELECT, ConstScene.FADE_TIME);
    }
}
