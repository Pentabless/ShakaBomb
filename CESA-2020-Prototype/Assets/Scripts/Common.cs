//======================================================
/// File Name	: Common.cs
/// Summary		: 定数
/// Date		: 2020/03/13
//======================================================
using UnityEngine;
using System.Collections;
//======================================================
//【使用方法】
// ① 使いたいスクリプト内で「using Common;」
// ② 使いたい箇所で任意のクラス、定数を入力
// 　 例：バルーンの最大所持数なら「Balloon.MAX」
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

    }

    // エネミー関連の定数
    public static class Enemy
    {

    }

    // バルーン関連の定数
    public static class Balloon
    {
        // 最大バルーン所持数
        public const int MAX = (3);
        // 生成したシャボン玉の所持できる状態の最大時間
        public const int COUNT = (80);
    }
}