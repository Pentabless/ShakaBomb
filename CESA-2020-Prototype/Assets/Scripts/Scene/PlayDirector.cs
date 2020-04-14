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
        Result,     // リザルト演出
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private GameObject player;
    private PlayerController playerController;

    [SerializeField]
    private List<GameObject> uiObjects; // イベント時にフェードアウトを適用させるオブジェクト

    private PlayState state;
    private float waitTime = 0.0f;

    // ポーズ可能かどうか
    public bool canPause { get; private set; } = false;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        state = PlayState.Start;
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(Player.NAME);
        playerController = player.GetComponent<PlayerController>();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        // ステートで処理を分岐する
        switch (state)
        {
            case PlayState.Start:
                UpdateStart();
                break;
            case PlayState.Playing:
                UpdatePlaying();
                break;
            case PlayState.Goal:
                UpdateGoal();
                break;
            case PlayState.Result:
                UpdateResult();
                break;
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
        state = PlayState.Playing;
        canPause = true;
    }

    //------------------------------------------------------------------------------------------
    // プレイ中処理
    //------------------------------------------------------------------------------------------
    private void UpdatePlaying()
    {

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
    // リザルト処理
    //------------------------------------------------------------------------------------------
    private void UpdateResult()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            FadeManager.FadeOut("ResultScene");
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
            for(int j = 0; j < texts.Count; j++)
            {
                Color color = textColor[j];
                color.a *= alpha;
                texts[j].color = color;
            }
            for (int j = 0; j < images.Count; j++)
            {
                Color color = imageColor[j];
                color.a *= alpha;
                images[j].color = color;
            }
           
            yield return null;
        }
    }
}
