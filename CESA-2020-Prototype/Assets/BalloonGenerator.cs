using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BalloonGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject balloonPrefab;

    bool isCreate;

    Vector3 createPosition;

    // 所持バルーン
    private GameObject[] m_balloons = new GameObject[Balloon.MAX];
    //private List<GameObject> m_balloonList = new List<GameObject>();

    void Awake()
    {
        isCreate = false;

        for (int i = 0; i < m_balloons.Length; i++)
            m_balloons[i] = null;
    }

    void Update()
    {
        if (isCreate)
        {
            if (m_balloons[Data.num_balloon] == null)
            {
                // プレファブと同じオブジェクトを作る
                m_balloons[Data.num_balloon] = Instantiate(balloonPrefab) as GameObject;
                // 座標を設定する
                m_balloons[Data.num_balloon].transform.position = createPosition;
                // 所持バルーンをカウント
                Data.num_balloon++;

                // デバッグ
                Debug.Log("所持しているバルーン " + Data.num_balloon + " / " + Balloon.MAX + "個");
            }
            // 作っていない状態にする
            isCreate = false;
        }
    }

    public void CreateBalloon(Vector3 create_pos)
    {
        createPosition = create_pos;
        isCreate = true;
    }
}
