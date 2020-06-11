/*------------------------------------------------------*/
/*--ファイル名：ResultDirector.cs-----------------------*/
/*--概要：リザルトシーンの処理(プレイ評価やシーン遷移)--*/
/*------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI


public class ResultDirector : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
    //色のついた星のスプライト
    public Sprite color_star;
    //スピード
    public float frame_speed;
    public float angle_speed;
    //画像が変わる角度
    public float change_sprite_angle;
    //回転を止める角度
    public float stop_rotate_angle;
    //流すBGM
    public AudioClip[] bgm_list;
    //表示する背景(明るい)
    public Sprite[] bright_background;
    //表示する背景(暗い)
    public Sprite[] gloomy_background;
    //仮　浄化率
    public float purification;
    /*-----------*/
    /*--private--*/
    /*-----------*/
    //リザルトフレーム
    //private GameObject go_result_frame;
    ////評価の星
    //private GameObject[] go_rank_star = new GameObject[3];
    //ステージ選択シーンへ
    private GameObject go_select_stage;
    //今回の汚染浄化率
    private GameObject text_stage_purification_rate;
    //浄化率(??%)
    private GameObject text_rate;
    //浄化率のパーセント
    private GameObject text_percent;
    //背景([0]…明るい　[1]…暗い)
    private Sprite[] background_sprite = new Sprite[2];
    //背景のSpriteRenderer
    private SpriteRenderer renderer_background;
    //画面フェード
    private FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    //カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    ////星を回転させたかどうか
    //private bool[] rotate_star = new bool[3];
    ////評価の星の数
    //private int num_rank_star;

    //ステージ選択シーンへ初期位置
    Vector3 select_stage_start_pos;
    //
    float select_stage_angle;

    //スプライトを切り替える時間(単位：秒)
    private float change_sprite_time;
    //リザルト画面が始まった時間
    private float start_result_time;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //オブジェクトを探す
        //go_result_frame = GameObject.Find("ResultFrame");
        go_select_stage = GameObject.Find("SelectStageButton");
        //コンポーネントを探す
        text_stage_purification_rate = GameObject.Find("StagePurificationRate");
        text_rate = GameObject.Find("PurificationRate");
        text_percent = GameObject.Find("PurificationRatePercent");
        renderer_background = GameObject.Find("BackGround").GetComponent<SpriteRenderer>();
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        select_stage_start_pos = go_select_stage.GetComponent<RectTransform>().localPosition;
        select_stage_angle = 0.0f;
        //座標変更
        //go_result_frame.transform.position = new Vector3(-30.0f, 2.0f, 0.0f);
        //stage_purification_rate.rectTransform.anchoredPosition = new Vector3(-Screen.width - 300.0f, 200.0f, 0.0f);

        //*----浄化率のテキストの設定----*//
        //プレイシーンから汚染ポイントと残っている汚染ポイントを使って浄化率を求める
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
        //パーセンテージに変更
        int percent = (int)(rate * 100);
        //浄化率のテキストを設定する
        text_rate.GetComponent<Text>().text = percent.ToString();
        //浄化率からランクを決める
        int rank = SharedData.instance.GetPercentRank(percent);
        //浄化率からテキストの色を設定する
        SetPercentTextPositionColor(rank,percent/10);
        //浄化率をBGMを決めて再生する
        PlayPercentBGM(rank);
        //浄化率から背景を設定する
        SetBackGroundSprite(rank);

        //*----SharedDataにあるステージデータに記録する----*//
        //SharedData.instance.SetPurificationRate(percent);


        //○○ステージクリア！　のテキストを設定する
        //stage_purification_rate.text = SharedData.instance.SetStageNameEnglish(SharedData.instance.play_stage_number) + " クリア!";
        //スコア　のテキストを設定する
        //text_score.text = "Score:" + score.ToString();
        //評価の星の数を決める
        //SetNumRankStar(score);
        //評価する星の数から評価する星の設定をする
        //SetRankStar(num_rank_star);
        //SetRankStar(Data.star_num);


        //フェードインさせる
        //FadeManager.FadeIn(1.5f);

        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
        //Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(GameObject.Find("Main Camera").GetComponent<Camera>());
        //CanvasScalerの設定を変える(画面サイズが変わっても自動的に大きさなどを変更するように)
        SharedData.instance.SetCanvasScaleOption(GameObject.Find("Canvas").GetComponent<CanvasScaler>());

        ////Canvasの設定を変える(選択フレーム)
        //SharedData.instance.SetCanvasOption(GameObject.Find("SelectFrame").GetComponent<Canvas>());
        ////CanvasScalerの設定を変える
        //SharedData.instance.SetCanvasScaleOption(GameObject.Find("SelectFrame").GetComponent<CanvasScaler>());
        ////オブジェクト「Canvas」より前に設定する
        //GameObject.Find("SelectFrame").GetComponent<Canvas>().sortingOrder = 10;


        //スプライトを切り替える時間を設定する(0.5秒～1.0秒の間ランダム)
        change_sprite_time = Random.Range(0.5f, 1.0f);
        //リザルト画面が始まった時間を設定する
        start_result_time = Time.time;

    }
    /*--終わり：Start--*/

    /*------------------*/
    /*--関数名：Update--*/
    /*--概要：更新------*/
    /*--引数：なし------*/
    /*--戻り値：なし----*/
    /*------------------*/
    void Update()
    {

        go_select_stage.GetComponent<RectTransform>().localPosition = new Vector3(select_stage_start_pos.x + (Mathf.Sin(select_stage_angle-90.0f)*30.0f), select_stage_start_pos.y, select_stage_start_pos.z);

        select_stage_angle += 0.1f;

        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 1.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(/*Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f)*/1.0f, 1.0f, 1.0f, 1.0f), -10);

        //フェードアウトしていたら
        if ((sc_screen_fade.GetFadeType() == true) && sc_screen_fade.GetFadeValue() > 0.0f)
        {
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 1.0f);
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(/*Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f)*/1.0f, 1.0f, 1.0f, 1.0f), 10);
        }

        //スプライトを切り替える時間になったら
        if (change_sprite_time + start_result_time < Time.time)
        {
            //スプライトを切り替える&時間を再設定する
            if (renderer_background.sprite == background_sprite[0])
            {
                //暗いスプライトに切り替える
                renderer_background.sprite = background_sprite[1];
                change_sprite_time += Random.Range(0.25f, 0.4f);    //短め
            }
            else if (renderer_background.sprite == background_sprite[1])
            {
                //明るいスプライトに切り替える
                renderer_background.sprite = background_sprite[0];
                change_sprite_time += Random.Range(0.25f, 0.75f);     //長め
            }
        }

        ////リザルトフレーム(ステージクリアの後ろにある旗みたいなもの)
        //if (go_result_frame.transform.position.x != 0.0f)
        //{
        //    go_result_frame.transform.position += new Vector3(frame_speed, 0.0f, 0.0f);
        //    if (go_result_frame.transform.position.x > 0.0f)
        //    {
        //        go_result_frame.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
        //    }
        //}

        ////ステージクリア(テキストUI)
        //if (text_stage_purification_rate.rectTransform.anchoredPosition.x != 0.0f)
        //{
        //    text_stage_purification_rate.rectTransform.anchoredPosition += new Vector2((frame_speed * Screen.width) / 30.0f, 0.0f);
        //    if (text_stage_purification_rate.rectTransform.anchoredPosition.x > 0.0f)
        //    {
        //        text_stage_purification_rate.rectTransform.anchoredPosition = new Vector3(0.0f, 200.0f);
        //    }
        //}

        //////評価する星
        ////for (int i = 0; i < go_rank_star.Length; i++)
        ////{
        ////    //回転できるかどうか
        ////    bool rotate_check = true;

        ////    //一番目じゃなかったら
        ////    if (i != 0)
        ////    {
        ////        //一つ前の星が回転終わっていなかったら
        ////        if (rotate_star[i - 1] == false)
        ////        {
        ////            rotate_check = false;
        ////        }
        ////    }

        ////    //回転できる状態で　回転済みじゃなかったら
        ////    if (rotate_check == true && rotate_star[i] == false)
        ////    {
        ////        //回転(角度スピードを足していく)
        ////        go_rank_star[i].transform.Rotate(new Vector3(0.0f, angle_speed, 0.0f));
        ////        //条件があっていたら画像を変える
        ////        ChangeStarSprite(i);
        ////        //条件に当てはまっていたら回転を止める
        ////        StopRotateStar(i);
        ////    }
        ////}

        //何かのキーを押したら
        if (Input.anyKeyDown ||
            //Aボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_A) > 0))
        {
            //フェードを始める
            sc_screen_fade.SetFadeType(true);
            //BGMをフェードアウトする
            SoundFadeController.SetFadeOutSpeed(-0.005f);
            //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
            SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
        }

        //フェードアウトが終わったら
        if (sc_screen_fade.GetFadeValue() == 1.0f)
        {
            //SharedDataにあるリストに飾りを入れる
            SharedData.instance.SetDecorationList(GameObject.Find("Main Camera").transform.position);
            //ステージ選択画面に移る
            SceneManager.LoadScene("NewStageSelectScene");
        }
    }
    /*--終わり：Update--*/

    /*------------------------------------------------------------*/
    /*--関数名：SetPercentTextPositionColor(private)--------------*/
    /*--概要：パーセントのテキストの色を設定する------------------*/
    /*--引数：ランク(int)、割合を10で割った数(一桁の時は0になる)--*/
    /*--戻り値：なし----------------------------------------------*/
    /*------------------------------------------------------------*/
    private void SetPercentTextPositionColor(int rank,int num)
    {
        Vector3 pos = new Vector3(400.0f, -300.0f,-2000.0f);
        switch (rank)
        {
            //29%以下だったら
            case 0:
                //赤　(仮)
                text_rate.GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f);
                text_percent.GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f);
                //座標変更
                text_stage_purification_rate.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, 0.0f, pos.z);
                text_rate.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y, pos.z);
                //割合の桁数によって座標を変える
                if (num > 0)
                {
                    text_percent.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + 300.0f, pos.y - 50.0f, pos.z);
                }
                else
                {
                    text_percent.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + 200.0f, pos.y - 50.0f, pos.z);
                }
                break;
            //69%以下だったら
            case 1:
                //オレンジ　(仮)
                text_rate.GetComponent<Text>().color = new Color(1.0f, 0.5f, 0.0f);
                text_percent.GetComponent<Text>().color = new Color(1.0f, 0.5f, 0.0f);
                //座標変更
                text_stage_purification_rate.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x, 0.0f, pos.z);
                text_rate.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x, pos.y, pos.z);
                text_percent.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x + 300.0f, pos.y - 50.0f, pos.z);
                break;
            //99%以下だったら
            case 2:
                //黄　(仮)
                text_rate.GetComponent<Text>().color = new Color(1.0f, 1.0f, 0.0f);
                text_percent.GetComponent<Text>().color = new Color(1.0f, 1.0f, 0.0f);
                //座標変更
                text_stage_purification_rate.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x, 0.0f, pos.z);
                text_rate.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x, pos.y, pos.z);
                text_percent.GetComponent<RectTransform>().localPosition = new Vector3(-pos.x + 300.0f, pos.y - 50.0f, pos.z);
                break;
            //100%だったら
            case 3:
                //黄緑　(仮)
                text_rate.GetComponent<Text>().color = new Color(0.4f, 0.9f, 0.0f);
                text_percent.GetComponent<Text>().color = new Color(0.4f, 0.9f, 0.0f);
                //座標変更
                text_stage_purification_rate.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, 0.0f, pos.z);
                text_rate.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y, pos.z);
                text_percent.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + 350.0f, pos.y - 50.0f, pos.z);
                break;
            //当てはまらなかったら(絶対にないと思う)
            default:
                //黒　(仮)
                text_rate.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f);
                text_percent.GetComponent<Text>().color = new Color(0.0f, 0.0f, 0.0f);
                break;
        }
    }
    /*--終わり：SetPercentTextPositionColor--*/

    /*-----------------------------------*/
    /*--関数名：PlayPercentBGM(private)--*/
    /*--概要：BGMを決めて再生する--------*/
    /*--引数：ランク(int)----------------*/
    /*--戻り値：なし---------------------*/
    /*-----------------------------------*/
    private void PlayPercentBGM(int rank)
    {
        AudioClip clip;
        clip = bgm_list[rank];
        //BGMを流す
        SoundPlayer.PlayBGM(clip);
    }
    /*--終わり：PlayPercentBGM--*/

    /*----------------------------------------*/
    /*--関数名：SetBackGroundSprite(private)--*/
    /*--概要：背景を決めて設定する------------*/
    /*--引数：ランク(int)---------------------*/
    /*--戻り値：なし--------------------------*/
    /*----------------------------------------*/
    private void SetBackGroundSprite(int rank)
    {
        //明るい背景を設定する
        background_sprite[0] = bright_background[rank];
        //暗い背景を設定する
        background_sprite[1] = gloomy_background[rank];

        //明るい背景をオブジェクトにセットする
        renderer_background.sprite = background_sprite[0];
    }
    /*--終わり：PlayPercentBGM--*/

    /////*-----------------------------------*/
    /////*--関数名：SetNumRankStar(private)--*/
    /////*--概要：評価の星の数を求める-------*/
    /////*--引数：スコア(int)----------------*/
    /////*--戻り値：なし---------------------*/
    /////*-----------------------------------*/
    ////private void SetNumRankStar(int check_score)
    ////{
    ////    //スコアから評価の星の数を求める(仮)
    ////    if (check_score >= 1000)
    ////    {
    ////        num_rank_star = 3;
    ////    }
    ////    else if (check_score >= 500)
    ////    {
    ////        num_rank_star = 2;
    ////    }
    ////    else if (check_score >= 300)
    ////    {
    ////        num_rank_star = 1;
    ////    }
    ////    else
    ////    {
    ////        num_rank_star = 0;
    ////    }
    ////}
    /////*--終わり：SetNumRankStar--*/

    /////*--------------------------------------------------*/
    /////*--関数名：SetRankStar(private)--------------------*/
    /////*--概要：評価する星の数から評価する星の設定をする--*/
    /////*--引数：星の数(int)-------------------------------*/
    /////*--戻り値：なし------------------------------------*/
    /////*--------------------------------------------------*/
    ////private void SetRankStar(int num)
    ////{
    ////    for (int i = 0; i < go_rank_star.Length; i++)
    ////    {
    ////        go_rank_star[i] = GameObject.Find("Star" + i.ToString());
    ////        //回転済みにする
    ////        rotate_star[i] = true;

    ////        //評価する星の数より小さかったら回転していない事にする
    ////        if (i < num)
    ////        {
    ////            rotate_star[i] = false;
    ////        }
    ////    }
    ////}
    /////*--終わり：SetRankStar--*/

    /////*-------------------------------------*/
    /////*--関数名：ChangeStarSprite(private)--*/
    /////*--概要：評価の星の画像を変更する-----*/
    /////*--引数：処理している星の番号(int)----*/
    /////*--戻り値：なし-----------------------*/
    /////*-------------------------------------*/
    ////private void ChangeStarSprite(int star_number)
    ////{
    ////    //画像が変わる角度を超えていて　画像を変えていなかったら
    ////    if ((go_rank_star[star_number].transform.localEulerAngles.y >= change_sprite_angle) &&
    ////        (go_rank_star[star_number].GetComponent<SpriteRenderer>().sprite.name == "TransparentStar"))
    ////    {
    ////        //画像を変更する
    ////        go_rank_star[star_number].GetComponent<SpriteRenderer>().sprite = color_star;
    ////        //画像の色を変更する
    ////        go_rank_star[star_number].GetComponent<SpriteRenderer>().color = Color.red;
    ////    }
    ////}
    /////*--終わり：ChangeStarSprite--*/

    /////*-----------------------------------*/
    /////*--関数名：StopRotateStar(private)--*/
    /////*--概要：評価の星の回転を止める-----*/
    /////*--引数：処理している星の番号(int)--*/
    /////*--戻り値：なし---------------------*/
    /////*-----------------------------------*/
    ////private void StopRotateStar(int star_number)
    ////{
    ////    //(回転を止める角度-10)を超えていて　回転を止めていなかったら
    ////    if ((go_rank_star[star_number].transform.localEulerAngles.y > (stop_rotate_angle - 10.0f)) &&
    ////        (rotate_star[star_number] == false))
    ////    {
    ////        //微調整する
    ////        go_rank_star[star_number].transform.rotation = Quaternion.Euler(new Vector3(0.0f, stop_rotate_angle, 0.0f));
    ////        //回転を止めるようにする
    ////        rotate_star[star_number] = true;
    ////    }
    ////}
    /////*--終わり：ChangeStarSprite--*/

}
