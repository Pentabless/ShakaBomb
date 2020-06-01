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
public class PatapataController : IEnemy
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------


    [SerializeField]
    private float range;

    private Vector3 startPosition;
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
        startPosition = this.transform.position;
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (currentStatus == Status.None)
        {
            // なみなみの動き
            //this.transform.position = new Vector3(Mathf.Sin(Time.time * Mathf.PI / 180) * 100 + startPosition.x, Mathf.Sin(Time.time) * 5.0f + startPosition.y, startPosition.z);

            // 縦方向
            this.transform.position = new Vector3(startPosition.x, Mathf.Sin(Time.time) * range + startPosition.y, startPosition.z);
        }

        //if (currentStatus == Status.Hit)
        //{
        //    this.transform.position = new Vector3(startPosition.x, Mathf.Sin(Time.time) * Enemy.BALLON_MOVEMENT + startPosition.y, startPosition.z);
        //}

        if (currentStatus == Status.Dead)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }
}
