using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepEnemyController : MonoBehaviour
{
    //自身についている泡
    List<Transform> enemy_attacks;
    //自身についている泡の数
    int num_attacks;
    //自身についている泡の色
    public Color attacks_color;
    //飛ばす泡のインターバル
    private const int attack_interval = 120;
    //飛ばす泡のカウント
    int attack_count;
    //初期の向き
    int start_dir;

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
                //泡をリストに追加する
                enemy_attacks.Add(child);
                //色を設定する
                child.GetComponent<Renderer>().material.color = attacks_color;
            }
        }
        //泡の数を覚える
        num_attacks = enemy_attacks.Count;
        attack_count = 0;

        //左向き…-1　右向き…1
        start_dir = (int)(transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        //泡が残っていたら
        if (num_attacks > 0)
        {
            //プレイヤーを見つけていて泡を飛ばすインターバルが終わったら
            if ((GetComponent<EnemyController>().GetFindPlayer() == true) && (attack_count <= 0))
            {
                //飛ばす泡の移動力を設定する
                enemy_attacks[num_attacks - 1].GetComponent<EnemyBubbleController>().SetMoveFroce(
                    new Vector2(GetComponent<EnemyController>().GetAttacksMoveForce().x * GetComponent<EnemyController>().GetDir(),
                    GetComponent<EnemyController>().GetAttacksMoveForce().y * Random.Range(0.1f, 3.0f)));
                //飛ばす泡をy軸に円運動するように設定する
                enemy_attacks[num_attacks - 1].GetComponent<EnemyBubbleController>().SetCircularMotionCheck(false, true);
                //後ろの要素から飛ばすようにする
                enemy_attacks[num_attacks - 1].parent = null;
                //泡の数を変更する
                num_attacks--;
                //インターバル設定
                attack_count = attack_interval;
            }
        }
        //残ってなかったら
        else if (num_attacks == 0)
        {
            //逃げる方向を決める
            GetComponent<EnemyController>().SetDir(GetComponent<EnemyController>().GetDir() * -1);
            //この処理に当たらないようにする
            num_attacks--;
        }
        else
        {
            transform.Translate(GetComponent<EnemyController>().GetDir() * GetComponent<EnemyController>().GetEnemyMoveForce().x, 0.0f, 0.0f);
        }

        //進む先に床がなかったら
        if (GetComponent<EnemyController>().GetFloorChecker() == false)
        {
            //方向を変える
            GetComponent<EnemyController>().SetDir(GetComponent<EnemyController>().GetDir() * -1);
            //床がある事にする
            GetComponent<EnemyController>().SetFloorChecker(true);
        }

        //攻撃のカウントダウンをする
        attack_count--;
    }
}
