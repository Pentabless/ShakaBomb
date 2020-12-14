//==============================================================================================
/// File Name	: FailedFrameController.cs
/// Summary		: クリア失敗用フレーム
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Common;
//==============================================================================================
public class FailedFrameController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    public enum FailedType
    {
        TIME_UP,
        FALL,
    }

    private enum FailedState
    {
        START,      
        SELECT,     
        FINISH,     
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // キャンバス
    private Canvas failedCanvas = null;
    // キャンバスグループ
    private CanvasGroup canvasGroup = null;
    // 種類を表すテキスト
    [SerializeField]
    private Text typeText = null;
    // 有効かどうか
    private bool enableFrame = false;
    // 失敗の種類
    private FailedType type = FailedType.TIME_UP;
    // 状態
    private FailedState state = FailedState.START;
    // 待ち時間
    private float waitTime = 0.0f;
    // 選択肢オブジェクト
    [SerializeField]
    private GameObject[] choices;
    // カーソル効果音
    public AudioClip cursorSE;
    // 決定効果音
    public AudioClip decisionSE;
    // 選択中の選択肢
    private int choice = 0;
    // 次の入力を受け付けるまでの時間
    private float pressDelay = 0f;                    



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        choice = 0;
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
    }



    //------------------------------------------------------------------------------------------
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        Init();
    }



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        failedCanvas = GetComponentInChildren<Canvas>();
        canvasGroup = failedCanvas.GetComponent<CanvasGroup>();
        failedCanvas.enabled = false;
        canvasGroup.alpha = 0.0f;
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (!enableFrame)
        {
            return;
        }

        // ステートで処理を分岐する
        switch (state)
        {
            case FailedState.START:  UpdateStart();  break;
            case FailedState.SELECT: UpdateSelect(); break;
            case FailedState.FINISH: UpdateFinish(); break;
            default:
                DebugLogger.Log("FailedFrameController:StateError");
                break;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 起動中の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateStart()
    {
        waitTime -= Time.deltaTime;
        canvasGroup.alpha = 1.0f - Mathf.Max(waitTime, 0.0f) / ConstFailedFrame.FADE_TIME;
        if (waitTime <= 0.0f)
        {
            state = FailedState.SELECT;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 選択中の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateSelect()
    {
        // 入力状態受け取り
        float axis = Input.GetAxisRaw("Vertical");
        bool pressUp = (Input.GetKey(KeyCode.UpArrow) || axis > 0.0f);
        bool pressDown = (Input.GetKey(KeyCode.DownArrow) || axis < 0.0f);


        // 連続入力制御
        if (pressDelay > 0f)
        {
            pressDelay -= Time.deltaTime;
        }
        
        // カーソル移動
        if (pressDelay <= 0f)
        {
            int input = (pressDown ? 1 : pressUp ? choices.Length - 1 : 0);
            int newChoice = (choice + input) % choices.Length;

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

        // 選択
        bool pressSubmit = Input.GetButtonDown("Submit");
        if (pressSubmit)
        {
            state = FailedState.FINISH;
            SoundPlayer.Play(decisionSE);

            switch (choice)
            {
                // リトライ
                case 0:
                    FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
                    break;
                // ステージ選択
                case 1:
                    FadeManager.FadeOut(ConstScene.STAGE_SELECT);
                    break;
                default:
                    DebugLogger.Log("FailedFrameController:SelectError");
                    break;
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 終了中の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateFinish()
    {

    }



    //------------------------------------------------------------------------------------------
    // summary : オブジェクトを起動する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void EnableFrame(FailedType type)
    {
        enableFrame = true;
        failedCanvas.enabled = true;
        waitTime = ConstFailedFrame.FADE_TIME;
        this.type = type;
        if (type == FailedType.TIME_UP)
        {
            typeText.text = "Time Up";
        }
        else if (type == FailedType.FALL)
        {
            typeText.text = "Fall";
        }
    }
}
