//==============================================================================================
/// File Name	: TitleDirector.cs
/// Summary		: タイトルシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class TitleDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 音声ファイル(BGM)
    [SerializeField,Header("Title BGM"),Tooltip("タイトルシーンで再生したい音声ファイルをアタッチ")]
    private AudioClip titleBGM = null;
    // 音声ファイル(SE)
    [SerializeField, Header("Game Start SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip startSE = null;
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

        // フェードインを開始
        FadeManager.FadeIn(ConstScene.FADE_TIME);

        // BGMを再生
        SoundPlayer.PlayBGM(titleBGM);
    }



    //------------------------------------------------------------------------------------------
    // summary : Aボタンが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedAButton()
    {
        // Aボタンが押されたか
        if (!isPressed && Input.GetButtonDown(ConstGamePad.BUTTON_A))
        {
            isPressed = true;

            // SEを再生
            SoundPlayer.Play(startSE);

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
        FadeManager.FadeOut(ConstScene.PROLOGUE, ConstScene.FADE_TIME);
    }
}
