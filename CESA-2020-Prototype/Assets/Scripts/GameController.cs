using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GameController : MonoBehaviour
{
    // ゲームパッドの接続確認
    private bool m_checkGamepad = false;

    private void Awake()
    {
        StartCoroutine(DelayCheck());
    }

    // ゲームパッドの接続確認
    public bool GetCheckGamepad()
    {
        return m_checkGamepad;
    }

    IEnumerator DelayCheck()
    {
        Debug.Log("check 3");

        while (true)
        {
            yield return new WaitForSecondsRealtime(GamePad.CHECK_INTERVAL);
            Debug.Log("check 2");

            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                Debug.Log("check 1");
                if (!string.IsNullOrEmpty(Input.GetJoystickNames()[i]))
                {
                    Debug.Log("ゲームパッドが接続されました");
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = true;
                }
                else
                {
                    Debug.Log("ゲームパッドが接続されていません");
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = false;
                }
            }
        }
    }
}
