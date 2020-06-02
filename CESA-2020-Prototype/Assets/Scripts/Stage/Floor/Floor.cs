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
public partial class Floor
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.parent = this.transform;
        }

        if(collision.transform.tag == "Player" || collision.transform.tag == "Balloon")
        {
            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            if (!rig)
            {
                rig = collision.gameObject.GetComponentInParent<Rigidbody2D>();
            }
           
            // 中で止まったら通り抜けるようにする
            if(Mathf.Abs(Data.currentPlayerVel.y) < 0.05f && Data.currentPlayerVel.y < Data.prePlayerVel.y)
            {
                passable = true;
            }
           
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.parent = null;
        }
    }


    /// <summary>
    /// ラープ完了したかどうか
    /// </summary>
    /// <param name="start">スタートポジション</param>
    /// <param name="end">エンドポジション</param>
    /// <returns>完了していればtrue</returns>
    protected bool CheckMove(Vector3 start, Vector3 end)
    {
        if (Mathf.Approximately(start.x, end.x) && Mathf.Approximately(start.y, end.y))
            return true;

        return false;
    }

    protected bool CheckDifferences(float x, float y, float differences)
    {
        float temp = Mathf.Abs(x) - Mathf.Abs(y);

        if (temp <= differences)
            return true;
        else
            return false;
    }

}