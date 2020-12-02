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
    public enum FailedType
    {
        TimeUp,
        Fall,
    }


    private enum FailedState
    {
        Start,      // 起動中
        Select,     // 選択中
        Finish,     // 終了
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private Canvas failedCanvas = null;               // キャンバス
    private CanvasGroup canvasGroup = null;           // キャンバスグループ

    [SerializeField]
    private Text typeText = null;                     // 種類を表すテキスト
    private bool enableFrame = false;                 // 有効かどうか
    private FailedType type = FailedType.TimeUp;      // 失敗の種類
    private FailedState state = FailedState.Start;    // 状態
    private float waitTime = 0.0f;                    // 待ち時間

    [SerializeField]
    private GameObject[] choices;                     // 選択肢オブジェクト

    public AudioClip cursorSE;                        // カーソル効果音
    public AudioClip decisionSE;                      // 決定効果音

    private int choice = 0;                           // 選択中の選択肢
    private float pressDelay = 0f;                    // 次の入力を受け付けるまでの時間



    //------------------------------------------------------------------------------------------
    // summary : 初期化
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
            case FailedState.Start:  UpdateStart();  break;
            case FailedState.Select: UpdateSelect(); break;
            case FailedState.Finish: UpdateFinish(); break;
            default:
                Debug.Log("FailedFrameController:StateError");
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
            state = FailedState.Select;
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
            state = FailedState.Finish;
            SoundPlayer.Play(decisionSE);

            switch (choice)
            {
                // リトライ
                case 0:
                    FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
                    break;
                // ステージ選択
                case 1:
                    FadeManager.FadeOut("NewStageSelectScene");
                    break;
                default:
                    Debug.Log("FailedFrameController:SelectError");
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
        if (type == FailedType.TimeUp)
        {
            typeText.text = "Time Up";
        }
        else if (type == FailedType.Fall)
        {
            typeText.text = "Fall";
        }
    }
}
