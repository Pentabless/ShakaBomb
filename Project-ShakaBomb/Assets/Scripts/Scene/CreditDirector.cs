//==============================================================================================
/// File Name	: CreditDirector.cs
/// Summary		: クレジットシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
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
    // CreditBoard(GameObject)
    [SerializeField, Header("Credit Board"), Tooltip("CreditBoardをアタッチする")]
    private GameObject creditBoard = null;
    // スクロールの速度
    [SerializeField, Header("スクロールの速度(0.0 - 10.0)"), Tooltip("スクロールの速度を設定する"), Range(0.0f, 10.0f)]
    private float scrollSpeed = ConstDecimal.ZERO;
    [SerializeField, Header("スクロールの限界"), Tooltip("スクロールの限界を設定する")]
    private float scrollLimit = ConstDecimal.ZERO;
    // 通過確認
    private bool isPressed = false;
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

        // スクロール処理
        ScrollBoard();
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
        isPassed = false;
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



    //------------------------------------------------------------------------------------------
    // summary : スクロール処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void ScrollBoard()
    {
        var speed = (scrollSpeed / 100);
        var positionY = creditBoard.transform.position.y;

        // スクロール
        creditBoard.transform.position += new Vector3(ConstDecimal.ZERO, speed, ConstDecimal.ZERO);

        if (!isPassed && positionY > scrollLimit)
        {
            // 通過確認
            isPassed = true;

            // 遷移処理
            Transition();
        }
    }
}
