using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common;

public class GameController : MonoBehaviour
{
    // ゲームパッドの接続確認
    private bool m_checkGamepad = false;

    //private string[] m_cacheJoystickNames;

    private void Awake()
    {
        StartCoroutine(DelayCheck());
    }

    private void Update()
    {

    }

    // ゲームパッドの接続確認
    public bool GetCheckGamepad()
    {
        return m_checkGamepad;
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
