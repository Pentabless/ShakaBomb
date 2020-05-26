using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField]
    // Bulletプレハブ
    private GameObject bulletPrefab;

    [SerializeField]
    // 生成できない時間
    private float cantCreateTime = 0.1f;
    // 残り生成できない時間
    private float remainingTime = 0.0f;
    [SerializeField]
    // 発射方向に応じて位置をずらす量
    private float offsetlength = 0.5f;


    void Start()
    {
    }

    void Awake()
    {

    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
    }

    public bool BulletCreate(Vector3 playerPos, float angle)
    {
        if (remainingTime > 0.0f)
        {
            return false;
        }
        Debug.Log("s:" + angle);
        // 位置を発射方向にずらす
        playerPos += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f) * offsetlength;

        // プレファブと同じオブジェクトを作る
        GameObject go = Instantiate(bulletPrefab, playerPos, Quaternion.identity);
        go.GetComponent<BulletController>().SetAngle(angle);

        remainingTime = cantCreateTime;
        return true;
    }
}
