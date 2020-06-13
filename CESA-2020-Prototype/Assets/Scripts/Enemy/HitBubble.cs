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
public class HitBubble : MonoBehaviour
{
    public bool burst { private set; get; } = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && !burst)
        {
            Vector2 effectSize = Vector2.one * 1.5f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                transform.position,
                null);
            burst = true;
            GameObject.Find(Common.Camera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.08f, 0.4f);
        }
        if (collision.transform.tag == "Bullet" && !burst)
        {
            Vector2 effectSize = Vector2.one * 3.5f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                transform.position,
                null);
            burst = true;
            GameObject.Find(Common.Camera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.08f, 0.4f);
        }
    }
}
