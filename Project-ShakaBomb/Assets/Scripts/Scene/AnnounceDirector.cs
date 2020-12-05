//==============================================================================================
/// File Name	: AnnounceDirector.cs
/// Summary		: アナウンスシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class AnnounceDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 音声ファイル(SE)
    [SerializeField, Header("Pressed SE"), Tooltip("ボタンが押された際に再生したい音声ファイルをアタッチ")]
    private AudioClip pressedSE = null;
    // PressAnyButton(Text)
    [SerializeField, Header("PressAnyButton(Text)"), Tooltip("PressAnyButton(Text)をアタッチする")]
    private Text pressAnyButtonText = null;
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
    // summary : 遷移処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Transition()
    {
        // フェードアウトを開始
        FadeManager.FadeOut(ConstScene.TITLE, ConstScene.FADE_TIME);
    }



    //------------------------------------------------------------------------------------------
    // summary : いずれかのボタンが押されたが押された際に行う処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void IsPressedAnyButton()
    {
        if (!isPressed && Input.anyKey)
        {
            // 通過確認
            isPressed = true;

            // SEを再生
            SoundPlayer.Play(pressedSE);

            // ブリンク処理を停止する
            StopBlink();

            // 遷移処理
            Transition();
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
        pressAnyButtonText.GetComponent<Blink>().StopBlink();
    }
}
