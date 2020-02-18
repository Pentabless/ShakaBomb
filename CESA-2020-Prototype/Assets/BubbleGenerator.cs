using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    public GameObject bubblePrefab;

    bool isCreate;

    // Start is called before the first frame update
    void Start()
    {
        isCreate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //作ろうとしている状態にする
            isCreate = true;
        }

        if (isCreate)
        {
            //プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(bubblePrefab) as GameObject;
            //座標を設定する
            go.transform.position = GameObject.Find("Player").transform.position;
            //作っていない状態にする
            isCreate = false;
        }
    }

    public void CreateBubble(bool create)
    {
        isCreate = create;
    }


    public void BubbleCreate()
    {
        isCreate = true;
    }
}
