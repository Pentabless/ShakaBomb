using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundDecorationController : MonoBehaviour
{
    //円運動用角度
    float circular_motion_angle;
    //円運動用
    float angle_force;
    //移動量
    Vector3 move_force;

    //座標を記憶する
    Vector3 remember_position;
    //回転を記憶する
    Quaternion remember_rotation;
    //拡大率を記憶する
    Vector3 remember_scale;
    //色を記憶する
    Color remember_color;
    //レイヤーを記憶する
    int remember_layer;

    // Start is called before the first frame update
    void Start()
    {
        circular_motion_angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //移動する
        transform.position += new Vector3(move_force.x * Mathf.Sin(Mathf.Deg2Rad*circular_motion_angle), move_force.y, move_force.z);
        //角度を足す
        circular_motion_angle += angle_force;
        
        //ある程度画面上に行ったら
        if(transform.position.y >= GameObject.Find("Main Camera").transform.position.y+10.0f)
        {
            //カウントを減らす
            GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>().DecreaseDecorationCount(GetComponent<SpriteRenderer>().sortingOrder);
            //自分を消す
            Destroy(this.gameObject);
        }     
    }

    public void SetMoveForce(Vector3 force)
    {
        move_force = force;
    }

    public void SetAngle(float angle)
    {
        angle_force = angle;
    }

    //前のシーンと同じオブジェクトを作成するために情報を覚える
    public void RememberInformation()
    {
        remember_position = transform.position;
        remember_rotation = transform.rotation;
        remember_scale = transform.localScale;
        remember_color = GetComponent<SpriteRenderer>().color;
        remember_layer = GetComponent<SpriteRenderer>().sortingOrder;
    }


    public Vector3 GetPosition()
    {
        return remember_position;
    }

    public Quaternion GetRotation()
    {
        return remember_rotation;
    }

    public Vector3 GetScale()
    {
        return remember_scale;
    }

    public Color GetColor()
    {
        return remember_color;
    }

    public int GetLayer()
    {
        return remember_layer;
    }

}
