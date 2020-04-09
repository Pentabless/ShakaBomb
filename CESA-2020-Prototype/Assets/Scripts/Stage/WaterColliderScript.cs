using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColliderScript : MonoBehaviour
{
    // 自身または親のRigidbody
    private Rigidbody2D rig;
    // 前回の速度
    public Vector2 preVelocity { get; protected set; }
    // 今回の速度
    public Vector2 nowVelocity { get; protected set; }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        if (!rig)
        {
            rig = GetComponentInParent<Rigidbody2D>();
        }
        preVelocity = nowVelocity = Vector2.zero;
    }

    private void Update()
    {
        // 速度の更新
        preVelocity = nowVelocity;
        nowVelocity = rig.velocity;
    }

    // Y方向の絶対値が大きい方の速度を取得する
    public Vector2 GetFasterVelocity()
    {
        return (Mathf.Abs(preVelocity.y) > Mathf.Abs(nowVelocity.y) ? preVelocity : nowVelocity);
    }
}
