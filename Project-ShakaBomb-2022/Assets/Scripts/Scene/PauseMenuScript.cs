//==============================================================================================
/// File Name	: PauseMenuScript.cs（修正予定）
/// Summary		: ポーズメニュに関わるスクリプト
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Common;
//==============================================================================================
public class PauseMenuScript : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // ポーズ管理スクリプト
    [SerializeField]
    private PauseManager pauseManager = null;
    // 選択肢オブジェクト
    [SerializeField]
    private GameObject[] choices = null;
    // カーソル効果音
    public AudioClip cursorSE = null;
    // 決定効果音
    public AudioClip decisionSE = null;
    // 選択中の選択肢
    private int choice = 0;
    // 次の入力を受け付けるまでの時間
    private float pressDelay = 0f;
    // 決定済みかどうか
    private bool wasDecided = false;    



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
        // 選択済みなら処理しない
        if (wasDecided)
        {
            return;
        }

        // 入力状態受け取り
        float axis = Input.GetAxisRaw("Vertical");
        float axis2 = Input.GetAxisRaw("Vertical2");
        bool pressUp = (Input.GetKey(KeyCode.UpArrow) || axis > 0.0f || axis2 > 0.0f);
        bool pressDown = (Input.GetKey(KeyCode.DownArrow) || axis < 0.0f || axis2 < 0.0f);


        // 連続入力制御
        if (pressDelay > 0f)
        {
            pressDelay -= Time.deltaTime;
        }
        
        // カーソル移動
        if (pressDelay <= 0f)
        {
            int input = (pressDown ? 1 : pressUp ? choices.Length - 1 : 0);
            int newChoice = (choice + input)%choices.Length;

            // カーソル移動があった場合に処理する
            if (newChoice != choice)
            {
                SoundPlayer.Play(cursorSE);
                choices[choice].GetComponent<Text>().color = new Color(1f, 1f, 1f);
                choice = newChoice;
                choices[choice].GetComponent<Text>().color = new Color(1f, 0.8f, 0f);
                pressDelay = 0.25f;
            }
        }

        // ステージ選択
        bool pressSubmit = Input.GetButtonDown("Submit");
        if (pressSubmit)
        {
            wasDecided = true;
            SoundPlayer.Play(decisionSE);

            switch (choice)
            {
                // プレイに戻る
                case 0:
                    Init();
                    pauseManager.ChangePauseState();
                    break;
                // リトライ
                case 1:
                    FadeManager.fadeColor = Color.black;
                    FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
                    break;
                // ステージ選択
                case 2:
                    FadeManager.fadeColor = Color.black;
                    FadeManager.FadeOut(ConstScene.STAGE_SELECT);
                    break;
                default:
                    DebugLogger.Log("PauseMenu:SelectError");
                    break;
            }
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
        choice = 1;
        for (int i = 0; i < choices.Length; ++i)
        {
            if (i == choice)
            {
                choices[i].GetComponent<Text>().color = new Color(1f, 0.8f, 0f);
            }
            else
            {
                choices[i].GetComponent<Text>().color = new Color(1f, 1f, 1f);
            }
        }
        wasDecided = false;
    }
}