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
    // 死亡演出情報
    [System.Serializable]
    public struct DeathAnimationInfo
    {
        // アニメーションの状態
        public enum DeathAnimatonState
        {
            Death,
            Appear,
            Move,
            Burst,
            Repair,
        }
        // 泡オブジェクト
        public GameObject bubble;
        // 故障アニメーションにかかる時間
        public float deathAnimationTime;
        // 泡の出現にかかる時間
        public float bubbleAppearTime;
        // 泡の移動にかかる時間
        public float bubbleMoveTime;
        // 泡の割れる演出にかかる時間
        public float bubbleBurstTime;
        // 修理アニメーションにかかる時間
        public float repairAnimationTime;
    }
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // アニメーター
    private Animator animator = null;
    // プレイヤー
    private PlayerController player = null;

    // 死亡アニメーションの情報
    [SerializeField]
    private DeathAnimationInfo deathAnimationInfo;
    // 死亡アニメーションの状態
    private DeathAnimationInfo.DeathAnimatonState deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Death;
    // プレイヤーが死亡したかどうか
    private bool playerIsDead = false;
    // 死亡アニメーション用タイマー
    private float deathAnimationTimer = 0;

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
        animator.SetInteger("LastDirection", Data.playerDir);
        if (player.JumpTiming())
        {
            animator.SetTrigger("Jump");
        }
        animator.SetBool("IsGround", player.IsGround());
        
        if(player.IsDead())
        {
            DeathUpdate();
        }
    }

    //------------------------------------------------------------------------------------------
    // 死亡時処理
    //------------------------------------------------------------------------------------------
    private void DeathUpdate()
    {
        if (!playerIsDead)
        {
            deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Death;
            playerIsDead = true;
            deathAnimationTimer = 0;
            animator.SetInteger("DeathState", 1);
        }

        deathAnimationTimer += Time.deltaTime;
        switch (deathAnimationState)
        {
            case DeathAnimationInfo.DeathAnimatonState.Death:
                if(deathAnimationTimer >= deathAnimationInfo.deathAnimationTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Appear;
                    deathAnimationTimer = 0;
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Appear:
                if (deathAnimationTimer >= deathAnimationInfo.bubbleAppearTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Move;
                    deathAnimationTimer = 0;
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Move:
                if (deathAnimationTimer >= deathAnimationInfo.bubbleMoveTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Burst;
                    deathAnimationTimer = 0;
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Burst:
                if (deathAnimationTimer >= deathAnimationInfo.bubbleBurstTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Repair;
                    animator.SetInteger("DeathState", 2);
                    deathAnimationTimer = 0;
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Repair:
                if (deathAnimationTimer >= deathAnimationInfo.repairAnimationTime)
                {
                    playerIsDead = false;
                    player.Repair();
                    animator.SetInteger("DeathState", 0);
                }
                break;
            default:
                break;
        }
    }
}
