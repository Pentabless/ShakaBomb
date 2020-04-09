using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI

public class ResultDirector : MonoBehaviour
{
    //スピード
    public float frame_speed;
    public float angle_speed;
    //色のついた星のスプライト
    public Sprite color_star;
    //クリアしたステージ番号(仮)
    public int clear_stage_number;
    //リザルトフレーム
    GameObject go_result_frame;
    //評価の星
    GameObject[] go_rank_star = new GameObject[3];
    //画面フェード
    Image image_screen_fade;
    //…ステージクリア！
    Text text_stage_clear;
    //スコア(テキスト)
    Text text_score;
    //スコア
    int score;
    //評価の星の数
    int num_rank_star;
    //星を回転させたかどうか
    bool[] rotate_star = new bool[3];

    // Start is called before the first frame update
    void Start()
    {
        //スコア(仮)
        score = 700;

        //オブジェクトを探す
        go_result_frame = GameObject.Find("ResultFrame");
        text_stage_clear = GameObject.Find("StageClear").GetComponent<Text>();
        text_score = GameObject.Find("Score").GetComponent<Text>();
        image_screen_fade = GameObject.Find("ScreenFade").GetComponent<Image>();
        //座標変更
        go_result_frame.transform.position = new Vector3(-30.0f, 2.0f, 0.0f);
        text_stage_clear.rectTransform.anchoredPosition = new Vector3(-Screen.width - 300.0f, 200.0f, 0.0f);
        //○○ステージクリア！　のテキストを設定する
        SetClearStageNameEnglish();
        //スコア　のテキストを設定する
        text_score.text = "Score:" + score.ToString();
        //評価の星の数を決める
        SetNumRankStar(score);
        //評価する星の数から評価する星の設定をする
        SetRankStar(num_rank_star);
    }

    // Update is called once per frame
    void Update()
    {
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

                //角度90°超えていて　画像を変えていなかったら
                if ((go_rank_star[i].transform.localEulerAngles.y >= 90.0f) &&
                    (go_rank_star[i].GetComponent<SpriteRenderer>().sprite.name == "TransparentStar"))
                {
                    //画像を変更する
                    go_rank_star[i].GetComponent<SpriteRenderer>().sprite = color_star;
                    //画像の色を変更する
                    go_rank_star[i].GetComponent<SpriteRenderer>().color = Color.red;
                }

                //角度が170°超えていて　回転を止めていなかったら
                if ((go_rank_star[i].transform.localEulerAngles.y > 170.0f) &&
                    (rotate_star[i] == false))
                {
                    //微調整する
                    go_rank_star[i].transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                    //回転を止めるようにする
                    rotate_star[i] = true;
                }
            }
        }

        //ステージ選択画面をロードする
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //フェードを始める
            image_screen_fade.GetComponent<FadeController>().SetFadeType(true);
        }

        //フェードアウトが終わったら
        if (image_screen_fade.color.a >= 1.0f)
        {
            //ステージ選択画面に移る
            SceneManager.LoadScene("StageSelectScene");
        }
    }

    //英語の何番目の表記設定 <自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void SetClearStageNameEnglish()
    {
        string number = "";

        switch (clear_stage_number % 10)
        {
            //「st」を付ける
            case 1:
                if (clear_stage_number != 11)
                {
                    number = clear_stage_number.ToString() + "st";
                }
                break;
            //「nd」を付ける
            case 2:
                if (clear_stage_number != 12)
                {
                    number = clear_stage_number.ToString() + "nd";
                }
                break;
            //「rd」を付ける
            case 3:
                if (clear_stage_number != 13)
                {
                    number = clear_stage_number.ToString() + "rd";
                }
                break;
        }
        //まだ何も設定されていなかったら
        if (number == "")
        {
            //「th」を付ける
            number = clear_stage_number.ToString() + "th";
        }
        text_stage_clear.text = number + " Stage クリア!";
    }

    //評価の星の数を決める <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void SetNumRankStar(int check_score)
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

    //評価する星の数から評価する星の設定をする -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void SetRankStar(int num)
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

}
