using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBubbleGenerator : MonoBehaviour
{
    //プレファブ
    public GameObject bubblePrefab;    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateBubble(GameObject character,Vector3 offset,Vector4 color,Vector2 move_force,bool circular_motion_x,bool circular_motion_y)
    {
        //プレファブと同じオブジェクトを作る
        GameObject go = Instantiate(bubblePrefab) as GameObject;
        //座標を設定する
        go.transform.position = character.transform.position + offset;
        //色を設定する
        go.GetComponent<Renderer>().material.color = color;
        //移動力を設定する
        go.GetComponent<EnemyBubbleController>().SetMoveFroce(move_force);
        //円運動をするかを設定する
        go.GetComponent<EnemyBubbleController>().SetCircularMotionCheck(circular_motion_x, circular_motion_y);
    }
}
