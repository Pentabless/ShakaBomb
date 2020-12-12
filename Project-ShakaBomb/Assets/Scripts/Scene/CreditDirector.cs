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
    // const variable
    //------------------------------------------------------------------------------------------
    // カウントダウン
    private readonly int COUNT_MAX = (300);



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
    // SlideObject(GameObject)
    [SerializeField, Header("Slide Object"), Tooltip("SlideObjectをアタッチする")]
    private SlideInObject slideObject = null;
    // スクロールの速度
    [SerializeField, Header("スクロールの速度(0.0 - 10.0)"), Tooltip("スクロールの速度を設定する"), Range(0.0f, 10.0f)]
    private float scrollSpeed = ConstDecimal.ZERO;
    // スクロールの限界
    [SerializeField, Header("スクロールの限界"), Tooltip("スクロールの限界を設定する")]
    private float scrollLimit = ConstDecimal.ZERO;
    // カウントダウン
    private int countDown = ConstInteger.ZERO;
    // 通過確認
    private bool isPassed = false;
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
        // いずれかのボタンが押された
        IsPressedAnyButton();

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
        isPassed = false;
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
    // remarks : Zキー（デバッグ用）
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedStartButton()
    {

        if (isAnyPressed)
        {
            // UIの表示状態の取得
            var isDisplay = slideObject.GetObjectState();

            if (isDisplay)
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
    // remarks : ステージセレクトシーンに遷移
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

        // スクロール
        creditBoard.transform.position += new Vector3(ConstDecimal.ZERO, speed, ConstDecimal.ZERO);

        var positionY = creditBoard.transform.position.y;

        if (!isPassed && positionY > scrollLimit)
        {
            // 通過確認
            isPassed = true;

            // 遷移処理
            Transition();
        }
    }
}
