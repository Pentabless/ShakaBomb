using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移

public class TitleDirector : MonoBehaviour
{
    //選択するスピード
    public float speed;
    //ゲームを始めるボタン
    GameObject go_start_button;
    //ゲームをやめるボタン
    GameObject go_exit_button;
    //選択フレーム
    GameObject go_select_tex;
    //ゲームをやめるかどうか
    bool select_eixt;
    //円運動用角度
    float angle;
    //距離
    Vector3 button_distance;
    //覚える座標
    Vector3 last_position;

    // Start is called before the first frame update
    void Start()
    {
        go_start_button = GameObject.Find("StartButton");
        go_exit_button = GameObject.Find("ExitButton");
        go_select_tex = GameObject.Find("SelectTex");
        go_select_tex.transform.position = go_start_button.transform.position;
        go_select_tex.transform.localScale = go_start_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
        select_eixt = false;
        angle = 0.0f;
        button_distance = Vector3.zero;
        last_position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //選択している画像が動いていなかったら
        if (angle == 0.0f)
        {
            //上を押したら
            if (Input.GetKeyDown(KeyCode.UpArrow) && select_eixt == true)
            {
                select_eixt = false;
                PreparaChangeButton(select_eixt);
            }
            //下を押したら
            if (Input.GetKeyDown(KeyCode.DownArrow) && select_eixt == false)
            {
                select_eixt = true;
                PreparaChangeButton(select_eixt);
            }
            //Space(決定)を押したら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //ExitGameを選択していなかったら
                if (select_eixt == false)
                {
                    SceneManager.LoadScene("StageSelectScene");
                }
                else
                {
                    //ゲームを閉じる
                }
            }
        }
        //動いている途中だったら
        else
        {
            //選択フレームを動かす
            go_select_tex.transform.position =
                last_position +
                new Vector3(
                (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (button_distance.x * 0.5f),
                (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (button_distance.y * 0.5f),
                0.0f);

            //選択フレームの大きさ変更
            if (select_eixt)
            {
                go_select_tex.transform.localScale = Vector3.Lerp(
                    go_start_button.transform.localScale,
                    go_exit_button.transform.localScale,
                    ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
            }
            else
            {
                go_select_tex.transform.localScale = Vector3.Lerp(
                    go_exit_button.transform.localScale,
                    go_start_button.transform.localScale,
                    ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
            }
            go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);

            //半周していなかったら
            if (angle < 180.0f)
            {
                angle += speed;
            }
            //半周していたら
            else
            {
                //微調整する
                if (select_eixt)
                {
                    go_select_tex.transform.position = go_exit_button.transform.position;
                    go_select_tex.transform.localScale = go_exit_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
                }
                else
                {
                    go_select_tex.transform.position = go_start_button.transform.position;
                    go_select_tex.transform.localScale = go_start_button.transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
                }
                //円運動が終わった事にする
                angle = 0.0f;
            }
        }

    }

    void PreparaChangeButton(bool select)
    {
        //円運動を始める
        angle += speed;
        //ゲームをやめるを選択していたら
        if (select)
        {
            //選択していたボタンの座標を覚える
            last_position = go_start_button.transform.position;
            //ゲームをやめるボタンとゲームを始めるボタンの距離を求める
            button_distance = go_exit_button.transform.position - go_start_button.transform.position;
        }
        else
        {
            //選択していたボタンの座標を覚える
            last_position = go_exit_button.transform.position;
            //ゲームを始めるボタンとゲームをやめるボタンの距離を求める
            button_distance = go_start_button.transform.position - go_exit_button.transform.position;
        }
    }
}
