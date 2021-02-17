//==============================================================================================
/// File Name	: GamepadManager.cs
/// Summary		: ゲームパッドの接続確認
//==============================================================================================
using System.Collections;
using UnityEngine;
using Common;
//==============================================================================================
public class GamepadManager : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // ゲームパッドの接続確認
    private bool checkGamepad = false;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {

    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {

    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {

    }



    //------------------------------------------------------------------------------------------
    // summary : ゲームパッドの接続確認
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public bool GetCheckGamepad()
    {
        return checkGamepad;
    }



    //------------------------------------------------------------------------------------------
    // summary : ゲームパッドの接続確認
    // remarks : none
    // param   : none
    // return  : none
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
                    checkGamepad = true;
                    Debug.Log("接続されている");
                }
                else
                {
                    // ゲームパッドが接続されていない
                    i = Input.GetJoystickNames().Length;
                    checkGamepad = false;
                    Debug.Log("接続されていない");
                }
            }
        }
    }
}
