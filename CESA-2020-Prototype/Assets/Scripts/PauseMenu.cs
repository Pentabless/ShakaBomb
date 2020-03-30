using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // ゲームパッド情報の取得
    [SerializeField]
    private GameController m_gamepadState = null;
    private bool m_checkGamepad = false;

    // メニューオブジェクトの取得
    [SerializeField]
    private Canvas m_menu = null;

    private void Start()
    {
        // メニューオブジェクトを非表示
        m_menu.gameObject.SetActive(false);
        // ゲームパッドの接続状況確認
        m_checkGamepad = m_gamepadState.GetCheckGamepad();
    }

    private void Update()
    {
        // メニューの表示（ゲームパッド未接続時）
        if (Input.GetKeyDown(KeyCode.Escape) && !m_checkGamepad)
        {
            m_menu.gameObject.SetActive(true);
        }
        // メニューの表示（ゲームパッド接続時）
        if (Input.GetKeyDown(KeyCode.Joystick1Button7) && m_checkGamepad)
        {
            m_menu.gameObject.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        m_menu.gameObject.SetActive(false);
    }
}
