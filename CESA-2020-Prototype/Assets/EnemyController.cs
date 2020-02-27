using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //プレイヤーを見つけたか
    bool find_player;
    //自身についている泡
    List<Transform> enemy_attacks;
    //自身についている泡の数
    int num_attacks;
    //飛ばす泡のインターバル
    private const int attack_interval = 120;
    //飛ばす泡のカウント
    int attack_count;
    //初期の向き
    int start_dir;
    //今の向き
    int dir;
    //逃げる速さ
    float escape_vel;
    //進む先に床がある
    bool fall_checker;

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        enemy_attacks = new List<Transform>();
        //子を全て取得する
        foreach (Transform child in transform)
        {
            //敵の攻撃だったら
            if (child.tag == "EnemyAttack")
            {
                enemy_attacks.Add(child);
            }
        }
        //泡の数を覚える
        num_attacks = enemy_attacks.Count;

        Debug.Log("NumAttack" + num_attacks);

        attack_count = 0;

        //左向き…-1　右向き…1
        start_dir = (int)(transform.localScale.x);
        dir = start_dir;

        escape_vel = 0.05f;

        fall_checker = true;
    }

    // Update is called once per frame
    void Update()
    {
        //泡が残っていたら
        if (num_attacks > 0)
        {
            //プレイヤーを見つけていて泡を飛ばすインターバルが終わったら
            if ((find_player == true) && (attack_count <= 0))
            {
                //飛ばす泡の向きを設定する
                enemy_attacks[num_attacks - 1].GetComponent<EnemyBubbleController>().SetDir(start_dir);
                //後ろの要素から飛ばすようにする
                enemy_attacks[num_attacks - 1].parent=null;
                //泡の数を変更する
                num_attacks--;
                //インターバル設定
                attack_count = attack_interval;
            }
        }
        //残ってなかったら
        else if(num_attacks==0)
        {
            //逃げる方向を決める
            dir *= -1;
            //この処理に当たらないようにする
            num_attacks--;
        }
        else
        {
            transform.Translate(dir * escape_vel, 0.0f, 0.0f);
        }

        //進む先に床がなかったら
        if(fall_checker==false)
        {
            //方向を変える
            dir *= -1;
            //床がある事にする
            fall_checker = true;
        }

        //攻撃のカウントダウンをする
        attack_count--;
        transform.localScale = new Vector3(dir, 1.0f, 1.0f);
    }

    public void FindPlayer(bool find)
    {
        find_player = find;
        if (find)
        {
            Debug.Log("プレイヤー見つけた！");
        }
        else
        {
            Debug.Log("プレイヤーどこ？");
        }
    }

    public void SetFallChecker(bool check)
    {
        fall_checker = check;
    }
}
