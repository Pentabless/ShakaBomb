//==============================================================================================
/// File Name	: PlayerController.cs
/// Summary		: プレイヤー管理
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
//==============================================================================================
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
//==============================================================================================
public class PlayerController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // リジッドボディ
    Rigidbody2D rig;

    // バブルジェネレータ
    [SerializeField]
    GameObject bubbleG;

    // バルーンコントローラ
    [SerializeField]
    BackBalloonController balloonController = null;
    // 回転を無視するオブジェクト
    [SerializeField]
    GameObject antiRotationWrapper = null;

    // バレットジェネレータ
    [SerializeField]
    GameObject bulletG;

    // コントローラ
    [SerializeField]
    GameObject gamepadManager;

    bool checkController;
    // 入力を受け付けるかのフラグ
    bool canControl = true;

    // 所持しているバルーン
    private List<GameObject> m_balloonList = new List<GameObject>();

    // 移動力
    [SerializeField]
    float accelForce;
    [SerializeField]
    float maxSpeed;
    float playerSpeed;
    float stickSence;       // 入力感度

    // ジャンプ関連
    [SerializeField]
    float defaultJumpForce;
    float jumpForce;
    int jumpCount = 0;
    bool jumpStopFlag;

    // 切り替えし猶予フレーム
    [SerializeField]
    int turnCount;

    // 接地フラグ
    bool isGround;
    bool bubbleGround;

    // コヨーテタイム
    [SerializeField]
    int coyoteTime;

    bool coyoteFlag = true;
    int coyoteCount = Integer.ZERO;

    // 敵接触時
    int hitCount = Integer.ZERO;

    // 空中ブーストの強さ
    [SerializeField]
    Vector2 boostForce;
    // ブースト時のエフェクトの位置オフセット
    [SerializeField]
    Vector2 boostFXOffset;
    // ブーストの回数制限 着地でリセット
    int boostCount = 0;
    // エネミーを利用したブーストのフラグ
    bool isEnemyBoost = false;

    // プレイヤーの向き
    int dir;            // 現在
    int lastDir;      // 前フレーム
    int dirCount;       // 切り替えし用向き保持カウント

    // キー入力の情報保持
    float jumpButton = Decimal.ZERO;
    float jumpButtonTrigger = Decimal.ZERO;

    float attackButton = Decimal.ZERO;
    float attackButtonTrigger = Decimal.ZERO;

    float boostButton = Decimal.ZERO;
    float boostButtonTrigger = Decimal.ZERO;

    // 浮力が最高になるバルーンサイズ
    [SerializeField]
    float maxPowerBalloonSize = 2.5f;
    // バレットの発射コスト
    [SerializeField]
    float bulletCost = 1.0f;
    // ブースト移動コスト
    [SerializeField]
    float boostCost = 1.5f;

    // 敵死亡時のアニメーション時間
    [SerializeField]
    float animationTime;
    // 死亡経過
    float deathCount;
    // 死亡フラグ
    bool deathFlag = false;

    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        bubbleGround = false;
        checkController = false;
        dir = Integer.ZERO;
        lastDir = Integer.ZERO;
        dirCount = Integer.ZERO;
        jumpForce = defaultJumpForce;
        jumpCount = Integer.ZERO;
        jumpStopFlag = false;
        playerSpeed = accelForce;
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    void Update()
    {
        // 故障アニメーション
        DeathAnimation(deathFlag);

        if (!canControl)
        {
            dir = Integer.ZERO;
            return;
        }

        // プレイヤー操作系統 (入力が必要なもの)--------------------------------------------------
        // コントローラの接続チェック
        checkController = gamepadManager.GetComponent<GamepadManager>().GetCheckGamepad();

        // 左右移動
        if (Input.GetAxis(Player.HORIZONTAL) < Integer.ZERO)
        {
            dir = -1;
            dirCount = turnCount;
            stickSence = Input.GetAxis(Player.HORIZONTAL);
        }
        else if (Input.GetAxis(Player.HORIZONTAL) > Integer.ZERO)
        {
            dir = 1;
            dirCount = turnCount;
            stickSence = Input.GetAxis(Player.HORIZONTAL);
        }
        else
        {
            dir = 0;
        }



        //// テストコード
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //bulletG.EnableGuideLines(transform.position, (Data.playerDir > 0 ? 0 : Mathf.PI));
        //}
        //else
        //{
        //    bulletG.DisableGuideLines();
        //}

        ////ガイドライン(停止時非表示)
        //if (Mathf.Abs(Input.GetAxis(Player.HORIZONTAL)) <= 0.1f && Mathf.Abs(Input.GetAxis(Player.VERTICAL)) <= 0.1f)
        //{
        //    bulletG.DisableGuideLines();

        //}
        //else
        //{
        //    bulletG.EnableGuideLines(transform.position, Mathf.Atan2(Input.GetAxis(Player.VERTICAL), Input.GetAxis(Player.HORIZONTAL)));
        //}

        // ガイドライン常時
        bulletG.GetComponent<BulletGenerator>().EnableGuideLines(transform.position, Mathf.Atan2(Input.GetAxis(Player.VERTICAL), Input.GetAxis(Player.HORIZONTAL)));

        // バレットの発射
        attackButton = Input.GetAxis(Player.ATTACK);
        var balloonFloor = Input.GetAxis(GamePad.BUTTON_B);
        if (attackButton > 0 && attackButtonTrigger == 0.0f && Data.balloonSize >= bulletCost)
        {
            float angle = 0.0f;
            float v = Input.GetAxis(Player.VERTICAL);
            float h = Input.GetAxis(Player.HORIZONTAL);
            //if (Mathf.Abs(v) > 0.0f)
            //{
            //    angle = Mathf.PI * Mathf.Sign(v) * 0.5f;
            //}
            //else if (Data.playerDir < 0)
            //{
            //    angle = Mathf.PI;
            //}
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                angle = Mathf.Atan2(v, h);
            }
            else if (Data.playerDir < 0)
            {
                angle = Mathf.PI;
            }
            if (bulletG.GetComponent<BulletGenerator>().BulletCreate(transform.position, angle))
            {
                balloonController.UseBalloon(bulletCost);
            }
        }
        //// ExplosionTest
        //if (Input.GetKeyDown(KeyCode.B) && Data.num_balloon > 0)
        //{
        //    Vector3 vel = this.transform.position;
        //    if (Data.playerDir > 0)
        //    {
        //        vel.x -= 5;
        //    }
        //    else
        //    {
        //        vel.x += 5;
        //    }
        //    vel.y -= 3;
        //    ExplosionForce(vel, 500.0f, 800.0f);
        //    UsedBalloon();
        //}

        // 空中ブースト Test
        jumpButton = Input.GetAxis(Player.JUMP);
        float sticV = Input.GetAxis(Player.VERTICAL);
        Mathf.Abs(sticV); // 入力の度合

        if (boostCount >= 1)
        {
            if (jumpButton > 0 && jumpButtonTrigger == 0.0f && boostCost <= Data.balloonSize && jumpCount >= 1)
            {
                Vector2 direction = new Vector2(Input.GetAxis(Player.HORIZONTAL), Input.GetAxis(Player.VERTICAL));
                if(sticV <= 0.1f)
                {
                    direction.y = 0;
                }
                direction.Normalize();

                Boost(direction, boostCost);

                boostCount--;
            }
        }

        // エネミーを利用したブースト(仮)
        if (isEnemyBoost)
        {
            rig.velocity = rig.velocity * 0.0f;
            if (Input.GetAxis(Player.VERTICAL) <= 0.0f && sticV >= 0.1f)
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(Player.HORIZONTAL), boostForce.y * 1.3f * Input.GetAxis(Player.VERTICAL)), ForceMode2D.Impulse);
            }
            else
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(Player.HORIZONTAL), (boostForce.y * 1.3f * Input.GetAxis(Player.VERTICAL)) + 10.0f), ForceMode2D.Impulse);
            }
        }


        // ジャンプ
        if (hitCount == 0)
        {
            if (jumpButton > 0 && jumpButtonTrigger == 0.0f && coyoteFlag == true)
            {
                jumpCount = 1;
                if (this.rig.velocity.y <= 0)
                {
                    if (!isGround)   // コヨーテ中
                    {
                        this.rig.velocity = new Vector2(this.rig.velocity.x, 0.0f);
                    }

                    rig.AddForce(new Vector2(0, jumpForce));
                    isGround = false;
                    jumpStopFlag = false;
                }
            }
            // ジャンプ中にボタンを離す
            if (jumpButton == 0 && isGround == false && !jumpStopFlag && this.rig.velocity.y > 0)
            {
                rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y * 0.6f);
                jumpStopFlag = true;
            }
        }


        // 前フレームのキー入力の情報保持
        // Jump
        jumpButtonTrigger = jumpButton;
        // Attack
        attackButtonTrigger = attackButton;
        // Boost
        boostButtonTrigger = boostButton;
        // ------------------------------------------------------------------------------


        // 切り替えし(地上にいるときのみ)
        if (isGround)
        {
            //if (bubbleGround)
            //{
            if (lastDir > 0.0f && dir == -1)
            {
                bubbleG.GetComponent<BubbleGenerator>().BubbleCreate();
            }
            if (lastDir < 0.0f && dir == 1)
            {
                bubbleG.GetComponent<BubbleGenerator>().BubbleCreate();
            }
            //}
        }

        // 最終入力方向
        dirCount--;
        if (dirCount <= 0 || dir != 0)
        {
            lastDir = dir;
        }

        // プレイヤーの向き保持
        if (dir != 0)
        {
            Data.playerDir = dir;
        }

        // プレイヤーの向き変更
        if (Data.playerDir != 1)
        {
            this.transform.localRotation = new Quaternion(0, 180, 0, 0);
            antiRotationWrapper.transform.rotation = Quaternion.identity;
        }
        else
        {
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
            antiRotationWrapper.transform.rotation = Quaternion.identity;
        }

        // プレイヤーの速度取得
        Data.prePlayerVel = Data.currentPlayerVel;
        Data.currentPlayerVel = this.rig.velocity;

        // 重力の変更(バブルの個数に応じて)
        float buoyancy = Mathf.Min(maxPowerBalloonSize, Data.balloonSize) / maxPowerBalloonSize;
        float t = buoyancy * buoyancy;
        rig.gravityScale = Mathf.Lerp(5.0f, 1.5f, t);
        jumpForce = defaultJumpForce * Mathf.Lerp(1.0f, 0.9f, t);
        playerSpeed = accelForce * Mathf.Lerp(1.0f, 0.85f, t);

        //switch (Data.num_balloon)
        //{
        //    case 0:
        //        this.rig.gravityScale = 5.0f;
        //        jumpForce = defaultJumpForce * 1.0f;
        //        playerSpeed = accelForce * 1.0f;
        //        break;

        //    case 1:
        //        this.rig.gravityScale = 3.0f;
        //        jumpForce = defaultJumpForce * 0.9f;
        //        playerSpeed = accelForce * 0.9f;
        //        break;

        //    case 2:
        //        this.rig.gravityScale = 2.0f;
        //        jumpForce = defaultJumpForce * 0.8f;
        //        playerSpeed = accelForce * 0.85f;
        //        break;

        //    case 3:
        //        this.rig.gravityScale = 1.5f;
        //        jumpForce = defaultJumpForce * 0.77f;
        //        playerSpeed = accelForce * 0.8f;
        //        break;

        //    default:
        //        break;
        //}

        //接地時であれば Gravity Speed は 変化しない
        if (coyoteFlag)
        {
            this.rig.gravityScale = 5.0f;
            //jumpForce = defaultJumpForce * 1.0f;
            //playerSpeed = accelForce * 1.0f;
        }

        // コヨーテタイムによる接地判定
        if (!isGround)
        {
            coyoteCount++;
        }
        else
        {
            coyoteCount = 0;
            coyoteFlag = true;
        }
        if (coyoteCount >= coyoteTime)
        {
            coyoteFlag = false;
        }

        // 敵接触カウント
        if (hitCount > 0)
        {
            hitCount--;
        }

        // エネミーブーストのリセット
        isEnemyBoost = false;
    }

    //------------------------------------------------------------------------------------------
    // FixedUpdate
    //------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        // 入力感度 絶対値化
        stickSence = Mathf.Abs(stickSence);

        // 移動
        if (hitCount == 0)
        {
            if (isGround)
            {
                if (dir >= 1.0f && rig.velocity.x <= maxSpeed)  // 右側
                {
                    rig.AddForce(new Vector2(playerSpeed * dir * stickSence, 0));
                }
                else if (dir <= -1.0f && rig.velocity.x >= -maxSpeed)   // 左側
                {
                    rig.AddForce(new Vector2(playerSpeed * dir * stickSence, 0));
                }

                //rig.AddForce(new Vector2(playerSpeed * dir * stickSence, 0));
            }
            else
            {
                if (dir >= 1.0f && rig.velocity.x <= maxSpeed)  // 右側
                {
                    rig.AddForce(new Vector2(playerSpeed / 2.0f * dir * stickSence, 0));
                }
                else if (dir <= -1.0f && rig.velocity.x >= -maxSpeed)   // 左側
                {
                    rig.AddForce(new Vector2(playerSpeed / 2.0f * dir * stickSence, 0));
                }

                //rig.AddForce(new Vector2(playerSpeed / 2.0f * dir * stickSence, 0));
            }
        }

        //// 速さ制限
        //if (dir >= 1.0f && rig.velocity.x >= maxSpeed)  // 右側
        //{
        //    rig.velocity = new Vector2(maxSpeed, rig.velocity.y);
        //}
        //else if (dir <= -1.0f && rig.velocity.x <= -maxSpeed)   // 左側
        //{
        //    rig.velocity = new Vector2(-maxSpeed, rig.velocity.y);
        //}


        if (hitCount == 0)
        {
            // 接地時慣性消滅
            if (isGround && dir == 0)
            {
                this.rig.velocity = new Vector2(0, this.rig.velocity.y);
            }
            // 爆風加速対処
            if (isGround && Mathf.Abs(rig.velocity.x) >= 5.0f)
            {
                rig.velocity = new Vector2(5.0f * dir, rig.velocity.y);
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // OnCollision
    //------------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BreakObject")
        {
            ExplosionForce(collision.transform.position, 500.0f, 800.0f);
        }

        if (collision.gameObject.tag == Enemy.NAME || collision.gameObject.tag == Enemy.ATTACK)
        {
            KnockBack(collision.transform.position);
        }

        // ダメージタイルと衝突した時に破裂させる
        if (collision.gameObject.tag == Stage.DAMAGE_TILE)
        {
            balloonController.Burst();
            bubbleG.GetComponent<BubbleGenerator>().StopChase();
            deathFlag = true;
        }

        // エネミーと接触したらぶっ飛ぶ(入力方向)
        if (collision.gameObject.tag == Enemy.HIT_STATE)
        {
            isEnemyBoost = true;
        }

    }

    //------------------------------------------------------------------------------------------
    // OnTrigger
    //------------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == Stage.GROUND || collision.tag == Bubble.GROUND || collision.tag == Common.Floor.NAME || collision.tag == "DamageTile")
        {
            isGround = true;
        }

        if (collision.tag == Bubble.GROUND)
        {
            bubbleGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CollectObject")
        {
            // リスポーン位置を設定
            Data.initialPlayerPos = collision.gameObject.transform.position;
        }

        if (collision.tag == Stage.GROUND || collision.tag == Bubble.GROUND || collision.tag == Common.Floor.NAME || collision.tag == "DamageTile")
        {
            isGround = true;
            jumpCount = 0;
            boostCount = 2;
            // 着地エフェクト
            Vector2 effectSize = Vector2.one * 0.4f;
            EffectGenerator.BubbleBurstFX(
                new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
                new Vector2(this.transform.position.x, this.transform.position.y - 1.0f),
                null);
        }

        // バブルキャノン(仮)
        if (collision.gameObject.tag == "BubbleCannon")
        {
            collision.gameObject.transform.parent.gameObject.GetComponent<BubbleCannon>().HitToPlayer();
            rig.velocity = rig.velocity * 0.0f;
            float sticV = Input.GetAxis(Player.VERTICAL);
            Mathf.Abs(sticV); // 入力の度合

            if (Input.GetAxis(Player.VERTICAL) <= 0.0f && sticV >= 0.1f)
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(Player.HORIZONTAL), boostForce.y * 1.3f * Input.GetAxis(Player.VERTICAL)), ForceMode2D.Impulse);
            }
            else
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(Player.HORIZONTAL), (boostForce.y * 1.3f * Input.GetAxis(Player.VERTICAL)) + 10.0f), ForceMode2D.Impulse);
            }
            // ブースト回数の回復
            boostCount = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Stage.GROUND || collision.tag == Bubble.GROUND || collision.tag == Common.Floor.NAME)
        {
            isGround = false;
            bubbleGround = false;
        }
    }

    //------------------------------------------------------------------------------------------
    // イベント
    //------------------------------------------------------------------------------------------
    // 入力を受け付けるか設定する
    public void EnableControl(bool enable)
    {
        canControl = enable;
    }

    //------------------------------------------------------------------------------------------
    // バルーン追加
    //------------------------------------------------------------------------------------------
    public void AddBalloon(GameObject go)
    {
        // 所持バルーンリストに追加
        m_balloonList.Add(go);
        // 所持バルーンをカウント
        Data.num_balloon++;
    }

    //------------------------------------------------------------------------------------------
    // バルーンを使用する(古い順に消費する)
    //------------------------------------------------------------------------------------------
    public void UsedBalloon()
    {
        Destroy(m_balloonList[Integer.ZERO]);
        m_balloonList.RemoveAt(Integer.ZERO);
        Data.num_balloon--;
    }

    //------------------------------------------------------------------------------------------
    // バルーンが壊れる
    //------------------------------------------------------------------------------------------
    public void BrokenBalloon(GameObject balloon)
    {
        if (m_balloonList.Remove(balloon))
            Data.num_balloon--;
    }

    //------------------------------------------------------------------------------------------
    // バルーンの現在の所持数を取得
    //------------------------------------------------------------------------------------------
    public int GetMaxBalloons()
    {
        return m_balloonList.Count;
    }

    //------------------------------------------------------------------------------------------
    // ブースト移動
    //------------------------------------------------------------------------------------------
    public void Boost(Vector3 direction, float cost)
    {
        rig.velocity = rig.velocity * 0.0f;

        if (direction.y >= 0.0f)
        {
            rig.AddForce(new Vector2(boostForce.x * direction.x, boostForce.y * direction.y + 10.0f), ForceMode2D.Impulse);
        }
        else
        {
            rig.AddForce(new Vector2(boostForce.x * direction.x, boostForce.y * direction.y), ForceMode2D.Impulse);
        }
        // エフェクトを生成する
        EffectGenerator.BoostTrailFX(new BoostTrailFX.Param(Color.white, 0.5f, rig), transform.position);
        balloonController.UseBoost(cost);
    }

    //------------------------------------------------------------------------------------------
    // 爆発
    //------------------------------------------------------------------------------------------
    public void ExplosionForce(Vector3 expPos, float xPow, float yPow)
    {
        Vector3 vel = this.transform.position - expPos;
        vel.Normalize();
        // 縦横で吹っ飛び倍率変更
        vel.x = vel.x * xPow;
        vel.y = vel.y * yPow;
        this.rig.AddForce(vel);
    }

    //------------------------------------------------------------------------------------------
    // ノックバック
    //------------------------------------------------------------------------------------------
    public void KnockBack(Vector3 hitPos)
    {
        isGround = false;
        hitCount = 15;
        Vector2 hitVel = this.transform.position - hitPos;
        Vector2 addVel = new Vector2(8.0f, 8.0f);
        hitVel.Normalize();
        if (hitVel.x < 0)
        {
            addVel.x *= -1;
        }
        this.rig.velocity = Vector2.zero;
        this.rig.AddForce(addVel, ForceMode2D.Impulse);
    }

    //------------------------------------------------------------------------------------------
    // 死亡時アニメーション
    //------------------------------------------------------------------------------------------
    public void DeathAnimation(bool enable)
    {
        // 死亡カウント
        if (enable)
        {
            EnableControl(false);
            deathCount += Time.deltaTime;
            // アニメーション終了
            if (deathCount >= animationTime)
            {
                this.transform.position = Data.initialPlayerPos;
                deathCount = 0.0f;
                deathFlag = false;
                EnableControl(true);
            }
        }
    }
}

