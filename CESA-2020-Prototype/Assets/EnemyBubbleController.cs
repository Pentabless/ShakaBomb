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

    // Start is called before the first frame update
    void Start()
    {
        isBlow = false;
        move_force = new Vector2(0.025f, 0.025f * Random.Range(0.1f, 3.0f));
        angle = Random.Range(0.0f, 180.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //既に飛んでいたら
        if (isBlow)
        {
            angle += 0.1f;
            //上下に揺れながら進む
            transform.Translate(move_force.x, Mathf.Sin(angle) * move_force.y, 0.0f);

            //画面外に出たら
            if(!GetComponent<Renderer>().isVisible)
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

    public void SetDir(int dir)
    {
        move_force.x *= dir;
    } 
}
