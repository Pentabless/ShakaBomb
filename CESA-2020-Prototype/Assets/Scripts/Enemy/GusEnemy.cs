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
public class GusEnemy : IEnemy
{
    [SerializeField]
    float moveSpeed;

    Vector3 initializePos;
    float distance;
    float currentTime;

    public Vector3 arrivalPosition { get; set; }
    public float destroy_count { get; set; }

    private void Start()
    {
        initializePos = this.transform.position;
        distance = Vector3.Distance(initializePos, this.arrivalPosition);
        currentTime = Time.time;
    }
    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        DestructionConfirmation();

        if (currentStatus == Status.None)
        {
            var percentage = (Time.time - currentTime * moveSpeed) / distance;
            Debug.Log(percentage);
            this.transform.position = Vector3.Lerp(initializePos, arrivalPosition, percentage);
        }

        //消える時間になったら
        if (destroy_count <= (Time.time - currentTime))
        {
            //消えるアニメーションに切り替える
            GetComponent<Animator>().SetTrigger("DestroyTrigger");
        }

        //消えるアニメーションをしていたら
        if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "GusEnemyDesappearAnimationClip")
        {
            //アニメーションが終了したら
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                onDestroy = true;   //消える
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }
}
