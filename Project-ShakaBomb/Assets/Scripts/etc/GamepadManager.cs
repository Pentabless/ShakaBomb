//==============================================================================================
/// File Name	: GamepadManager.cs
/// Summary		: ゲームパッドの接続確認
//==============================================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class GamepadManager : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private Image m_connected = null;
    [SerializeField]
    private Image m_disconnected = null;

    // ゲームパッドの接続確認
    private bool m_checkGamepad = false;



    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        m_connected.gameObject.SetActive(false);
        m_disconnected.gameObject.SetActive(false);
    }



    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        StartCoroutine(DelayCheck());
    }



    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        DebugCheckGamepad();
    }



    //------------------------------------------------------------------------------------------
    // ゲームパッドの接続確認
    //------------------------------------------------------------------------------------------
    public bool GetCheckGamepad()
    {
        return m_checkGamepad;
    }



    //------------------------------------------------------------------------------------------
    // デバッグ用
    //------------------------------------------------------------------------------------------
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



    //------------------------------------------------------------------------------------------
    // ゲームパッドの接続確認
    //------------------------------------------------------------------------------------------
    IEnumerator DelayCheck()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(ConstGamePad.CHECK_INTERVAL);

            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                if (!string.IsNullOrEmpty(Input.GetJoystickNames()[i]))
                {
                    // ゲームパッドが接続されている
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = true;
                }
                else
                {
                    // ゲームパッドが接続されていない
                    i = Input.GetJoystickNames().Length;
                    m_checkGamepad = false;
                }
            }
        }
    }
}
