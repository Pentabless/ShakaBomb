using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // ゲームパッドの接続確認
    private bool m_checkGamepad = false;

    private void Awake()
    {
        // 接続されているゲームパッドの名前を調べる
        var gamepadNames = Input.GetJoystickNames();

        // ゲームパッドが接続されているかどうか
        if (gamepadNames == null || gamepadNames[0] == "")
        {
            m_checkGamepad = false;
            Debug.Log("ゲームパッドが接続されていません");
        }
        else
        {
            m_checkGamepad = true;
            Debug.Log("「" + gamepadNames[0] + "」が接続されました");
        }
    }

    // ゲームパッドの接続確認
    public bool GetCheckGamepad()
    {
        return m_checkGamepad;
    }
}
