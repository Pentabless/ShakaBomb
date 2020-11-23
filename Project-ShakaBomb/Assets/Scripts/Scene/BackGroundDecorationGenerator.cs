using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundDecorationGenerator : MonoBehaviour
{
    //飾りのオブジェクト
    public GameObject decoration_prefab;
    //飾り(背景)の数
    public int num_back_decoration;
    //飾り(前景)の数
    public int num_front_decoration;

    //飾り(背景)のカウント
    int back_decoration_count;
    //飾り(前景)のカウント
    int front_decoration_count;

    // Start is called before the first frame update
    void Start()
    {
     }

    // Update is called once per frame
    void Update()
    {
    }

    //飾りを作る
    public bool CreateDecoration(Vector3 position, Vector3 scale, Color color, int layer)
    {
        //作れる状態か
        bool can_create = false;

        //前景だったら
        if (layer >= 0)
        {
            //前景の数が最大値より小さかったら
            if(front_decoration_count<num_front_decoration)
            {
                //作れる状態にする
                can_create = true;
                //前景の数をカウントする
                front_decoration_count++;
            }
        }
        //背景だったら
        else if (layer < 0)
        {
            //背景の数が最大値より小さかったら
            if (back_decoration_count < num_back_decoration)
            {
                //作れる状態にする
                can_create = true;
                //背景の数をカウントする
                back_decoration_count++;
            }
        }

        //作れる状態だったら
        if (can_create)
        {
            //プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(decoration_prefab) as GameObject;
            // ジェネレーターの子オブジェクトにする
            go.transform.parent = this.gameObject.transform;
            //座標を設定する
            go.transform.position = position;
            //座標を設定する
            go.transform.localScale = scale;
            //色を設定する
            go.GetComponent<SpriteRenderer>().color = color;
            //レイヤーを設定する
            go.GetComponent<SpriteRenderer>().sortingOrder = layer;
            //移動量を設定する
            go.GetComponent<BackGroundDecorationController>().SetMoveForce(new Vector3(Random.Range(0.01f, 0.05f), Random.Range(0.05f, 0.2f), 0.0f));
            //角度を設定する
            go.GetComponent<BackGroundDecorationController>().SetAngle(Random.Range(1.0f, 10.0f));
        }

        return can_create;
    }

    //カウントを減らす
    public void DecreaseDecorationCount(int layer)
    {
        //前景だったら
        if (layer >= 0)
        {
            //カウントが0より上だったら
            if (front_decoration_count > 0)
            {
                //カウントを減らす
                front_decoration_count--;
            }
        }
        //背景だったら
        else if (layer < 0)
        {
            //カウントが0より上だったら
            if (back_decoration_count > 0)
            {
                //カウントを減らす
                back_decoration_count--;
            }
        }
    }

    //背景の泡の数を渡す
    public int GetNumBackDecoration()
    {
        return num_back_decoration;
    }

    //前景の泡の数を渡す
    public int GetNumFrontDecoration()
    {
        return num_front_decoration;
    }
}
