//==============================================================================================
/// File Name	: StageSelectPauseMenu.cs
/// Summary		: ステージセレクトシーンのポーズメニュー
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
//==============================================================================================
public class StageSelectPauseMenu : MonoBehaviour
{
    [SerializeField]
    private PauseManager pauseManager;      //ポーズ管理スクリプト
    [SerializeField]
    private GameObject[] choices;           //選択肢オブジェクト

    public AudioClip cursorSE;          //カーソル効果音
    public AudioClip decisionSE;        //決定効果音

    private int choice = 0;             //選択中の選択肢
    private float pressDelay = 0f;      //次の入力を受け付けるまでの時間
    private bool wasDecided = false;    //決定済みかどうか

    //初期化処理
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
        wasDecided = false;
    }

    private void Start()
    {
        Init();
    }
    
    private void Update()
    {
        //選択済みなら処理しない
        if (wasDecided)
        {
            return;
        }

        //入力状態受け取り
        float axis = Input.GetAxisRaw("Vertical");
        float axis2 = Input.GetAxisRaw("Vertical2");
        bool pressUp = (Input.GetKey(KeyCode.UpArrow) || axis > 0.0f || axis2 > 0.0f);
        bool pressDown = (Input.GetKey(KeyCode.DownArrow) || axis < 0.0f || axis2 < 0.0f);


        //連続入力制御
        if (pressDelay > 0f)
        {
            pressDelay -= Time.deltaTime;
        }

        //カーソル移動
        if (pressDelay <= 0f)
        {
            int input = (pressDown ? 1 : pressUp ? choices.Length - 1 : 0);
            int newChoice = (choice + input) % choices.Length;

            // カーソル移動があった場合に処理する
            if (newChoice != choice)
            {
                SoundPlayer.Play(cursorSE, 0.5f);
                choices[choice].GetComponent<Text>().color = new Color(1f, 1f, 1f);
                choice = newChoice;
                choices[choice].GetComponent<Text>().color = new Color(1f, 0.8f, 0f);
                pressDelay = 0.25f;
            }
        }

        //ステージ選択
        bool pressSubmit = Input.GetButtonDown("Submit");
        if (pressSubmit)
        {
            wasDecided = true;
            SoundPlayer.Play(decisionSE, 0.5f);

            switch (choice)
            {
                //プレイに戻る
                case 0:
                    Init();
                    pauseManager.ChangePauseState();
                    break;
                //タイトルに戻る
                case 1:
                    FadeManager.fadeColor = Color.black;
                    FadeManager.FadeOut("TitleScene");
                    break;
                default:
                    FadeManager.fadeColor = Color.black;
                    Debug.Log("PauseMenu:SelectError");
                    break;
            }
        }
    }
}
