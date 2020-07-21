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
public class StayDustEnemy : IEnemy
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    //アニメーター
    private Animator animator;
    //当たり判定
    private CircleCollider2D collision;
    //初期の当たり判定の半径
    private float start_collision_radius;
    //初期の当たり判定のオフセット
    private float start_collision_offset;

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
        //コンポーネント
        animator = GetComponent<Animator>();
        collision = GetComponent<CircleCollider2D>();

        start_collision_radius = collision.radius;
        start_collision_offset = collision.offset.y;
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        DestructionConfirmation();

        if (currentStatus == Status.None)
        {
            //アニメーションと連動して当たり判定を拡大縮小する
            float animate_rate = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            //オフセットを更新する
            collision.offset = new Vector2(0.0f,start_collision_offset + (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (animate_rate * 180.0f)))) * 0.25f);
            //当たり判定の範囲を更新する
            collision.radius = start_collision_radius + (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (animate_rate * 180.0f)))) * 0.25f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }
}
