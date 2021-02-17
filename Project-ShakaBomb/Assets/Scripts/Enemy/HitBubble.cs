//==============================================================================================
/// File Name	: HitBubble.cs
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

    [SerializeField]
    private AudioClip wrapSE = null;
    [SerializeField]
    private AudioClip burstSE = null;

    private void Awake()
    {
        SoundPlayer.Play(wrapSE, 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == ConstPlayer.NAME && !burst)
        {
            SoundPlayer.Play(burstSE);
            Vector2 effectSize = Vector2.one * 1.5f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                transform.position,
                null);
            burst = true;
            GameObject.Find(ConstCamera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.1f, 1.0f);
        }
        if (collision.transform.tag == "Bullet" && !burst)
        {
            SoundPlayer.Play(burstSE,0.7f);
            Vector2 effectSize = Vector2.one * 3.5f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                transform.position,
                null);
            burst = true;
            GameObject.Find(ConstCamera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.1f, 1.0f);
        }
    }
}
