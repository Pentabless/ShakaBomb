//==============================================================================================
/// File Name	: PlayerAnimator.cs
/// Summary		: プレイヤーのアニメーション制御
//==============================================================================================
using UnityEngine;
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

    // アニメーション速度を一時的に保存するための変数
    private float tempSpeed = 0;

    // 死亡アニメーションの情報
    [SerializeField]
    private DeathAnimationInfo deathAnimationInfo;
    // 死亡アニメーションの状態
    private DeathAnimationInfo.DeathAnimatonState deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Death;
    // プレイヤーが死亡したかどうか
    private bool playerIsDead = false;
    // 死亡アニメーション用タイマー
    private float deathAnimationTimer = 0;

    // 死亡位置
    private Vector3 deathPos = Vector3.zero;



    //------------------------------------------------------------------------------------------
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(ConstPlayer.NAME).GetComponent<PlayerController>();
        GameObject.Find(PauseManager.NAME).GetComponent<PauseManager>().AddIgnoreObject(gameObject);
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        animator.SetInteger("Direction", player.GetCurrentDir());
        animator.SetInteger("LastDirection", Data.playerDir);
        animator.SetBool("Jump", player.JumpTiming());
        animator.SetBool("IsGround", player.IsGround());
        animator.SetFloat("VelocityY", Data.prePlayerVel.y);

        if (player.AutoMoveFinished())
        {
            animator.SetBool("Goal", true);
        }

        if (player.IsDead())
        {
            DeathUpdate();
        }

        if(Data.time <= 0)
        {
            animator.SetBool("TimeUp", true);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 死亡時処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void DeathUpdate()
    {
        if (!playerIsDead)
        {
            if (Data.time <= 0)
            {
                return;
            }

            deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Death;
            playerIsDead = true;
            deathAnimationTimer = 0;
            deathAnimationInfo.bubble.SetActive(false);
            animator.SetInteger("DeathState", 1);
        }

        deathAnimationTimer += Time.deltaTime;
        switch (deathAnimationState)
        {
            case DeathAnimationInfo.DeathAnimatonState.Death:
                if (deathAnimationTimer >= deathAnimationInfo.deathAnimationTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Appear;
                    deathAnimationTimer = 0;
                    deathAnimationInfo.bubble.SetActive(true);
                    deathAnimationInfo.bubble.transform.localScale = new Vector3(0, 0, 1);
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Appear:
                float t2 = Mathf.Min(deathAnimationTimer / deathAnimationInfo.bubbleAppearTime, 1);
                t2 *= 2;
                deathAnimationInfo.bubble.transform.localScale = new Vector3(3 * t2, 3 * t2, 1);
                if (deathAnimationTimer >= deathAnimationInfo.bubbleAppearTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Move;
                    deathAnimationTimer = 0;
                    deathPos = player.transform.position;
                }
                break;
            case DeathAnimationInfo.DeathAnimatonState.Move:
                float t3 = Mathf.Min(deathAnimationTimer / deathAnimationInfo.bubbleMoveTime, 1);
                t3 = t3 * (2 - t3);
                player.transform.position = Vector3.Lerp(deathPos, Data.initialPlayerPos + Vector3.up * 0.1f, t3);
                if (deathAnimationTimer >= deathAnimationInfo.bubbleMoveTime)
                {
                    deathAnimationState = DeathAnimationInfo.DeathAnimatonState.Burst;
                    deathAnimationTimer = 0;
                    deathAnimationInfo.bubble.SetActive(false);
                    player.GetComponent<Rigidbody2D>().velocity *= 0.0f;
                    EffectGenerator.BubbleBurstFX(new BubbleBurstFX.Param(Color.white, Vector2.one * 3), Data.initialPlayerPos + Vector3.up * 0.1f);
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
                    if (Data.time > 0)
                    {
                        player.Repair();
                    }
                    animator.SetInteger("DeathState", 0);
                }
                break;
            default:
                break;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : アニメーションを停止
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void StopAnimation()
    {
        tempSpeed = animator.speed;
        animator.speed = 0;
    }



    //------------------------------------------------------------------------------------------
    // summary : アニメーションを再開する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void ResumeAnimation()
    {
        animator.speed = tempSpeed;
    }
}
