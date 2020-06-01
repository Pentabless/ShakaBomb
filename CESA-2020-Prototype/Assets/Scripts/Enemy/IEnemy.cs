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

    virtual protected void DestructionConfirmation()
    {
        if(currentStatus == Status.Dead)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 当たり判定時に呼ぶ
    /// </summary>
    /// <param name="collision"></param>
    virtual protected void OnCollisionEnterEvent(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && currentStatus == Status.Hit)
        {
            currentStatus = Status.Dead;

            Vector2 effectSize = Vector2.one * 1.5f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                transform.position,
                null);
        }

        if (collision.transform.tag == "Bullet")
        {
            this.transform.tag = Enemy.HIT_STATE;
            currentStatus = Status.Hit;
        }
    }
}
