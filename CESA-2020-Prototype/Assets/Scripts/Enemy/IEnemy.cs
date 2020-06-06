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
public class IEnemy : MonoBehaviour
{
	//------------------------------------------------------------------------------------------
    // member variable
	//------------------------------------------------------------------------------------------
    public enum Status
    {
        None,
        Hit,
        Dead,
    }

    public Status currentStatus = Status.None;

    [SerializeField]
    GameObject bubblePre;
    GameObject bubble;

    protected bool onDestroy = false;

    protected void DestructionConfirmation()
    {
        if(bubble != null)
        {
            bubble.transform.position = this.transform.position;
        }
        else if(onDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 当たり判定時に呼ぶ
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionEnterEvent(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && currentStatus == Status.Hit)
        {
            currentStatus = Status.Dead;
        }

        if (collision.transform.tag == "Bullet")
        {
            bubble = Instantiate(bubblePre);
            bubble.transform.position = this.transform.position;
            onDestroy = true;
            bubble.transform.tag = Enemy.HIT_STATE;
            this.transform.tag = Enemy.HIT_STATE;
            currentStatus = Status.Hit;
        }
    }
}
