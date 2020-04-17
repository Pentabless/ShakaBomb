using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static int num_balloon = 0;
    public static int playerDir = 0;
    public static float playerVelX;
    
    public static int colletcObject = 0;            // プレイ中のステージで獲得したCollectObjectの数
    
    public static float timeLimit = 0.0f;
    public static float time = 0.0f;
    public static int star_num = 0;
}
