using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Common;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Image m_connected = null;
    [SerializeField]
    private Image m_disconnected = null;

    // ゲームパッドの接続確認
    private bool m_checkGamepad = false;

    private void Awake()
    {
        m_connected.gameObject.SetActive(false);
        m_disconnected.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(DelayCheck());
    }

    private void Update()
    {
        DebugCheckGamepad();
    }

    // ゲームパッドの接続確認
    public bool GetCheckGamepad()
    {
        return m_checkGamepad;
    }

    // デバッグ用
    private void DebugCheckGamepad()
    {
        if (m_checkGamepad)
        {
            m_connected.gameObject.SetActive(true);
            m_disconnected.gameObject.SetActive(false);
        }
        else
        {
            m_connected.gameObject.SetActive(false);
            m_disconnected.gameObject.SetActive(true);
        }
    }

    IEnumerator DelayCheck()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(GamePad.CHECK_INTERVAL);

            Debug.Log(Input.GetJoystickNames());

            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                if (!string.IsNullOrEmpty(Input.GetJoystickNames()[i]))
                {
                    //Debug.Log("ゲームパッドが接続されました");
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = true;
                }
                else
                {
                    //Debug.Log("ゲームパッドが接続されていません");
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = false;
                }
            }
        }
    }
}
