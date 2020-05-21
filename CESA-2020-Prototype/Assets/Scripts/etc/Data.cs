//==============================================================================================
/// File Name	: Data.cs
/// Summary		: データ保持
//==============================================================================================
using System.Collections;
//==============================================================================================
public class Data
{
    public static float balloonSize = 0;
    public static int num_balloon = 0;
    public static int playerDir = 0;
    public static float playerVelX;
    
    public static int colletcObject = 0;            // プレイ中のステージで獲得したCollectObjectの数
    
    public static float timeLimit = 0.0f;
    public static float time = 0.0f;
    public static int star_num = 0;
}
