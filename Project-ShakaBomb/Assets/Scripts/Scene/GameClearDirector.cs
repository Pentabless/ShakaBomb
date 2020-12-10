//==============================================================================================
/// File Name	: GameClearDirector.cs
/// Summary		: クリアシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class GameClearDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // SlideObject(GameObject)
    [SerializeField, Header("Slide Object"), Tooltip("SlideObjectをアタッチする")]
    private SlideInObject slideObject = null;
    // 音声ファイル(BGM)
    [SerializeField, Header("Clear BGM"), Tooltip("クリアシーンで再生したい音声ファイルをアタッチ")]
    private AudioClip allClearBGM = null;
    // 音声ファイル(SE)
    [SerializeField, Header("Pressed SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip pressedSE = null;
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

        // フェードインを開始
        FadeManager.FadeIn(ConstScene.FADE_TIME);

        // BGMを再生
        SoundPlayer.PlayBGM(allClearBGM);

        // UIのスライドインを開始
        slideObject.PlayIn();
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        // Aボタンが押されたか
        IsPressedAButton();
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
    // summary : Aボタンが押された際に行う処理
    // remarks : Zキー（デバッグ用）
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedAButton()
    {
        // Aボタンが押されたか
        if (!isPressed && Input.GetButtonDown(ConstGamePad.BUTTON_A))
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
    // remarks : クレジットシーンに遷移
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Transition()
    {
        // BGMのフェードアウトを開始
        SoundFadeController.SetFadeOutSpeed(ConstScene.SOUND_FADE_TIME);

        // フェードアウトを開始
        FadeManager.FadeOut(ConstScene.CREDIT, ConstScene.FADE_TIME);
    }
}
