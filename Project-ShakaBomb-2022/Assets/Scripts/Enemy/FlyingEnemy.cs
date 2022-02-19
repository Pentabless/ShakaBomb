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
public class FlyingEnemy : IEnemy
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private float range;
    [SerializeField]
    private bool vertical;
    [SerializeField]
    private bool horizontal;

    private Vector3 startPosition;

    private float timer = 0;

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
        DestructionConfirmation();

        if (currentStatus == Status.None)
        {
            timer += Time.deltaTime;
            if(vertical == horizontal)
            {
                // 縦方向
                this.transform.position = new Vector3(startPosition.x, Mathf.Sin(timer) * range + startPosition.y, startPosition.z);
            }
            else if(vertical)
            {
                // 縦方向
                this.transform.position = new Vector3(startPosition.x, Mathf.Sin(timer) * range + startPosition.y, startPosition.z);
            }
            else
            {
                // 横方向
                this.transform.position = new Vector3(Mathf.Sin(timer) * range + startPosition.x, startPosition.y, startPosition.z);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }
}
