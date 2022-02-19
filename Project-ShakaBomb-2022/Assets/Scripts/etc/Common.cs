//==============================================================================================
/// File Name	: Common.cs
/// Summary		: 定数リスト
//==============================================================================================
namespace Common
{
    //------------------------------------------------------------------------------------------
    // フレームレート関連
    //------------------------------------------------------------------------------------------
    public static class ConstRenderRate
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("ForceRenderRate");
        public static readonly float RATE                 = (60.0f);
    }



    //------------------------------------------------------------------------------------------
    // カメラ関連
    //------------------------------------------------------------------------------------------
    public static class ConstCamera
    {
        // オブジェクト名・タグ名
        public static readonly string NAME           = ("Camera");
        public static readonly string MAIN_CAMERA    = ("Main Camera");
        public static readonly string CONTROLLER     = ("CameraController");
        public static readonly float  CELL_X         = (50.0f);
        public static readonly float  SECOND_CELL_Y  = (26.0f);
        public static readonly float  FIRST_CELL_Y   = (33.0f);
        public static readonly float  SPEED          = (50.0f);
        public static readonly float  POSITION_Z     = (-10.0f);
        public static readonly float  REMEMBER_FRAME = (1.0f);
        public static readonly float  CANNOT_FRAME   = (0.1f);
    }



    //------------------------------------------------------------------------------------------
    // シーン関連
    //------------------------------------------------------------------------------------------
    public static class ConstScene
    {
        // シーン名
        public static readonly string TITLE             = ("TitleScene");
        public static readonly string PROLOGUE          = ("PrologueScene");
        public static readonly string TUTORIAL          = ("TutorialScene");
        public static readonly string STAGE_SELECT      = ("StageSelectScene");
        public static readonly string GAME_PLAY         = ("PlayScene");
        public static readonly string RESULT            = ("ResultScene");
        public static readonly string ALL_CLEAR         = ("GameClearScene");
        public static readonly string CREDIT            = ("CreditScene");
        // フェード時間
        public static readonly float FADE_TIME          = (2.0f);
        public static readonly float SOUND_FADE_TIME    = (0.01f);
    }



    //------------------------------------------------------------------------------------------
    // ゲームパッド関連
    //------------------------------------------------------------------------------------------
    public static class ConstGamePad
    {
        // ゲームパッドの検出間隔
        public static readonly float CHECK_INTERVAL       = (2.0f);
        // 操作
        public static readonly string BUTTON_A            = ("Button A");
        public static readonly string BUTTON_B            = ("Button B");
        public static readonly string BUTTON_X            = ("Button X");
        public static readonly string BUTTON_Y            = ("Button Y");
        public static readonly string HORIZONTAL          = ("Horizontal");
        public static readonly string VERTICAL            = ("Vertical");
    }



    //------------------------------------------------------------------------------------------
    // クリア失敗用フレーム関連
    //------------------------------------------------------------------------------------------
    public static class ConstFailedFrame
    {
        // オブジェクト名
        public static readonly string NAME                = ("FailedFrame");
        // フェード時間
        public static readonly float FADE_TIME            = (1.0f);
    }



    //------------------------------------------------------------------------------------------
    // プレイヤー関連
    //------------------------------------------------------------------------------------------
    public static class ConstPlayer
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("Player");
        public static readonly string CONTROLLER          = ("PlayerController");
        // 操作
        public static readonly string ATTACK              = ("Button B");
        public static readonly string JUMP                = ("Button A");
        public static readonly string BOOST               = ("Button X");
        public static readonly string HORIZONTAL          = ("Horizontal");
        public static readonly string VERTICAL            = ("Vertical");
        // 長押しのインターバル
        public static readonly float PUSH_INTERVAL        = (2.0f);
    }



    //------------------------------------------------------------------------------------------
    // エネミー関連
    //------------------------------------------------------------------------------------------
    public static class ConstEnemy
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("Enemy");
        public static readonly string HIT_STATE           = ("HitEnemy");
        public static readonly string ATTACK              = ("EnemyAttack");
        public static readonly float  BALLON_MOVEMENT     = (1.0f);
    }



    //------------------------------------------------------------------------------------------
    // ステージ関連
    //------------------------------------------------------------------------------------------
    public static class ConstStage
    {
        // オブジェクト名・タグ名
        public static readonly string SRAGE               = ("Stage");
        public static readonly string GROUND              = ("Ground");
        public static readonly string DAMAGE_TILE         = ("DamageTile");
        public static readonly float  NOT_INPUT_COUNT     = (2.0f);
    }



    //------------------------------------------------------------------------------------------
    // 床関連
    //------------------------------------------------------------------------------------------
    public static class ConstFloor
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("Floor");
    }



    //------------------------------------------------------------------------------------------
    // 汚れ関連
    //------------------------------------------------------------------------------------------
    public static class ConstDirt
    {
        // オブジェクト名・タグ名
        public static readonly string MANAGER = ("DirtManager");
    }



    //------------------------------------------------------------------------------------------
    // バブル関連
    //------------------------------------------------------------------------------------------
    public static class ConstBubble
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("Bubble");
        public static readonly string CONTROLLER          = ("BubbleController");
        public static readonly string GENERATOR           = ("BubbleGenerator");
        public static readonly string GROUND              = ("GroundBubble");
        // 消滅時間
        public static readonly int EXTINCTION_TIME        = (160);
        // 泡の最大サイズ
        public static readonly float MAX_SIZE             = (0.8f);
    }



    //------------------------------------------------------------------------------------------
    // バルーン関連
    //------------------------------------------------------------------------------------------
    public static class ConstBalloon
    {
        // オブジェクト名・タグ名
        public static readonly string NAME                = ("Balloon");
        public static readonly string GENERATOR           = ("BalloonGenerator");
        public static readonly string CONTROLLER          = ("BalloonController");
        // 最大バルーン所持数
        public static readonly int MAX                    = (3);
        // 生成したシャボン玉の所持できる状態の最大時間
        public static readonly int COUNT                  = (80);
        // プレイヤーとの距離
        public static readonly float DISTANCE_X           = (1.2f);
        public static readonly float DISTANCE_Y           = (1.7f);
        // ラインレンダラー
        public static readonly float LINE_WIDTH           = (0.05f);
    }



    //------------------------------------------------------------------------------------------
    // ゴール関連
    //------------------------------------------------------------------------------------------
    public static class ConstGoal
    {
        // オブジェクト名・タグ名
        public static readonly string NAME = ("GoalObject");
    }



    //------------------------------------------------------------------------------------------
    // ゴール関連
    //------------------------------------------------------------------------------------------
    public static class ConstDirector
    {
        // オブジェクト名・タグ名
        public static readonly string STAGE_SELECT  = ("StageSelectDirector");
        public static readonly string PLAY          = ("PlayDirector");
    }



    //------------------------------------------------------------------------------------------
    // 整数
    //------------------------------------------------------------------------------------------
    public static class ConstInteger
    {
        public static readonly int ZERO                   = (0);
    }



    //------------------------------------------------------------------------------------------
    // 小数
    //------------------------------------------------------------------------------------------
    public static class ConstDecimal
    {
        public static readonly float ZERO                 = (0.0f);
    }
}