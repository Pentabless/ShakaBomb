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
                //後ろの要素から飛ばすようにする
                enemy_attacks[num_attacks - 1].parent=null;
                //泡の数を変更する
                num_attacks--;
                //インターバル設定
                attack_count = attack_interval;
            }
        }

        //攻撃のカウントダウンをする
        attack_count--;
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
}
