//==============================================================================================
/// File Name	: GameInitial
/// Summary		: 画面サイズ設定
//==============================================================================================
using UnityEngine;

public class GameInitial
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Screen.SetResolution(1920, 1080, true);
    }
}