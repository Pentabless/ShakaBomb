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
public class RotationFloor : Floor
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    float speed;
    [SerializeField]
    float radius;
    [SerializeField]
    bool rightRotation;
    [SerializeField]
    bool leftRotation;

    bool direction;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (startPosition == Vector3.zero)
            startPosition = this.transform.position;

        direction = 
            rightRotation == leftRotation ? 
            true : rightRotation == true ? 
            true : false;

        thisCollider = GetComponent<BoxCollider2D>();
        platform = GetComponent<PlatformEffector2D>();

    }


    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        var x = radius * Mathf.Sin(Time.time * (direction ? speed : speed * -1)) + startPosition.x;
        var y = radius * Mathf.Cos(Time.time * (direction ? speed : speed * -1)) + startPosition.y;
        var z = Common.Decimal.ZERO;

        this.transform.position = new Vector3(x, y, z);

        Pass();
    }
}
