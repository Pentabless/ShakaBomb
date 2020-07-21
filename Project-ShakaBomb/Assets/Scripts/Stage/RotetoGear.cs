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
public class RotetoGear : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 速度
    [SerializeField]
    float speed;
    [SerializeField]
    bool right;
    [SerializeField]
    bool left;

    [SerializeField]
    List<GameObject> floors;

    bool direction;
	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        direction =
            right == left ?
            true : left == true ?
            true : false;

    }


    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, direction? +speed:-speed));

        foreach(var obj in floors)
        {
            obj.transform.Rotate(new Vector3(0, 0, direction ? -speed : +speed));
        }
    }
}
