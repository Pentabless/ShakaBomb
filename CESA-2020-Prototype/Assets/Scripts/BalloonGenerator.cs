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
    private List<GameObject> m_balloonList = new List<GameObject>();

    // 初期化
    private void Init()
    {
        isCreate = false;
    }

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (isCreate)
        {
            // プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(balloonPrefab) as GameObject;
            // 座標を設定する
            go.transform.position = createPosition;
            // 所持バルーンをカウント
            Data.num_balloon++;
            m_balloonList.Add(go);
            // 作っていない状態にする
            isCreate = false;
        }
    }

    // バルーンを生成
    public void CreateBalloon(Vector3 create_pos)
    {
        createPosition = create_pos;
        isCreate = true;
    }

    // バルーンを使用する(古い順に消費する)
    public void UsedBalloon()
    {
        Destroy(m_balloonList[0]);
        m_balloonList.RemoveAt(0);
        Data.num_balloon--;
    }

    // バルーンが壊された時
    public void BrokenBalloon(int count)
    {
        int num = (count - 1);
        Destroy(m_balloonList[num]);
        m_balloonList.RemoveAt(num);
        Data.num_balloon--;
    }

    // バルーンの現在の所持数を取得
    public int GetMaxBalloons()
    {
        return m_balloonList.Count;
    }
}
