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
    HitBubble hitBubble;

    protected bool onDestroy = false;

    protected void DestructionConfirmation()
    {
        if (bubble && hitBubble.burst)
        {
            GameObject.Find(Bubble.GENERATOR).GetComponent<BubbleGenerator>().BubbleCreate(transform.position, 4, false);
            currentStatus = Status.Dead;

            Destroy(bubble.gameObject);
            onDestroy = true;
        }

        if (onDestroy)
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
        if (collision.transform.tag == "Bullet")
        {
            bubble = Instantiate(bubblePre);
            bubble.transform.parent = transform;
            bubble.transform.localPosition *= 0;
            hitBubble = bubble.GetComponent<HitBubble>();
            StartCoroutine(BubbleAppear());
            bubble.transform.tag = Enemy.HIT_STATE;
            this.transform.tag = Enemy.HIT_STATE;
            currentStatus = Status.Hit;
        }
    }

    protected IEnumerator BubbleAppear()
    {
        var defaultSize = bubble.transform.localScale;
        bubble.transform.localScale *= 0;
        int time = (int)(0.15f * 60);
        int offset = (int)(0.4f * time / (1 - 0.4));
        for(int i = offset; i <= time+offset; i++)
        {
            float t = i / (float)(time + offset);
            t = t * (2 - t);
            bubble.transform.localScale = defaultSize * t;
            yield return null;
        }
        yield return null;
    }
}
