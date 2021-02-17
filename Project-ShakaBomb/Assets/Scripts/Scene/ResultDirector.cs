//==============================================================================================
/// File Name	: ResultDirector.cs（修正予定）
/// Summary		: リザルトシーンの管理を行うクラス
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class ResultDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 色のついた星のスプライト
    public Sprite color_star;
    // スピード
    public float frame_speed;
    public float angle_speed;
    // 画像が変わる角度
    public float change_sprite_angle;
    // 回転を止める角度
    public float stop_rotate_angle;
    // 流すBGM
    public AudioClip[] bgm_list;
    // 表示する背景(明るい)
    public Sprite[] bright_background;
    // 表示する背景(暗い)
    public Sprite[] gloomy_background;
    // 仮・浄化率
    public float purification;



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // ステージ選択シーンへ
    private GameObject go_select_stage;
    // 今回の汚染浄化率
    private GameObject text_stage_purification_rate;
    // 浄化率(??%)
    private GameObject text_rate;
    // 浄化率のパーセント
    private GameObject text_percent;
    // 背景([0]…明るい　[1]…暗い)
    private Sprite[] background_sprite = new Sprite[2];
    // 背景のSpriteRenderer
    private SpriteRenderer renderer_background;
    // 画面フェード
    private FadeController sc_screen_fade;
    // 背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    // カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    // ステージ選択シーンへ初期位置
    Vector3 select_stage_start_pos;
    float select_stage_angle;
    // スプライトを切り替える時間(単位：秒)
    private float change_sprite_time;
    // リザルト画面が始まった時間
    private float start_result_time;
    // ランプの数
    private int num_lamp;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        // オブジェクトを探す
        go_select_stage = GameObject.Find("SelectStageButton");
        // コンポーネントを探す
        text_stage_purification_rate = GameObject.Find("StagePurificationRate");
        text_rate = GameObject.Find("PurificationRate");
        text_percent = GameObject.Find("PurificationRatePercent");
        renderer_background = GameObject.Find("BackGround").GetComponent<SpriteRenderer>();
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        select_stage_angle = 0.0f;

        // 座標設定
        text_stage_purification_rate.GetComponent<RectTransform>().anchoredPosition = new Vector3(560, 525, -2000);
        text_rate.GetComponent<RectTransform>().anchoredPosition = new Vector3(560, 225, -2000);
        text_percent.GetComponent<RectTransform>().anchoredPosition = new Vector3(860, 175, -2000);
        go_select_stage.GetComponent<RectTransform>().anchoredPosition = new Vector3(-400, 200, -2000);
        select_stage_start_pos = new Vector3(-200, 100, -2100);

        // プレイシーンから汚染ポイントと残っている汚染ポイントを使って浄化率を求める
        purification = Data.currentCleanRate;
        if (Data.cleanRate.ContainsKey(Data.stage_number))
        {
            Data.cleanRate[Data.stage_number] = Mathf.Max(Data.cleanRate[Data.stage_number], purification);
        }
        else
        {
            Data.cleanRate[Data.stage_number] = purification;
        }
        float rate = purification;
        // パーセンテージに変更
        int percent = (int)(rate * 100);
        // 浄化率のテキストを設定する
        text_rate.GetComponent<Text>().text = percent.ToString();
        // 浄化率からランクを決める
        num_lamp = SharedData.instance.GetPercentRank(percent);
        // 浄化率からテキストの色を設定する
        SetPercentTextPositionColor(num_lamp, percent / 10);
        // 浄化率をBGMを決めて再生する
        PlayPercentBGM(num_lamp);
        // 浄化率から背景を設定する
        SetBackGroundSprite(num_lamp);

        SharedData.instance.SetPurificationRate(percent);



        // フェードインさせる
        FadeManager.fadeColor = Color.black;
        FadeManager.FadeIn(1.5f);

        // Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(GameObject.Find("Main Camera").GetComponent<Camera>());
        // CanvasScalerの設定を変える(画面サイズが変わっても自動的に大きさなどを変更するように)
        SharedData.instance.SetCanvasScaleOption(GameObject.Find("Canvas").GetComponent<CanvasScaler>());

        // スプライトを切り替える時間を設定する(0.5秒～1.0秒の間ランダム)
        change_sprite_time = Random.Range(0.5f, 1.0f);
        // リザルト画面が始まった時間を設定する
        start_result_time = Time.time;
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {

        go_select_stage.GetComponent<RectTransform>().anchoredPosition = new Vector3(select_stage_start_pos.x + (Mathf.Sin(select_stage_angle - 90.0f) * 30.0f), select_stage_start_pos.y, select_stage_start_pos.z);

        select_stage_angle += 0.1f;


        //スプライトを切り替える時間になったら
        if (change_sprite_time + start_result_time < Time.time)
        {
            //スプライトを切り替える&時間を再設定する
            if (renderer_background.sprite == background_sprite[0])
            {
                //暗いスプライトに切り替える
                renderer_background.sprite = background_sprite[1];
                change_sprite_time += Random.Range(0.25f, 0.4f);
            }
            else if (renderer_background.sprite == background_sprite[1])
            {
                //明るいスプライトに切り替える
                renderer_background.sprite = background_sprite[0];
                change_sprite_time += Random.Range(0.25f, 0.75f);
            }
        }

        //何かのキーを押したら
        if ((Input.anyKeyDown ||
            //Aボタンを押したら
            Input.GetAxis(ConstGamePad.BUTTON_A) > 0) &&
            // フェードアウト中でないなら
            !FadeManager.isFadeOut)
        {
            FadeManager.fadeColor = Color.black;
            // ステージ番号が15でランプの数が2つ以上じゃなかったら(最終ステージクリアしていない)
            if (!((Data.stage_number == 15) && (num_lamp >= 2)))
            {
                FadeManager.FadeOut(ConstScene.STAGE_SELECT, ConstScene.FADE_TIME);
            }
            else
            {
                FadeManager.FadeOut(ConstScene.ALL_CLEAR, ConstScene.FADE_TIME);
            }
            // BGMをフェードアウトする
            SoundFadeController.SetFadeOutSpeed(-0.005f);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : パーセントのテキストの色を設定する
    // remarks : none
    // param   : int、int
    // return  : none
    //------------------------------------------------------------------------------------------
    private void SetPercentTextPositionColor(int rank, int num)
    {
        // 今回の汚染浄化率のテキストの座標(評価１の時基準)
        Vector3 stage_purification_pos = new Vector3(560.0f, 500.0f, -2000.0f);
        // パーセンテージの座標(今回の汚染浄化率の差)
        Vector3 purification_rate_pos = new Vector3(0.0f, -220.0f, 0.0f);
        // パーセンテージの単位の座標(パーセンテージの差)
        Vector3 percent_pos = new Vector3(300.0f, -50.0f, 0.0f);
        // 色(初期：黒)
        Color color = new Color(0.0f, 0.0f, 0.0f);

        switch (rank)
        {
            // 29%以下だったら
            case 0:
                // 赤
                color = new Color(1.0f, 0.0f, 0.0f);
                // 座標変更
                stage_purification_pos = new Vector3(stage_purification_pos.x + 700.0f, stage_purification_pos.y, stage_purification_pos.z);
                purification_rate_pos = new Vector3(purification_rate_pos.x, purification_rate_pos.y, purification_rate_pos.z);
                // 割合の桁数によって座標を変える
                if (num > 0)
                {
                    percent_pos = new Vector3(percent_pos.x, percent_pos.y, percent_pos.z);
                }
                else
                {
                    percent_pos = new Vector3(percent_pos.x - 100.0f, percent_pos.y, percent_pos.z);
                }
                break;
            // 69%以下だったら
            case 1:
                // オレンジ
                color = new Color(1.0f, 0.5f, 0.0f);
                // 座標変更
                stage_purification_pos = new Vector3(stage_purification_pos.x, stage_purification_pos.y, stage_purification_pos.z);
                purification_rate_pos = new Vector3(purification_rate_pos.x, purification_rate_pos.y, purification_rate_pos.z);
                percent_pos = new Vector3(percent_pos.x, percent_pos.y, percent_pos.z);
                break;
            // 99%以下だったら
            case 2:
                // 黄
                color = new Color(1.0f, 1.0f, 0.0f);
                // 座標変更
                stage_purification_pos = new Vector3(stage_purification_pos.x, stage_purification_pos.y, stage_purification_pos.z);
                purification_rate_pos = new Vector3(purification_rate_pos.x, purification_rate_pos.y, purification_rate_pos.z);
                percent_pos = new Vector3(percent_pos.x, percent_pos.y, percent_pos.z);
                break;
            // 100%だったら
            case 3:
                // 黄緑
                color = new Color(0.4f, 0.9f, 0.0f);
                // 座標変更
                stage_purification_pos = new Vector3(stage_purification_pos.x + 700.0f, stage_purification_pos.y, stage_purification_pos.z);
                purification_rate_pos = new Vector3(purification_rate_pos.x, purification_rate_pos.y, purification_rate_pos.z);
                percent_pos = new Vector3(percent_pos.x + 50.0f, percent_pos.y, percent_pos.z);
                break;
            // 当てはまらなかったら
            default:
                // 黒
                color = new Color(0.0f, 0.0f, 0.0f);
                break;
        }

        // 座標変更
        text_stage_purification_rate.GetComponent<RectTransform>().anchoredPosition = stage_purification_pos;
        text_rate.GetComponent<RectTransform>().anchoredPosition = stage_purification_pos + purification_rate_pos;
        text_percent.GetComponent<RectTransform>().anchoredPosition = stage_purification_pos + purification_rate_pos + percent_pos;
        // パーセンテージの色
        text_rate.GetComponent<Text>().color = color;
        text_percent.GetComponent<Text>().color = color;
    }



    //------------------------------------------------------------------------------------------
    // summary : BGMを選択し再生する
    // remarks : none
    // param   : int
    // return  : none
    //------------------------------------------------------------------------------------------
    private void PlayPercentBGM(int rank)
    {
        AudioClip clip;
        clip = bgm_list[rank];
        // BGMを再生
        SoundPlayer.PlayBGM(clip);
    }



    //------------------------------------------------------------------------------------------
    // summary : 背景を選択し設定する
    // remarks : none
    // param   : int
    // return  : none
    //------------------------------------------------------------------------------------------
    private void SetBackGroundSprite(int rank)
    {
        // 明るい背景を設定する
        background_sprite[0] = bright_background[rank];
        // 暗い背景を設定する
        background_sprite[1] = gloomy_background[rank];
        // 明るい背景をオブジェクトにセットする
        renderer_background.sprite = background_sprite[0];
    }
}
