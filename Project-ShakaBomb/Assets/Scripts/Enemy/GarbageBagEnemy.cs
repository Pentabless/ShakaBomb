//==============================================================================================
/// File Name	: GarbageBagEnemy.cs
/// Summary		: GarbageBagEnemyの制御スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class GarbageBagEnemy : IEnemy
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // 移動範囲
    private Vector2 moveRange = Vector2.zero;
    [SerializeField]
    // 横方向の移動速度
    private float horizontalSpeed = 1;
    [SerializeField]
    // 縦方向の移動速度
    private float verticalSpeed = 1;

    // 初期位置
    private Vector3 startPosition = Vector3.zero;
    // タイマー
    private float timer = 0;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        startPosition = transform.position;
    }
    
	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        timer += Time.deltaTime;

        DestructionConfirmation();

        if (currentStatus == Status.None)
        {
            float angle = Mathf.PI * 2 * timer;
            var offset = new Vector2(Mathf.Cos(angle * horizontalSpeed), Mathf.Sin(angle * verticalSpeed)) * moveRange;
            transform.position = startPosition + (Vector3)offset;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }
}
