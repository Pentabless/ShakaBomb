//==============================================================================================
/// File Name	: NewStageSelectDirecotr.cs
/// Summary		: ステージセレクト制御スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class NewStageSelectDirector : MonoBehaviour
{
    public static string NAME = "NewStageSelectDirecotr";

    // ステージセレクトシーンの状態
    private enum StageSelectState
    {
        Start,
        Playing,
        Decide,
        FadeOut,
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // プレイヤー
    private GameObject player = null;
    private PlayerController playerController = null;

    // ワイプエフェクト
    private WipeCamera wipeCamera = null;
    // ポーズマネージャー
    private PauseManager pauseManager = null;
    // ポーズ用フレーム
    //private FailedFrameController failedFrameController;

    // ゲームの進行状況
    private StageSelectState state = StageSelectState.Start;
    // 待ち時間用タイマー
    private float waitTime = 0.0f;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {

    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        Data.time = 100;

        player = GameObject.Find(Player.NAME);
        playerController = player.GetComponent<PlayerController>();

        // シーン開始時にフェードインする
        FadeManager.FadeIn(0.01f);
        wipeCamera = GameObject.Find(Common.Camera.MAIN_CAMERA).GetComponent<WipeCamera>();
        wipeCamera.StartFadeIn(player.transform.position, 1.5f);

    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        switch (state)
        {
            case StageSelectState.Start:
                state = StageSelectState.Playing;
                break;
            case StageSelectState.Playing:
                bool pressPause = (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7));
                if (pressPause)
                {
                    pauseManager.ChangePauseState();
                }
                break;
            case StageSelectState.Decide:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    state = StageSelectState.FadeOut;
                }
                break;
            case StageSelectState.FadeOut:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0.0f)
                {
                    FadeManager.fadeColor = Color.clear;
                    FadeManager.FadeOut("PlayScene", 1.5f);
                    wipeCamera.StartFadeOut(player.transform.position, 1.4f);
                    waitTime = 99.0f;
                }
                break;
            default:
                Debug.Log("StageSelectScene:StateError");
                break;
        }
    }

    //------------------------------------------------------------------------------------------
    // ステージの決定
    //------------------------------------------------------------------------------------------
    public void DecideStage()
    {
        state = StageSelectState.Decide;
        waitTime = 0.5f;

        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
    }
}
