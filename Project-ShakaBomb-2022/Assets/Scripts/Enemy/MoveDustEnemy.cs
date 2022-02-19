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
public class MoveDustEnemy : IEnemy
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------

    //範囲
    public float left_move_range;
    public float right_move_range;
    //スピード    
    public float move_speed;


    //アニメーター
    private Animator animator;
    //当たり判定
    private BoxCollider2D collision;
    //初期の当たり判定のサイズ
    private float start_collision_size;
    //初期の当たり判定のオフセット
    private float start_collision_offset;
    //初期の座標
    private Vector3 start_position;

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
        collision = GetComponent<BoxCollider2D>();

        start_collision_size = collision.size.y;
        start_collision_offset = collision.offset.y;
        start_position = transform.position;
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
            //左向きの時
            if (transform.localScale.x == -1.0f)
            {
                //範囲の左側から出たら
                if (transform.position.x <= start_position.x - left_move_range)
                {
                    //向きを変える
                    transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                }
            }
            //右向きの時
            else if (transform.localScale.x == 1.0f)
            {
                //範囲の右側から出たら
                if (transform.position.x >= start_position.x + right_move_range)
                {
                    //向きを変える
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
            }
            //移動させる
            transform.position += new Vector3(transform.localScale.x * move_speed * (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (animate_rate * 180.0f)))), 0.0f);

            //オフセットを更新する
            collision.offset = new Vector2(0.0f, start_collision_offset - (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (animate_rate * 180.0f)))) * 0.2f);
            //当たり判定の範囲を更新する
            collision.size = new Vector2(3.0f, start_collision_size - (Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (animate_rate * 180.0f)))) * 0.5f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterEvent(collision);
    }

}
