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

    /*-----------*/
    /*--private--*/
    /*-----------*/
    //リザルトフレーム
    private GameObject go_result_frame;
    //評価の星
    private GameObject[] go_rank_star = new GameObject[3];
    //…ステージクリア！
    private Text text_stage_clear;
    //スコア(テキスト)
    private Text text_score;
    //画面フェード
    private FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    //カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    //星を回転させたかどうか
    private bool[] rotate_star = new bool[3];
    //スコア
    private int score;
    //評価の星の数
    private int num_rank_star;

    /*-----------------*/
    /*--関数名：Start--*/
    /*--概要：初期化---*/
    /*--引数：なし-----*/
    /*--戻り値：なし---*/
    /*-----------------*/
    void Start()
    {
        //スコア(仮)
        score = 700;

        //オブジェクトを探す
        go_result_frame = GameObject.Find("ResultFrame");
        //コンポーネントを探す
        text_stage_clear = GameObject.Find("StageClear").GetComponent<Text>();
        text_score = GameObject.Find("Score").GetComponent<Text>();
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //座標変更
        go_result_frame.transform.position = new Vector3(-30.0f, 2.0f, 0.0f);
        text_stage_clear.rectTransform.anchoredPosition = new Vector3(-Screen.width - 300.0f, 200.0f, 0.0f);
        //○○ステージクリア！　のテキストを設定する
        text_stage_clear.text = SharedData.instance.SetStageNameEnglish(SharedData.instance.play_stage_number) + " クリア!";
        //スコア　のテキストを設定する
        text_score.text = "Score:" + score.ToString();
        //評価の星の数を決める
        //SetNumRankStar(score);
        //評価する星の数から評価する星の設定をする
        //SetRankStar(num_rank_star);
        SetRankStar(Data.star_num);


        //フェードインさせる
        //FadeManager.FadeIn(1.5f);

        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
        //Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(GameObject.Find("Main Camera").GetComponent<Camera>());
        //CanvasScalerの設定を変える(画面サイズが変わっても自動的に大きさなどを変更するように)
        SharedData.instance.SetCanvasScaleOption(GameObject.Find("Canvas").GetComponent<CanvasScaler>());

        //Canvasの設定を変える(選択フレーム)
        SharedData.instance.SetCanvasOption(GameObject.Find("SelectFrame").GetComponent<Canvas>());
        //CanvasScalerの設定を変える
        SharedData.instance.SetCanvasScaleOption(GameObject.Find("SelectFrame").GetComponent<CanvasScaler>());
        //オブジェクト「Canvas」より前に設定する
        GameObject.Find("SelectFrame").GetComponent<Canvas>().sortingOrder = 10;

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
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

        //フェードアウトしていたら
        if ((sc_screen_fade.GetFadeType() == true) && sc_screen_fade.GetFadeValue() > 0.0f)
        {
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 3.0f);
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(camera_range[0].x, camera_range[1].x), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), 10);
        }

        //リザルトフレーム(ステージクリアの後ろにある旗みたいなもの)
        if (go_result_frame.transform.position.x != 0.0f)
        {
            go_result_frame.transform.position += new Vector3(frame_speed, 0.0f, 0.0f);
            if (go_result_frame.transform.position.x > 0.0f)
            {
                go_result_frame.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            }
        }

        //ステージクリア(テキストUI)
        if (text_stage_clear.rectTransform.anchoredPosition.x != 0.0f)
        {
            text_stage_clear.rectTransform.anchoredPosition += new Vector2((frame_speed * Screen.width) / 30.0f, 0.0f);
            if (text_stage_clear.rectTransform.anchoredPosition.x > 0.0f)
            {
                text_stage_clear.rectTransform.anchoredPosition = new Vector3(0.0f, 200.0f);
            }
        }

        //評価する星
        for (int i = 0; i < go_rank_star.Length; i++)
        {
            //回転できるかどうか
            bool rotate_check = true;

            //一番目じゃなかったら
            if (i != 0)
            {
                //一つ前の星が回転終わっていなかったら
                if (rotate_star[i - 1] == false)
                {
                    rotate_check = false;
                }
            }

            //回転できる状態で　回転済みじゃなかったら
            if (rotate_check == true && rotate_star[i] == false)
            {
                //回転(角度スピードを足していく)
                go_rank_star[i].transform.Rotate(new Vector3(0.0f, angle_speed, 0.0f));
                //条件があっていたら画像を変える
                ChangeStarSprite(i);
                //条件に当てはまっていたら回転を止める
                StopRotateStar(i);
            }
        }

        //Spaceキーを押したら
        if (Input.GetKeyDown(KeyCode.Space) ||
            //Aボタンを押したら
            (Input.GetAxis(Common.GamePad.BUTTON_A) > 0))
        {
            //フェードを始める
            sc_screen_fade.SetFadeType(true);
            //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
            SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());
        }

        //フェードアウトが終わったら
        if (sc_screen_fade.GetFadeValue() == 1.0f)
        {
            //SharedDataにあるリストに飾りを入れる
            SharedData.instance.SetDecorationList(GameObject.Find("Main Camera").transform.position);
            //ステージ選択画面に移る
            SceneManager.LoadScene("StageSelectScene");
        }
    }
    /*--終わり：Update--*/

    /*-----------------------------------*/
    /*--関数名：SetNumRankStar(private)--*/
    /*--概要：評価の星の数を求める-------*/
    /*--引数：スコア(int)----------------*/
    /*--戻り値：なし---------------------*/
    /*-----------------------------------*/
    private void SetNumRankStar(int check_score)
    {
        //スコアから評価の星の数を求める(仮)
        if (check_score >= 1000)
        {
            num_rank_star = 3;
        }
        else if (check_score >= 500)
        {
            num_rank_star = 2;
        }
        else if (check_score >= 300)
        {
            num_rank_star = 1;
        }
        else
        {
            num_rank_star = 0;
        }
    }
    /*--終わり：SetNumRankStar--*/

    /*--------------------------------------------------*/
    /*--関数名：SetRankStar(private)--------------------*/
    /*--概要：評価する星の数から評価する星の設定をする--*/
    /*--引数：星の数(int)-------------------------------*/
    /*--戻り値：なし------------------------------------*/
    /*--------------------------------------------------*/
    private void SetRankStar(int num)
    {
        for (int i = 0; i < go_rank_star.Length; i++)
        {
            go_rank_star[i] = GameObject.Find("Star" + i.ToString());
            //回転済みにする
            rotate_star[i] = true;

            //評価する星の数より小さかったら回転していない事にする
            if (i < num)
            {
                rotate_star[i] = false;
            }
        }
    }
    /*--終わり：SetRankStar--*/

    /*-------------------------------------*/
    /*--関数名：ChangeStarSprite(private)--*/
    /*--概要：評価の星の画像を変更する-----*/
    /*--引数：処理している星の番号(int)----*/
    /*--戻り値：なし-----------------------*/
    /*-------------------------------------*/
    private void ChangeStarSprite(int star_number)
    {
        //画像が変わる角度を超えていて　画像を変えていなかったら
        if ((go_rank_star[star_number].transform.localEulerAngles.y >= change_sprite_angle) &&
            (go_rank_star[star_number].GetComponent<SpriteRenderer>().sprite.name == "TransparentStar"))
        {
            //画像を変更する
            go_rank_star[star_number].GetComponent<SpriteRenderer>().sprite = color_star;
            //画像の色を変更する
            go_rank_star[star_number].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    /*--終わり：ChangeStarSprite--*/

    /*-----------------------------------*/
    /*--関数名：StopRotateStar(private)--*/
    /*--概要：評価の星の回転を止める-----*/
    /*--引数：処理している星の番号(int)--*/
    /*--戻り値：なし---------------------*/
    /*-----------------------------------*/
    private void StopRotateStar(int star_number)
    {
        //(回転を止める角度-10)を超えていて　回転を止めていなかったら
        if ((go_rank_star[star_number].transform.localEulerAngles.y > (stop_rotate_angle - 10.0f)) &&
            (rotate_star[star_number] == false))
        {
            //微調整する
            go_rank_star[star_number].transform.rotation = Quaternion.Euler(new Vector3(0.0f, stop_rotate_angle, 0.0f));
            //回転を止めるようにする
            rotate_star[star_number] = true;
        }
    }
    /*--終わり：ChangeStarSprite--*/

}
