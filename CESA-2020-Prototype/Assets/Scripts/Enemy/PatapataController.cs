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
public class PatapataController : MonoBehaviour
{
	//------------------------------------------------------------------------------------------
    // member variable
	//------------------------------------------------------------------------------------------
    
    // パタパタのステータス
    enum Status
    {
        Fly,
        Hit,
        Dead,
    }

    [SerializeField]
    private float range;

    private Status currentStatus = Status.Fly;
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
        if(currentStatus == Status.Fly)
        {
            // なみなみの動き
            //this.transform.position = new Vector3(Mathf.Sin(Time.time * Mathf.PI / 180) * 100 + startPosition.x, Mathf.Sin(Time.time) * 5.0f + startPosition.y, startPosition.z);

            // 縦方向
            this.transform.position = new Vector3(startPosition.x, Mathf.Sin(Time.time) * range + startPosition.y, startPosition.z);
        }

        if(currentStatus == Status.Hit)
        {
            this.transform.position = new Vector3(startPosition.x, Mathf.Sin(Time.time) * Enemy.BALLON_MOVEMENT + startPosition.y, startPosition.z);
        }

        if (currentStatus == Status.Dead)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && currentStatus == Status.Hit)
            currentStatus = Status.Dead;

        if (collision.transform.tag == "Bullet")
        {
            startPosition = this.transform.position;
            currentStatus = Status.Hit;
        }
    }
}
