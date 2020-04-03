using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // ゲームパッド情報の取得
    [SerializeField]
    private GameController m_gamepadState = null;

    // ポーズメニューの取得
    [SerializeField]
    private Canvas m_pauseMenuCanvas = null;

    //[SerializeField]
    //private Button 

    private bool m_checkGamepad = false;
    private bool m_activeCheck = false;

    private void Start()
    {
        // ポーズメニューを非表示
        m_pauseMenuCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ゲームパッドの接続状況確認
        m_checkGamepad = m_gamepadState.GetCheckGamepad();

        SwitchMenu(m_activeCheck);
    }

    public void ResumeGame()
    {
        // ゲームを再開する
        m_pauseMenuCanvas.gameObject.SetActive(false);
    }

    private void SwitchMenu(bool active)
    {
        if (!active)
            DisplayMenu();
        else
            HiddenMenu();
    }

    private void DisplayMenu()
    {
        // ポーズメニューの表示（ゲームパッド未接続時）
        if (Input.GetKeyDown(KeyCode.Escape) && !m_checkGamepad)
        {
            m_activeCheck = true;
            m_pauseMenuCanvas.gameObject.SetActive(true);
        }
        // ポーズメニューの表示（ゲームパッド接続時）
        if (Input.GetKeyDown(KeyCode.Joystick1Button7) && m_checkGamepad)
        {
            m_activeCheck = true;
            m_pauseMenuCanvas.gameObject.SetActive(true);
        }
    }

    private void HiddenMenu()
    {
        // ポーズメニューの非表示（ゲームパッド未接続時）
        if (Input.GetKeyDown(KeyCode.Escape) && !m_checkGamepad)
        {
            m_activeCheck = false;
            m_pauseMenuCanvas.gameObject.SetActive(false);
        }
        // ポーズメニューの非表示（ゲームパッド接続時）
        if (Input.GetKeyDown(KeyCode.Joystick1Button7) && m_checkGamepad)
        {
            m_activeCheck = false;
            m_pauseMenuCanvas.gameObject.SetActive(false);
        }
    }
}
