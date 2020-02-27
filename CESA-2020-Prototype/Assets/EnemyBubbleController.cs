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
    //
    Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        isBlow = false;
        move_force = new Vector2(0.025f, 0.025f);
        angle = Random.Range(0.0f,180.0f);
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //既に飛んでいたら
        if(isBlow)
        {
            angle += 0.1f;
            //上下に揺れながら進む
            transform.Translate(-move_force.x, Mathf.Sin(angle)*move_force.y, 0.0f);
        }
        else
        {
            //親がいなかったら
            if(transform.parent==null)
            {
                isBlow = true;
            }
        }
    }

}
