using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBubbleController : MonoBehaviour
{
    //飛んでいるか
    bool isBlow;
    //移動の力
    Vector2 move_force;
    //円運動するための角度
    float angle;
    //円運動する設定  [0]…x　[1]…y
    bool[] circular_motion_check = new bool[2] { false, false };

    // Start is called before the first frame update
    void Start()
    {
        isBlow = false;
        angle = Random.Range(0.0f, 180.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //既に飛んでいたら
        if (isBlow)
        {
            angle += 0.1f;

            Vector3 move = move_force;

            //x軸が円運動するように設定されていたら
            if(circular_motion_check[0])
            {
                move.x *= Mathf.Sin(angle);
            }
            //y軸が円運動するように設定されていたら
            if(circular_motion_check[1])
            {
                move.y *= Mathf.Sin(angle);
            }
            //上下に揺れながら進む
            transform.Translate(move);

            //画面外に出たら
            if (!GetComponent<Renderer>().isVisible)
            {
                //消える
                Destroy(this.gameObject);
            }
        }
        else
        {
            //親がいなかったら
            if (transform.parent == null)
            {
                isBlow = true;
            }
        }
    }

    public void SetMoveFroce(Vector2 force)
    {
        move_force = force;
    }

    public void SetCircularMotionCheck(bool x,bool y)
    {
        circular_motion_check[0] = x;
        circular_motion_check[1] = y;
    }
}
