//==============================================================================================
/// File Name	: PlayerAnimator.cs
/// Summary		: プレイヤーのアニメーション制御
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class PlayerAnimator : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // アニメーター
    private Animator animator;
    // プレイヤー
    private PlayerController player;


	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(Player.NAME).GetComponent<PlayerController>();
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        animator.SetInteger("Direction", player.GetCurrentDir());
        if (player.JumpTiming())
        {
            animator.SetTrigger("Jump");
        }
        animator.SetBool("IsGround", player.IsGround());
        animator.SetBool("IsDead", player.IsDead());
    }
}
