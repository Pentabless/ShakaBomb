//======================================================
/// File Name	: BalloonGenerator.cs
/// Summary		: バルーン生成管理
//======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
//======================================================
public class BalloonGenerator : MonoBehaviour
{
    // プレイヤーの取得
    [SerializeField]
    private PlayerController m_playerController = null;
    // バルーンの取得
    [SerializeField]
    private GameObject m_balloon = null;

    private bool m_isCreate = false;
    private Vector3 createPosition;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (m_isCreate)
        {
            // バルーンを生成
            CreateOneBalloon();

            // 作っていない状態にする
            m_isCreate = false;
        }
    }

    // 初期化
    private void Init()
    {
        m_isCreate = false;
    }

    // バルーンを生成できるようにする
    public void CreateBalloon(Vector3 create_pos)
    {
        createPosition = create_pos;
        m_isCreate = true;
    }

    // バルーンをひとつ生成する
    public void CreateOneBalloon()
    {
        // バルーンを生成する
        GameObject go = Instantiate(m_balloon) as GameObject;
        // 生成したバルーンを子オブジェクトに登録する
        go.transform.parent = this.transform;
        // 座標を設定する
        go.transform.position = createPosition;
        // プレイヤーのバルーン所持リストに追加
        m_playerController.AddBalloon(go);
    }

    // バルーンを使用する(古い順に消費する)
    public void UsedBalloon()
    {
        m_playerController.UsedBalloon();
    }

    // バルーンが壊された時
    public void BrokenBalloon()
    {
        m_playerController.BrokenBalloon();
    }

    // バルーンの現在の所持数を取得
    public int GetMaxBalloons()
    {
        return m_playerController.GetMaxBalloons(); ;
    }
}
