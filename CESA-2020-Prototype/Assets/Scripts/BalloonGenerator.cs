using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BalloonGenerator : MonoBehaviour
{
    // プレイヤーの取得
    [SerializeField]
    private PlayerController m_playerController = null;

    // バルーンの取得
    [SerializeField]
    private GameObject m_balloonPrefab;

    private bool m_isCreate;

    Vector3 createPosition;

    // 初期化
    private void Init()
    {
        m_isCreate = false;
    }

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (m_isCreate)
        {
            // プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(m_balloonPrefab) as GameObject;
            // 座標を設定する
            go.transform.position = createPosition;
            // プレイヤーのバルーン所持リストに追加
            m_playerController.AddBalloon(go);
            // 作っていない状態にする
            m_isCreate = false;
        }
    }

    // バルーンを生成
    public void CreateBalloon(Vector3 create_pos)
    {
        createPosition = create_pos;
        m_isCreate = true;
    }

    // バルーンを使用する(古い順に消費する)
    public void UsedBalloon()
    {
        m_playerController.UsedBalloon();
    }

    // バルーンが壊された時
    public void BrokenBalloon(int count)
    {
        m_playerController.BrokenBalloon(count);
    }

    // バルーンの現在の所持数を取得
    public int GetMaxBalloons()
    {
        return m_playerController.GetMaxBalloons(); ;
    }
}
