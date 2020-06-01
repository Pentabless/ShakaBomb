//==============================================================================================
/// File Name	: Common.cs
/// Summary		: 定数
//==============================================================================================
using UnityEngine;
using System.Collections;
//==============================================================================================
namespace Common
{
    //------------------------------------------------------------------------------------------
    // フレームレート関連
    //------------------------------------------------------------------------------------------
    public static class RenderRate
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("ForceRenderRate");
        public const float RATE                 = (60.0f);
    }

    //------------------------------------------------------------------------------------------
    // カメラ関連
    //------------------------------------------------------------------------------------------
    public static class Camera
    {
        // オブジェクト名・タグ名
        public const string NAME           = ("Camera");
        public const string MAIN_CAMERA    = ("Main Camera");
        public const string CONTROLLER     = ("CameraController");
        public const float  CELL_X         = 50.0f;
        public const float  CELL_Y         = 14.0f;
        public const float  SPEED          = 50.0f;
        public const float  POSITION_Z     = -10.0f;
        public const float  REMEMBER_FRAME = 1.0f;
        public const float  CANNOT_FRAME   = 0.1f;
    }

    //------------------------------------------------------------------------------------------
    // ゲームパッド関連
    //------------------------------------------------------------------------------------------
    public static class GamePad
    {
        // ゲームパッドの検出間隔
        public const float CHECK_INTERVAL       = (2.0f);
        // 操作
        public const string BUTTON_A            = ("Button A");
        public const string BUTTON_B            = ("Button B");
        public const string BUTTON_X            = ("Button X");
        public const string BUTTON_Y            = ("Button Y");
        public const string HORIZONTAL          = ("Horizontal");
        public const string VERTICAL            = ("Vertical");
    }

    //------------------------------------------------------------------------------------------
    // クリア失敗用フレーム関連
    //------------------------------------------------------------------------------------------
    public static class FailedFrame
    {
        // オブジェクト名
        public const string NAME                = "FailedFrame";
        // フェード時間
        public const float FADE_TIME            = (1.0f);
    }

    //------------------------------------------------------------------------------------------
    // プレイヤー関連
    //------------------------------------------------------------------------------------------
    public static class Player
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("Player");
        public const string CONTROLLER          = ("PlayerController");
        // 操作
        public const string ATTACK              = ("Button B");
        public const string JUMP                = ("Button A");
        public const string BOOST               = ("Button X");
        public const string HORIZONTAL          = ("Horizontal");
        public const string VERTICAL            = ("Vertical");
        // 長押しのインターバル
        public const float PUSH_INTERVAL        = 2.0f;
    }

    //------------------------------------------------------------------------------------------
    // エネミー関連
    //------------------------------------------------------------------------------------------
    public static class Enemy
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("Enemy");
        public const string HIT_STATE           = ("HitEnemy");
        public const string ATTACK              = ("EnemyAttack");
        public const float  BALLON_MOVEMENT      = 1.0f;
    }

    //------------------------------------------------------------------------------------------
    // ステージ関連
    //------------------------------------------------------------------------------------------
    public static class Stage
    {
        // オブジェクト名・タグ名
        public const string SRAGE               = ("Stage");
        public const string GROUND              = ("Ground");
        public const string DAMAGE_TILE         = ("DamageTile");
    }

    //------------------------------------------------------------------------------------------
    // 床関連
    //------------------------------------------------------------------------------------------
    public static class Floor
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("Floor");
    }

    //------------------------------------------------------------------------------------------
    // バブル関連
    //------------------------------------------------------------------------------------------
    public static class Bubble
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("Bubble");
        public const string CONTROLLER          = ("BubbleController");
        public const string GROUND              = ("GroundBubble");
        // 消滅時間
        public const int EXTINCTION_TIME        = (160);
        // 泡の最大サイズ
        public const float MAX_SIZE             = (0.8f);
    }

    //------------------------------------------------------------------------------------------
    // バルーン関連
    //------------------------------------------------------------------------------------------
    public static class Balloon
    {
        // オブジェクト名・タグ名
        public const string NAME                = ("Balloon");
        public const string GENERATOR           = ("BalloonGenerator");
        public const string CONTROLLER          = ("BalloonController");
        // 最大バルーン所持数
        public const int MAX                    = (3);
        // 生成したシャボン玉の所持できる状態の最大時間
        public const int COUNT                  = (80);
        // プレイヤーとの距離
        public const float DISTANCE_X           = (1.2f);
        public const float DISTANCE_Y           = (1.7f);
        // ラインレンダラー
        public const float LINE_WIDTH           = (0.05f);
    }

    //------------------------------------------------------------------------------------------
    // 整数
    //------------------------------------------------------------------------------------------
    public static class Integer
    {
        public const int ZERO                   = (0);
    }

    //------------------------------------------------------------------------------------------
    // 小数
    //------------------------------------------------------------------------------------------
    public static class Decimal
    {
        public const float ZERO                 = (0.0f);
    }
}