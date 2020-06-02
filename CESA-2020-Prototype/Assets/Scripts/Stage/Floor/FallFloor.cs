//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class FallFloor : Floor
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    enum FallState
    {
        Stay,
        Up,
        Down
    }

    FallState currentState = FallState.Stay;

    [SerializeField]
    float waitCount = 0.0f;
    [SerializeField]
    float fallSpeed;
    [SerializeField]
    float comebackSpeed;

    float count = 0.0f;
    float reStartTime = 0.0f;
    float distance;


    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (startPosition == Vector3.zero)
            startPosition = this.transform.position;

        distance = Vector3.Distance(startPosition, endPosition);

        thisCollider = GetComponent<BoxCollider2D>();
        platform = GetComponent<PlatformEffector2D>();
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

        if (currentState == FallState.Stay && this.transform.childCount != 0)
        {
            // 時間経過を見る
            count += Time.deltaTime;
            if (count >= waitCount)
            {
                reStartTime = Time.time;
                count = 0.0f;
                currentState = FallState.Down;
            }
        }

        // 時間が経ったら落下開始
        if (currentState == FallState.Down)
        {
            // 現在の位置
            var percentage = ((Time.time - reStartTime) * fallSpeed) / distance;

            this.transform.position = Vector3.Lerp(startPosition, endPosition, percentage);
            if (CheckMove(this.transform.position, endPosition))
            {
                currentState = FallState.Up;
            }
        }

        // 落下後上昇
        if (currentState == FallState.Up)
        {
            // 現在の位置
            var percentage = ((Time.time - reStartTime) * comebackSpeed) / distance;

            this.transform.position = Vector3.Lerp(endPosition, startPosition, percentage);
            if (CheckMove(this.transform.position, startPosition))
            {
                currentState = FallState.Stay;
            }

        }
        Pass();
    }
}
