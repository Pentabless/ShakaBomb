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
        Acceleration,
        Damage
    }

    // リジッドボディ
    Rigidbody2D rig;

    // バブルジェネレータ
    [SerializeField]
    GameObject bubbleG;
    BubbleGenerator bubbleGenerator = null;

    // バルーンコントローラ
    [SerializeField]
    BackBalloonController balloonController = null;
    // 回転を無視するオブジェクト
    [SerializeField]
    GameObject antiRotationWrapper = null;

    // バレットジェネレータ
    [SerializeField]
    GameObject bulletG;
    BulletGenerator bulletGenerator = null;

    // ダートマネージャ
    [SerializeField]
    DirtManager dirtManager = null;

    // コントローラ
    [SerializeField]
    GameObject gamepadManagerObject;
    GamepadManager gamaepadManager = null;

    // SE
    [SerializeField]
    List<AudioClip> audios;

    // 入力を受け付けるかのフラグ
    bool canControl = true;
    // ジャンプできるかのフラグ
    bool canJump = true;

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
    bool jumpTiming;

    // 切り替えし猶予フレーム
    [SerializeField]
    int turnCount;

    // 接地フラグ
    bool isGround;

    // 敵接触時
    int hitCount = ConstInteger.ZERO;

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
    float jumpButton = ConstDecimal.ZERO;
    float jumpButtonTrigger = ConstDecimal.ZERO;

    float boostButton = ConstDecimal.ZERO;
    float boostButtonTrigger = ConstDecimal.ZERO;

    // 浮力が最高になるバルーンサイズ
    [SerializeField]
    float maxPowerBalloonSize = 2.5f;
    // バレットの発射コスト
    [SerializeField]
    float bulletCost = 1.0f;
    // ブースト移動コスト
    [SerializeField]
    float boostCost = 1.5f;

    // 死亡フラグ
    bool deathFlag = false;

    // 自動操作フラグ
    bool autoControl = false;
    // 移動移動が完了したかどうか
    bool autoMoveFinished = false;
    // 移動先
    Vector3 targetPos = Vector3.zero;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        isGround = false;
        dir = ConstInteger.ZERO;
        lastDir = ConstInteger.ZERO;
        dirCount = ConstInteger.ZERO;
        jumpForce = defaultJumpForce;
        jumpCount = ConstInteger.ZERO;
        jumpStopFlag = false;
        jumpTiming = false;
        playerSpeed = accelForce;

        bubbleGenerator = bubbleG.GetComponent<BubbleGenerator>();
        bulletGenerator = bulletG.GetComponent<BulletGenerator>();
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        jumpTiming = false;

        // 故障中の処理
        if (deathFlag)
        {
            DeathUpdate();
        }

        // 敵接触カウント
        if (hitCount > 0)
        {
            hitCount--;
        }

        if (autoControl)
        {
            bulletGenerator.DisableGuideLines();
            if (transform.position.x < targetPos.x - 0.1f)
            {
                Data.playerDir = lastDir = dir = 1;
                stickSence = 0.1f;
                autoMoveFinished = false;
            }
            else if (transform.position.x > targetPos.x + 0.1f)
            {
                Data.playerDir = lastDir = dir = -1;
                stickSence = 0.1f;
                autoMoveFinished = false;
            }
            else
            {
                dir = 0;
                stickSence = 0;
                autoMoveFinished = true;
            }

            return;
        }

        if (!canControl)
        {
            dir = ConstInteger.ZERO;
            bulletGenerator.DisableGuideLines();
            return;
        }

        // プレイヤー操作系統 (入力が必要なもの)--------------------------------------------------

        // 左右移動
        if (Input.GetAxis(ConstPlayer.HORIZONTAL) < ConstInteger.ZERO)
        {
            dir = -1;
            dirCount = turnCount;
            stickSence = Input.GetAxis(ConstPlayer.HORIZONTAL);
        }
        else if (Input.GetAxis(ConstPlayer.HORIZONTAL) > ConstInteger.ZERO)
        {
            dir = 1;
            dirCount = turnCount;
            stickSence = Input.GetAxis(ConstPlayer.HORIZONTAL);
        }
        else
        {
            dir = 0;
        }

        // バレットの発射
        bool releaseAttack = Input.GetButtonUp(ConstPlayer.ATTACK);
        var balloonFloor = Input.GetAxis(ConstGamePad.BUTTON_B);
        if (releaseAttack && Data.balloonSize >= bulletCost)
        {
            float angle = 0.0f;
            float v = Input.GetAxis(ConstPlayer.VERTICAL);
            float h = Input.GetAxis(ConstPlayer.HORIZONTAL);
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                angle = Mathf.Atan2(v, h);
            }
            else if (Data.playerDir < 0)
            {
                angle = Mathf.PI;
            }
            if (bulletGenerator.BulletCreate(transform.position, angle))
            {
                balloonController.UseBalloon(bulletCost);
            }
        }

        // バレットの発射ボタンを押している間、ガイドラインを表示する
        if (Input.GetButton(ConstPlayer.ATTACK))
        {
            stickSence = 0.0f;
            dir = 0;
            var inputDir = new Vector2(Input.GetAxis(ConstPlayer.HORIZONTAL), Input.GetAxis(ConstPlayer.VERTICAL));
            if (inputDir.magnitude > 0)
            {
                bulletGenerator.EnableGuideLines(transform.position, Mathf.Atan2(inputDir.y, inputDir.x));
            }
            else
            {
                bulletGenerator.EnableGuideLines(transform.position, (Data.playerDir >= 0 ? 0 : Mathf.PI));
            }
        }
        else
        {
            bulletGenerator.DisableGuideLines();
        }

        // 空中ブースト
        jumpButton = Input.GetAxis(ConstPlayer.JUMP);
        boostButton = Input.GetAxis(ConstPlayer.BOOST);
        float sticV = Mathf.Abs(Input.GetAxis(ConstPlayer.VERTICAL));

        if ((jumpButton > 0 && jumpButtonTrigger == 0.0f && boostCost <= Data.balloonSize && jumpCount > 0) || (boostButton > 0 && boostButtonTrigger == 0.0f && boostCost <= Data.balloonSize))
        {
            Vector2 direction = new Vector2(Input.GetAxis(ConstPlayer.HORIZONTAL), Input.GetAxis(ConstPlayer.VERTICAL));
            if (sticV <= 0.1f)
            {
                direction.y = 0;
            }
            direction.Normalize();

            Boost(direction);

            // エフェクトを生成する
            EffectGenerator.BoostTrailFX(new BoostTrailFX.Param(Color.white, 0.5f, rig), transform.position);
            balloonController.UseBoost(boostCost, 1.3f);

            SoundPlayer.Play(audios[(int)AudioType.Acceleration]);

            boostCount--;
        }

        // エネミーを利用したブースト(仮)
        if (isEnemyBoost)
        {
            Boost(Vector3.zero);
        }


        // ジャンプ
        if (hitCount == 0)
        {
            if (canJump && jumpButton > 0 && jumpButtonTrigger == 0.0f && isGround)
            {
                if (this.rig.velocity.y <= 0)
                {
                    jumpCount = 1;
                    rig.AddForce(new Vector2(0, jumpForce));
                    isGround = false;
                    jumpStopFlag = false;
                    jumpTiming = true;

                    SoundPlayer.Play(audios[(int)AudioType.Jump]);
                }
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
                bubbleGenerator.BubbleCreate(transform.position, 1, true);
                dirtManager.SweepDirt();
            }
            if (lastDir < 0.0f && dir == 1)
            {
                bubbleGenerator.BubbleCreate(transform.position, 1, true);
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

        // プレイヤーの速度取得
        Data.prePlayerVel = Data.currentPlayerVel;
        Data.currentPlayerVel = this.rig.velocity;

        // 重力の変更(バブルの個数に応じて)
        float buoyancy = Mathf.Min(maxPowerBalloonSize, Data.balloonSize) / maxPowerBalloonSize;
        float t = buoyancy * buoyancy;
        rig.gravityScale = Mathf.Lerp(5.0f, 1.5f, t);
        jumpForce = defaultJumpForce * Mathf.Lerp(1.0f, 0.9f, t);
        playerSpeed = accelForce * Mathf.Lerp(1.0f, 0.85f, t);

        // エネミーブーストのリセット
        isEnemyBoost = false;
    }



    //------------------------------------------------------------------------------------------
    // summary : FixedUpdate
    // remarks : none
    // param   : none
    // return  : none
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
            if (isGround && dir == 0 && rig.velocity.y <= 0.1f)
            {
                this.rig.velocity = new Vector2(0, this.rig.velocity.y);
            }
            // 爆風加速対処
            if (isGround && Mathf.Abs(rig.velocity.x) >= 5.0f && rig.velocity.y <= 0.1f)
            {
                rig.velocity = new Vector2(5.0f * dir, rig.velocity.y);
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : OnCollisionEnter2D
    // remarks : none
    // param   : Collision2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BreakObject")
        {
            ExplosionForce(collision.transform.position, 500.0f, 800.0f);
        }

        if (collision.gameObject.tag == ConstEnemy.NAME || collision.gameObject.tag == ConstEnemy.ATTACK)
        {
            if (!deathFlag)
            {
                StartCoroutine(AudioPlay());
                GameObject.Find(ConstCamera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.1f, 1.0f);
            }
            KnockBack(collision.transform.position);
            balloonController.Burst();
            balloonController.EnableMerge(false);
            bubbleGenerator.StopChase();
            deathFlag = true;
        }

        // ダメージタイルと衝突した時に破裂させる
        if (collision.gameObject.tag == ConstStage.DAMAGE_TILE)
        {
            if (!deathFlag)
            {
                StartCoroutine(AudioPlay());
                GameObject.Find(ConstCamera.MAIN_CAMERA).GetComponent<CameraShake>().Shake(0.1f, 1.0f);
            }
            //SoundPlayer.Play(audios[(int)AudioType.Damage]);
            balloonController.Burst();
            balloonController.EnableMerge(false);
            bubbleGenerator.StopChase();
            deathFlag = true;
        }

        // エネミーと接触したらぶっ飛ぶ(入力方向)
        if (collision.gameObject.tag == ConstEnemy.HIT_STATE)
        {
            isEnemyBoost = true;
        }

    }




    //------------------------------------------------------------------------------------------
    // summary : AudioPlay
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private IEnumerator AudioPlay()
    {
        SoundPlayer.Play(audios[(int)AudioType.Damage]);
        yield return null;
    }



    //------------------------------------------------------------------------------------------
    // summary : OnTriggerStay2D
    // remarks : none
    // param   : Collider2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == ConstStage.GROUND || 
            collision.tag == ConstBubble.GROUND || 
            collision.tag == ConstFloor.NAME || 
            collision.tag == ConstStage.DAMAGE_TILE)
        {
            isGround = true;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : OnTriggerEnter2D
    // remarks : none
    // param   : Collider2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CollectObject")
        {
            // リスポーン位置を設定
            Data.initialPlayerPos = collision.gameObject.transform.position;
        }

        if (collision.tag == ConstStage.GROUND || 
            collision.tag == ConstBubble.GROUND || 
            collision.tag == ConstFloor.NAME || 
            collision.tag == ConstStage.DAMAGE_TILE)
        {
            isGround = true;
            jumpCount = 0;
            boostCount = 2;

            // 着地エフェクト(保留)
            if (!IsDead())
            {
                Vector2 effectSize = Vector2.one * 0.7f;
                EffectGenerator.BubbleBurstFX(
                    new BubbleBurstFX.Param(this.GetComponent<SpriteRenderer>().color, effectSize, 160, 4),
                    new Vector2(this.transform.position.x, this.transform.position.y - 1.0f),
                    null);
            }
        }

        // バブルキャノン(仮)
        if (collision.gameObject.tag == "BubbleCannon")
        {
            collision.gameObject.transform.parent.gameObject.GetComponent<BubbleCannon>().HitToPlayer();
            rig.velocity = rig.velocity * 0.0f;
            float sticV = Input.GetAxis(ConstPlayer.VERTICAL);
            Mathf.Abs(sticV); // 入力の度合

            if (Input.GetAxis(ConstPlayer.VERTICAL) <= 0.0f && sticV >= 0.1f)
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(ConstPlayer.HORIZONTAL), 
                    boostForce.y * 1.3f * Input.GetAxis(ConstPlayer.VERTICAL)), 
                    ForceMode2D.Impulse);
            }
            else
            {
                rig.AddForce(new Vector2(boostForce.x * 1.3f * Input.GetAxis(ConstPlayer.HORIZONTAL), 
                    (boostForce.y * 1.3f * Input.GetAxis(ConstPlayer.VERTICAL)) + 10.0f), 
                    ForceMode2D.Impulse);
            }
            // ブースト回数の回復
            boostCount = 2;
        }

        // バブルチャージ
        if (collision.gameObject.tag == "BubbleCharge")
        {
            Boost(Vector3.zero);
        }

        // 汚れ
        if (collision.gameObject.tag == "Dirt")
        {
            dirtManager.EnterDirt(collision.gameObject);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : OnTriggerExit2D
    // remarks : none
    // param   : Collider2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == ConstStage.GROUND || 
            collision.tag == ConstBubble.GROUND || 
            collision.tag == ConstFloor.NAME)
        {
            isGround = false;
            jumpCount = 1;
        }

        // 汚れ
        if (collision.gameObject.tag == "Dirt")
        {
            dirtManager.ExitDirt(collision.gameObject);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 入力を受け付けるか設定する
    // remarks : none
    // param   : bool
    // return  : none
    //------------------------------------------------------------------------------------------
    public void EnableControl(bool enable)
    {
        canControl = enable;
    }



    //------------------------------------------------------------------------------------------
    // summary : ジャンプを受け付けるか設定する
    // remarks : none
    // param   : bool
    // return  : none
    //------------------------------------------------------------------------------------------
    public void EnableJump(bool enable)
    {
        canJump = enable;
    }



    //------------------------------------------------------------------------------------------
    // summary : 自動操作に切り替えるか設定する
    // remarks : none
    // param   : bool
    // return  : none
    //------------------------------------------------------------------------------------------
    public void EnableAutoControl(bool enable)
    {
        autoControl = enable;
    }



    //------------------------------------------------------------------------------------------
    // summary : 自動操作の行き先を設定する
    // remarks : none
    // param   : Vector3
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
        autoMoveFinished = false;
    }



    //------------------------------------------------------------------------------------------
    // summary : 自動移動が完了しているか取得する
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool AutoMoveFinished()
    {
        return autoMoveFinished;
    }



    //------------------------------------------------------------------------------------------
    // summary : 現在の方向を取得する
    // remarks : none
    // param   : none
    // return  : int
    //------------------------------------------------------------------------------------------
    public int GetCurrentDir()
    {
        return dir;
    }



    //------------------------------------------------------------------------------------------
    // summary : ジャンプしたタイミングか取得する
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool JumpTiming()
    {
        return jumpTiming;
    }



    //------------------------------------------------------------------------------------------
    // summary : 接地しているか取得する
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool IsGround()
    {
        return isGround;
    }



    //------------------------------------------------------------------------------------------
    // summary : 死亡しているか取得する
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool IsDead()
    {
        return deathFlag;
    }



    //------------------------------------------------------------------------------------------
    // summary : バルーン追加
    // remarks : none
    // param   : GameObject
    // return  : none
    //------------------------------------------------------------------------------------------
    public void AddBalloon(GameObject go)
    {
        // 所持バルーンリストに追加
        m_balloonList.Add(go);
        // 所持バルーンをカウント
        Data.num_balloon++;
    }



    //------------------------------------------------------------------------------------------
    // summary : バルーンを使用する(古い順に消費する)
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void UsedBalloon()
    {
        Destroy(m_balloonList[ConstInteger.ZERO]);
        m_balloonList.RemoveAt(ConstInteger.ZERO);
        Data.num_balloon--;
    }



    //------------------------------------------------------------------------------------------
    // summary : バルーンが壊れる
    // remarks : none
    // param   : GameObject
    // return  : none
    //------------------------------------------------------------------------------------------
    public void BrokenBalloon(GameObject balloon)
    {
        if (m_balloonList.Remove(balloon))
            Data.num_balloon--;
    }



    //------------------------------------------------------------------------------------------
    // summary : バルーンの現在の所持数を取得
    // remarks : none
    // param   : none
    // return  : int
    //------------------------------------------------------------------------------------------
    public int GetMaxBalloons()
    {
        return m_balloonList.Count;
    }



    //------------------------------------------------------------------------------------------
    // summary : ブースト移動コストを取得
    // remarks : none
    // param   : none
    // return  : float
    //------------------------------------------------------------------------------------------
    public float GetBoostCost()
    {
        return boostCost;
    }



    //------------------------------------------------------------------------------------------
    // summary : ブースト移動
    // remarks : none
    // param   : Vector3
    // return  : none
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
    // summary : 爆発
    // remarks : none
    // param   : Vector3、float、float
    // return  : none
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
    // summary : ノックバック処理
    // remarks : none
    // param   : Vector3
    // return  : none
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
    // summary : 死亡時処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void DeathUpdate()
    {
        EnableControl(false);
    }



    //------------------------------------------------------------------------------------------
    // summary : 復活処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void Repair()
    {
        deathFlag = false;
        EnableControl(true);
        balloonController.EnableMerge(true);
    }
}

