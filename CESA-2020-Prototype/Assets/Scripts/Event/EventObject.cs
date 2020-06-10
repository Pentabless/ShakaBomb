//==============================================================================================
/// File Name	: EventObject.cs
/// Summary		: イベントオブジェクト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Common;
//==============================================================================================
public class EventObject : MonoBehaviour
{
    // 開始情報
    [System.Serializable]
    struct StartInfo
    {
        // 開始条件
        public enum Condition
        {
            EnterArea,  // エリア内侵入時有効
            Custom,     // カスタム
        }
        public Condition condition;
    }

    // 終了情報
    [System.Serializable]
    struct EndInfo
    {
        // 終了条件
        public enum Condition
        {
            ExitArea,   // エリア内侵入時有効
            Time,       // 一定時間経過
            Custom,     // カスタム
        }
        public Condition condition;
        public float time;
    }
   
    // 有効状態
    enum EnableState
    {
        False,  // 無効
        True,   // 有効
        End,    // 終了（無効）
    }

    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // 開始情報
    private StartInfo startInfo;
    [SerializeField]
    // 開始イベント
    private UnityEvent startEvents;

    [SerializeField]
    // 終了情報
    private EndInfo endInfo;
    [SerializeField]
    // 終了イベント
    private UnityEvent endEvents;

    // 何度も有効にするかどうか
    [SerializeField]
    private bool once = true;

    // 有効かどうか
    private EnableState enableState = EnableState.False;
    // 経過時間
    private float time = 0.0f;
    // 有効当たり判定数
    private int hitCount = 0;


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
        
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        if (enableState != EnableState.True)
        {
            return;
        }

        time += Time.deltaTime;

        if (endInfo.condition == EndInfo.Condition.Time && time >= endInfo.time)
        {
            EndEvent();
        }
    }

    //------------------------------------------------------------------------------------------
    // イベントの開始
    //------------------------------------------------------------------------------------------
    public void StartEvent()
    {
        enableState = EnableState.True;
        time = 0.0f;

        startEvents.Invoke();
    }

    //------------------------------------------------------------------------------------------
    // イベントの終了
    //------------------------------------------------------------------------------------------
    public void EndEvent()
    {
        enableState = (once ? EnableState.End : EnableState.False);

        endEvents.Invoke();
    }


    //------------------------------------------------------------------------------------------
    // OnTriggerEnter2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        hitCount++;

        if (enableState != EnableState.False)
        {
            return;
        }

        if (startInfo.condition == StartInfo.Condition.EnterArea)
        {
            StartEvent();
        }
    }

    //------------------------------------------------------------------------------------------
    // OnTriggerExit2D
    //------------------------------------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        hitCount--;

        if (enableState != EnableState.True || hitCount > 0)
        {
            return;
        }

        if (endInfo.condition == EndInfo.Condition.ExitArea)
        {
            EndEvent();
        }
    }
}
