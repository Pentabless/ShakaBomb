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
}
