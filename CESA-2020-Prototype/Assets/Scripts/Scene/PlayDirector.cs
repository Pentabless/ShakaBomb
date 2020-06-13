//==============================================================================================
/// File Name	: PlayDirector.cs
/// Summary		: プレイシーンの進行管理
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
//==============================================================================================
public class PlayDirector : MonoBehaviour
{
    public const string NAME = "PlayDirector";

    // プレイシーンの状態
    enum PlayState
    {
        Start,      // 開始演出
        Playing,    // プレイ中
        Goal,       // ゴール演出
        Failed,     // クリア失敗演出
        Result,     // リザルト演出

        TutorialGoal,       // チュートリアル専用ゴール演出
        TutorialResult,     // チュートリアル専用リザルト演出
    }

    // タイムデータ
    [System.Serializable]
    public struct TimeData
    {
        public float timeLimit; // 制限時間
        public float star3Time; // 星3のタイム
        public float star2Time; // 星2のタイム
        public float star1Time; // 星1のタイム
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // ステージのタイムデータ
    [SerializeField]
    private TimeData timeData;
    // 残り時間
    [SerializeField]
    private float time = 0.0f;

    // イベント時にフェードアウトを適用させるオブジェクト
    [SerializeField]
    private List<GameObject> uiObjects;

    // プレイヤー
    private GameObject player;
    private PlayerController playerController;

    // ワイプエフェクト
    private WipeCamera wipeCamera;
    // ポーズマネージャー
    private PauseManager pauseManager;
    // クリア失敗用フレーム
    private FailedFrameController failedFrameController;

    // ゲームの進行状況
    private PlayState state;
    // 待ち時間用タイマー
    private float waitTime = 0.0f;

    // ポーズ可能かどうか
    public bool canPause { get; private set; } = false;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        state = PlayState.Start;
        Data.time = time = timeData.timeLimit;
        Data.timeLimit = timeData.timeLimit;
        Data.star_num = 0;
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(Player.NAME);
        playerController = player.GetComponent<PlayerController>();
        GameObject go = GameObject.Find(PauseManager.NAME);
        if (!go)
        {
            Debug.Log("PauseManagerをシーンに追加してください");
        }
        pauseManager = go.GetComponent<PauseManager>();
        go = GameObject.Find(FailedFrame.NAME);
        if (!go)
        {
            Debug.Log("FailedFrameをシーンに追加してください\nPauseManagerのignoreObjectsにFailedFrameを追加してください");
        }
        failedFrameController = go.GetComponent<FailedFrameController>();

        // シーン開始時にフェードインする
        FadeManager.FadeIn(0.01f);
        wipeCamera = GameObject.Find(Common.Camera.MAIN_CAMERA).GetComponent<WipeCamera>();
        wipeCamera.StartFadeIn(player.transform.position, 1.0f);
        waitTime = 1.0f;

    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        // ステートで処理を分岐する
        switch (state)
        {
            case PlayState.Start:   UpdateStart();      break;
            case PlayState.Playing: UpdatePlaying();    break;
            case PlayState.Goal:    UpdateGoal();       break;
            case PlayState.Failed:  UpdateFailed();     break;
            case PlayState.Result:  UpdateResult();     break;

            case PlayState.TutorialGoal: UpdateTutorialGoal(); break;
            case PlayState.TutorialResult: UpdateTutorialResult(); break;
            default:
                Debug.Log("PlayDirecotr:StateError");
                break;
        }
    }

    //------------------------------------------------------------------------------------------
    // スタート中処理
    //------------------------------------------------------------------------------------------
    private void UpdateStart()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.Playing;
            canPause = true;
            GameObject go = GameObject.Find(AreaNameScript.NAME);
            if (go && Data.stage_number >= 0)
            {
                string text = "第" + (Data.stage_number + 1) + "区画";
                go.GetComponent<AreaNameScript>().ShowAreaName(text, 1.0f, 2.0f);
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // プレイ中処理
    //------------------------------------------------------------------------------------------
    private void UpdatePlaying()
    {
        time -= Time.deltaTime;

        // 時間切れになったらクリア失敗になる
        if (time <= 0.0f)
        {
            time = 0.0f;
            TimeUp();
        }

        Data.time = time;


    }

    //------------------------------------------------------------------------------------------
    // ゴール処理
    //------------------------------------------------------------------------------------------
    private void UpdateGoal()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.Result;
            waitTime = 0.0f;
        }
    }

    //------------------------------------------------------------------------------------------
    // チュートリアルゴール処理
    //------------------------------------------------------------------------------------------
    private void UpdateTutorialGoal()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.TutorialResult;
            waitTime = 0.0f;
        }
    }
    //------------------------------------------------------------------------------------------
    // クリア失敗処理
    //------------------------------------------------------------------------------------------
    private void UpdateFailed()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            waitTime = 999.0f;
            failedFrameController.EnableFrame(FailedFrameController.FailedType.TimeUp);
            pauseManager.ResetFilterColor();
            pauseManager.Pause(1.0f);
        }
    }

    //------------------------------------------------------------------------------------------
    // リザルト処理
    //------------------------------------------------------------------------------------------
    private void UpdateResult()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            FadeManager.fadeColor = Color.clear;
            FadeManager.FadeOut("NewResultScene", 1.5f);
            wipeCamera.StartFadeOut(player.transform.position, 1.4f);
            //SceneEffecterController.StartEffect();
            waitTime = 99.0f;
        }
    }

    //------------------------------------------------------------------------------------------
    // チュートリアルリザルト処理
    //------------------------------------------------------------------------------------------
    private void UpdateTutorialResult()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            FadeManager.fadeColor = Color.clear;
            FadeManager.FadeOut("NewStageSelectScene", 1.5f);
            wipeCamera.StartFadeOut(player.transform.position, 1.4f);
            //SceneEffecterController.StartEffect();
            waitTime = 99.0f;
        }
    }

    //------------------------------------------------------------------------------------------
    // ゴールイベント
    //------------------------------------------------------------------------------------------
    public void Goal()
    {
        state = PlayState.Goal;
        waitTime = 2.0f;
        canPause = false;

        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // プレイヤーをゴールの位置に移動させる
        playerController.EnableAutoControl(true);
        playerController.SetTargetPos(GameObject.Find(Common.Goal.NAME).transform.position);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());
    }

    //------------------------------------------------------------------------------------------
    // チュートリアルゴールイベント
    //------------------------------------------------------------------------------------------
    public void TutorialGoal()
    {
        state = PlayState.TutorialGoal;
        waitTime = 2.0f;
        canPause = false;

        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // プレイヤーをゴールの位置に移動させる
        playerController.EnableAutoControl(true);
        playerController.SetTargetPos(GameObject.Find(Common.Goal.NAME).transform.position);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());
    }


    //------------------------------------------------------------------------------------------
    // タイムアップイベント
    //------------------------------------------------------------------------------------------
    public void TimeUp()
    {
        state = PlayState.Failed;
        waitTime = 3.0f;
        canPause = false;

        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());
    }


    //------------------------------------------------------------------------------------------
    // UIのフェードアウト処理
    //------------------------------------------------------------------------------------------
    private IEnumerator FadeOutUICoroutine()
    {
        var texts = Utility.GetAllComponents<Text>(uiObjects, true);
        var images = Utility.GetAllComponents<Image>(uiObjects, true);

        var textColor = Utility.GetColors(texts);
        var imageColor = Utility.GetColors(images);

        for (int i = 1; i <= 90; ++i)
        {
            float alpha = (90 - i) / 90.0f;
            Color color;
            for(int j = 0; j < texts.Count; j++)
            {
                color = textColor[j];
                color.a *= alpha;
                texts[j].color = color;
            }
            for (int j = 0; j < images.Count; j++)
            {
                color = imageColor[j];
                color.a *= alpha;
                images[j].color = color;
            }
           
            yield return null;
        }
    }
}
