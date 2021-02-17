//==============================================================================================
/// File Name	: TitleDirector.cs
/// Summary		: タイトルシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
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
    // PressAButton(Image)
    [SerializeField, Header("PressAButton(Image)"), Tooltip("PressAButton(Image)をアタッチする")]
    private Image pressAButtonImage = null;
    // 通過確認
    private bool isAPressed = false;
    private bool isXPressed = false;
    private bool isYPressed = false;



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
        SoundPlayer.PlayBGM(titleBGM);
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

        // Xボタンが押されたか
        IsPressedXButton();

        // Yボタンが押されたか
        IsPressedYButton();
    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        isAPressed = false;
        isXPressed = false;
        isXPressed = false;

        SharedData.instance.initializeStageData = false;
        SharedData.instance.SetStageDataSize(15);
        Data.stage_number = 0;
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
        if (!isXPressed && !isYPressed && !isAPressed && Input.GetButtonDown(ConstGamePad.BUTTON_A))
        {
            // 通過確認
            isAPressed = true;

            // SEを再生
            SoundPlayer.Play(startSE);

            // ブリンク処理を停止させる
            StopBlink();

            // アピール処理を行う
            pressAButtonImage.GetComponent<AppealImage>().Play();

            // 遷移処理
            Transition();
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : Xボタンが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedXButton()
    {
        // Xボタンが押されたか
        if (!isXPressed && !isYPressed && !isAPressed && Input.GetButtonDown(ConstGamePad.BUTTON_X))
        {
            // 通過確認
            isXPressed = true;

            // SEを再生
            SoundPlayer.Play(startSE);

            // ゲーム終了
            ExitGame();
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : Yボタンが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedYButton()
    {
        // Yボタンが押されたか
        if (!isXPressed && !isYPressed && !isAPressed && Input.GetButtonDown(ConstGamePad.BUTTON_Y))
        {
            // 通過確認
            isYPressed = true;

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
        if(isAPressed)
        {
            // BGMのフェードアウトを開始
            SoundFadeController.SetFadeOutSpeed(ConstScene.SOUND_FADE_TIME);

            // フェードアウトを開始
            FadeManager.FadeOut(ConstScene.PROLOGUE, ConstScene.FADE_TIME);
        }
        else if (isYPressed)
        {
            // BGMのフェードアウトを開始
            SoundFadeController.SetFadeOutSpeed(ConstScene.SOUND_FADE_TIME);

            // フェードアウトを開始
            FadeManager.FadeOut(ConstScene.CREDIT, ConstScene.FADE_TIME);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ブリンク処理を停止する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void StopBlink()
    {
        pressAButtonImage.GetComponent<Blink>().StopBlink();
    }



    //------------------------------------------------------------------------------------------
    // summary : ゲームを終了する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
