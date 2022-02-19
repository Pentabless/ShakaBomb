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
public class MoveFloor : Floor
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    float speed;

    bool moveFlag = false;
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
        // 現在の位置
        var percentage = ((Time.time - reStartTime) * speed) / distance;

        if (!moveFlag)
        {
            this.transform.position = Vector3.Lerp(startPosition, endPosition, percentage);
            if (CheckMove(this.transform.position, endPosition))
            {
                reStartTime = Time.time;
                moveFlag = true;
            }
        }
        else
        {
            this.transform.position = Vector3.Lerp(endPosition, startPosition, percentage);
            if (CheckMove(this.transform.position, startPosition))
            {
                reStartTime = Time.time;
                moveFlag = false;
            }
        }

        Pass();
    }
}
