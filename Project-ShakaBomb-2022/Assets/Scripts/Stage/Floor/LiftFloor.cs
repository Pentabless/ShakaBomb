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
public class LiftFloor : Floor
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    enum LiftFloorStatus
    {
        Stay,
        Move,
        Return
    }

    LiftFloorStatus currentState = LiftFloorStatus.Stay;

    [SerializeField]
    float waitCount = 0.0f;
    [SerializeField]
    float speed;

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
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (currentState == LiftFloorStatus.Stay)
        {
            if (this.transform.childCount != 0)
            {
                count += Time.deltaTime;
                if (count >= waitCount)
                {
                    currentState = LiftFloorStatus.Move;
                    reStartTime = Time.time;
                    count = 0.0f;
                }
            }
        }

        if (currentState == LiftFloorStatus.Move)
        {
            var percentage = ((Time.time - reStartTime) * speed) / distance;
            Debug.Log(percentage);

            this.transform.position = Vector3.Lerp(startPosition, endPosition, percentage);
            if (CheckMove(this.transform.position, endPosition))
            {
                reStartTime = Time.time;
                currentState = LiftFloorStatus.Return;
            }
        }

        if (currentState == LiftFloorStatus.Return)
        {
            var percentage = ((Time.time - reStartTime) * speed) / distance;

            this.transform.position = Vector3.Lerp(endPosition, startPosition, percentage);
            if (CheckMove(this.transform.position, startPosition))
            {
                currentState = LiftFloorStatus.Stay;
            }
        }

        Pass();
    }
}
