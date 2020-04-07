//======================================================
/// File Name	: Common.cs
/// Summary		: 定数
/// Date		: 2020/03/13
//======================================================
using UnityEngine;
using System.Collections;
//======================================================
namespace Common
{
    // ゲームパッド関連
    public static class GamePad
    {
        // ゲームパッドの検出間隔
        public const float CHECK_INTERVAL = (2.0f);
    }

    // プレイヤー関連の定数
    public static class Player
    {
        // オブジェクト名
        public const string NAME = ("Player");
    }

    // エネミー関連の定数
    public static class Enemy
    {

    }

    // ステージ関連
    public static class Stage
    {
        // タイル
        public const string DAMAGE_TILE = ("DamageTile");
    }

    // バルーン関連の定数
    public static class Balloon
    {
        // オブジェクト名
        public const string NAME = ("BalloonGenerator");
        // 最大バルーン所持数
        public const int MAX = (3);
        // 生成したシャボン玉の所持できる状態の最大時間
        public const int COUNT = (80);
        // プレイヤーとの距離
        public const float DISTANCE_X = (1.2f);
        public const float DISTANCE_Y = (1.7f);
    }
}