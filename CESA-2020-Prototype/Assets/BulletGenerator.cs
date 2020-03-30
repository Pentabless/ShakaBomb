using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;

    bool isCreate;

    Vector3 createPosition;


    void Start()
    {
    }

    void Awake()
    {
        isCreate = false;
    }

    void Update()
    {

    }

    public void BulletCreate(Vector3 playerPos)
    {
        // プレファブと同じオブジェクトを作る
        GameObject go = Instantiate(bulletPrefab) as GameObject;
        createPosition = playerPos;
        createPosition.y += 0.5f;
        // 座標を設定する
        go.transform.position = createPosition;
        // 作っていない状態にする
        isCreate = false;
    }
}
