using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject balloonPrefab;

    bool isCreate;

    readonly int maxBalloon = 5;

    Vector3 createPosition;

    // Start is called before the first frame update
    void Start()
    {
        isCreate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCreate)
        {
            Data.num_balloon++;
            //プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(balloonPrefab) as GameObject;
            //座標を設定する
            go.transform.position = createPosition;
            //作っていない状態にする
            isCreate = false;
        }
        // デバッグ用
        Debug.Log("Balloon = " + Data.num_balloon);
    }

    public void CreateBalloon(Vector3 create_pos)
    {
        createPosition = create_pos;
        isCreate = true;
    }
}
