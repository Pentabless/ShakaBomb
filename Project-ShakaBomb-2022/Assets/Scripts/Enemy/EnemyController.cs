using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //プレイヤーを見つけたか
    bool find_player;
    //進む先に床がある
    bool floor_checker;
    //今の向き
    int now_dir;
    //移動力
    public Vector2 enemy_move_force;
    //攻撃の移動力
    public Vector2 attacks_move_force;

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤーを見つけていない
        find_player = false;
        //床がある
        floor_checker = true;
        //左向き -1　　右向き 1
        now_dir = (int)(transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        //向きを更新する
        transform.localScale = new Vector3(now_dir, 1.0f, 1.0f);
    }

    //プレイヤーを見つけたか
    public bool GetFindPlayer()
    {
        return find_player;
    }
    public void SetFindPlayer(bool find)
    {
        find_player = find;
    }

    //床があるか
    public bool GetFloorChecker()
    {
        return floor_checker;
    }
    public void SetFloorChecker(bool check)
    {
        floor_checker = check;
    }

    //向いている方向
    public int GetDir()
    {
        return now_dir;
    }
    public void SetDir(int dir)
    {
        now_dir = dir;
    }

    //移動力
    public Vector2 GetEnemyMoveForce()
    {
        return enemy_move_force;
    }
    //攻撃の移動力
    public Vector2 GetAttacksMoveForce()
    {
        return attacks_move_force;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(this.gameObject);
        }
    }
}
