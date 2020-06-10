//==============================================================================================
/// File Name	: Data.cs
/// Summary		: データ保持
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================================
public class Data
{
    public static float balloonSize = 0;
    public static int num_balloon = 0;
    public static int playerDir = 0;
    public static float playerVelX;
    public static Vector2 currentPlayerVel = Vector2.zero;
    public static Vector2 prePlayerVel = Vector2.zero;
    public static UnityEngine.Vector3 initialPlayerPos;

    public static int colletcObject = 0;            // プレイ中のステージで獲得したCollectObjectの数
    public static Dictionary<int, float> cleanRate = new Dictionary<int, float>();

    public static float timeLimit = 0.0f;
    public static float time = 0.0f;
    public static int star_num = 0;

    public static int stage_number;
}
