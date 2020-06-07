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
    // オーディオタイプ
    enum AudioType
    {
        Jump,
        Acceleration
    }

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

    // ダートマネージャ
    [SerializeField]
    DirtManager dirtManager = null;

    // コントローラ
    [SerializeField]
    GameObject gamepadManager;

    // SE
    [SerializeField]
    List<AudioClip> audios;

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
            bulletG.GetComponent<BulletGenerator>().DisableGuideLines();
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

        // バレットの発射
        bool releaseAttack = Input.GetButtonUp(Player.ATTACK);
        var balloonFloor = Input.GetAxis(GamePad.BUTTON_B);
        if (releaseAttack && Data.balloonSize >= bulletCost)
        {
            float angle = 0.0f;
            float v = Input.GetAxis(Player.VERTICAL);
            float h = Input.GetAxis(Player.HORIZONTAL);
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

        // バレットの発射ボタンを押している間、ガイドラインを表示する
        if (Input.GetButton(Player.ATTACK))
        {
            var inputDir = new Vector2(Input.GetAxis(Player.HORIZONTAL), Input.GetAxis(Player.VERTICAL));
            if (inputDir.magnitude > 0)
            {
                bulletG.GetComponent<BulletGenerator>().EnableGuideLines(transform.position, Mathf.Atan2(inputDir.y, inputDir.x));
            }
            else
            {
                bulletG.GetComponent<BulletGenerator>().EnableGuideLines(transform.position, (Data.playerDir >= 0 ? 0 : Mathf.PI));
            }
        }
        else
        {
            bulletG.GetComponent<BulletGenerator>().DisableGuideLines();
        }

        // 空中ブースト
        jumpButton = Input.GetAxis(Player.JUMP);
        float sticV = Mathf.Abs(Input.GetAxis(Player.VERTICAL));

        if (boostCount >= 1)
        {
            if (jumpButton > 0 && jumpButtonTrigger == 0.0f && boostCost <= Data.balloonSize && jumpCount >= 1)
            {
                Vector2 direction = new Vector2(Input.GetAxis(Player.HORIZONTAL), Input.GetAxis(Player.VERTICAL));
                if (sticV <= 0.1f)
                {
                    direction.y = 0;
                }
                direction.Normalize();

                Boost(direction);

                // エフェクトを生成する
                EffectGenerator.BoostTrailFX(new BoostTrailFX.Param(Color.white, 0.5f, rig), transform.position);
                balloonController.UseBoost(boostCost);

                SoundPlayer.Play(audios[(int)AudioType.Acceleration]);

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

                    SoundPlayer.Play(audios[(int)AudioType.Jump]);
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
        // Boost
        boostButtonTrigger = boostButton;
        // ------------------------------------------------------------------------------


        // 切り替えし(地上にいるときのみ)
        if (isGround)
        {
            if (lastDir > 0.0f && dir == -1)
            {
                bubbleG.GetComponent<BubbleGenerator>().BubbleCreate();
                dirtManager.SweepDirt();
            }
            if (lastDir < 0.0f && dir == 1)
            {
                bubbleG.GetComponent<BubbleGenerator>().BubbleCreate();
                dirtManager.SweepDirt();
            }
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

        //接地時であれば Gravity Speed は 変化しない
        if (coyoteFlag)
        {
            this.rig.gravityScale = 5.0f;
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
            }
        }

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
            balloonController.Burst();
            bubbleG.GetComponent<BubbleGenerator>().StopChase();
            deathFlag = true;
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

            //// 着地エフェクト(保留)
            //Vector2 effectSize = Vector2.one * 0.7f;
            //EffectGenerator.BubbleBurstFX(
            //    new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize),
            //    new Vector2(this.transform.position.x, this.transform.position.y - 1.0f),
            //    null);
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

        // 汚れ
        if (collision.gameObject.tag == "Dirt")
        {
            dirtManager.EnterDirt(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Stage.GROUND || collision.tag == Bubble.GROUND || collision.tag == Common.Floor.NAME)
        {
            isGround = false;
            jumpCount = 1;
            bubbleGround = false;
        }

        // 汚れ
        if (collision.gameObject.tag == "Dirt")
        {
            dirtManager.ExitDirt(collision.gameObject);
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
    // ブースト移動コストを取得
    //------------------------------------------------------------------------------------------
    public float GetBoostCost()
    {
        return boostCost;
    }

    //------------------------------------------------------------------------------------------
    // ブースト移動
    //------------------------------------------------------------------------------------------
    public void Boost(Vector3 direction)
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
        jumpCount = 1;
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

