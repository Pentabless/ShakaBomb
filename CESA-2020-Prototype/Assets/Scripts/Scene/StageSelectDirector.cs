using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移
using UnityEngine.UI;   //UI

public class StageSelectDirector : MonoBehaviour
{
    //選択するスピード
    public float speed;
    //選択するステージの名前(※build settingにシーンを登録しておく事)
    public string[] stage_names;
    //タイトル画面に戻るための無操作時間(単位：分)
    public float to_title_not_operate_minute;
    //左右キーを長押しした時に次のステージに選択するためのフレーム数
    public int select_next_stage_frame;
    //仮　クリアしたステージ番号
    public int clear_stage_number;
    //カメラオブジェクト
    GameObject go_camera;
    //選択フレーム
    GameObject go_select_tex;
    //タイトルボタン
    GameObject go_title_button;
    //ステージの画像
    GameObject[] go_stage;
    //背景
    GameObject go_background;
    //画面フェード
    FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    BackGroundDecorationGenerator sc_decoration_generator;
    //距離
    Vector3 stage_distance;
    //覚える座標
    Vector3 last_position;
    //移ろうとしているシーンの名前
    string next_scene_name;
    //選んでいるステージ番号
    int stage_number;
    //選んでいたステージ番号
    int last_number;
    //次のステージに選択するためのカウント
    int select_next_stage_count;
    //円運動用角度
    float angle;
    //何も操作しなくなった時の時間
    float not_operate_time;
    //タイトルボタンを選んでいるか
    bool select_title;
    //ステージを選択できる状態か
    bool select_stage;
    //フェードアウトが始まっているか
    bool start_fade_out;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_camera = GameObject.Find("Main Camera");
        go_select_tex = GameObject.Find("SelectTex");
        go_title_button = GameObject.Find("TitleButton");
        go_background = GameObject.Find("ProvisionalBackGround");
        //コンポーネントを探す
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //ステージ
        FindStageObject();

        //座標変更
        go_select_tex.transform.position = go_stage[0].transform.position;
        //拡大率変更
        go_select_tex.transform.localScale = (go_stage[stage_number].transform.localScale.x * go_stage[0].transform.Find("StageFrame").transform.localScale) + new Vector3(0.5f, 0.5f, 0.0f);
        //初期化
        stage_distance = Vector2.zero;
        last_position = Vector2.zero;
        next_scene_name = "";
        stage_number = 0;
        last_number = 0;
        select_next_stage_count = 0;
        angle = 0.0f;
        not_operate_time = -1.0f;
        select_title = false;
        select_stage = true;
        start_fade_out = false;

        for (int i = 0; i < go_stage.Length; i++)
        {
            //クリアしているステージ番号に1足したステージ番号までを遊べる状態にする
            if (i <= clear_stage_number + 1)
            {
                go_stage[i].GetComponent<StageFrameController>().SetCanPlay(true);
            }
            else
            {
                go_stage[i].GetComponent<StageFrameController>().SetCanPlay(false);
            }
        }

        //ステージフレームに選ばれている番号を教える
        SetSelectStage(false);
        //ステージ全てをLineで通す
        AllStageLinePass();

        //Canvasの設定を変える(泡の飾りをUIより前に表示するために)
        SharedData.instance.SetCanvasOption(GameObject.Find("Canvas").GetComponent<Canvas>());

        //飾りを作成する(背景と前景)
        SharedData.instance.CreatePreviousSceneDecoration(sc_decoration_generator);
    }

    // Update is called once per frame
    void Update()
    {
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);

        //フェードアウトを始めていなかったら
        if (start_fade_out == false)
        {
            //選択している画像が動いていなかったら
            if (angle == 0.0f)
            {
                //タイトルを選んでいなかったら
                if (select_title == false)
                {
                    //拡大率を変える
                    go_select_tex.transform.localScale = (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale) + new Vector3(0.5f, 0.5f, 0.0f);

                    //次のステージに選択するためのカウントが0以下になっていたら
                    if (select_next_stage_count <= 0)
                    {
                        //左右入力の処理
                        LeftRightInput();
                    }
                    //下矢印キーを押したら
                    if ((Input.GetKey(KeyCode.DownArrow)) ||
                        //十字下ボタンを押したら
                        (Input.GetAxis("cross Y") > 0.5) ||
                        //左スティックを下に傾けたら
                        (Input.GetAxis(Common.GamePad.VERTICAL) < 0))
                    {
                        //カウントを設定されていなかったら(ステージ選択ボタンを押していなかったら)
                        if (select_next_stage_count != select_next_stage_frame)
                        {
                            //タイトルを選択している状態にする
                            select_title = true;
                            PreparaChangeTitle(select_title);
                            //ステージを選べない状態にする
                            select_stage = false;
                        }
                    }
                }
                else
                {
                    //上矢印キーを押したら
                    if ((Input.GetKeyDown(KeyCode.UpArrow)) ||
                        //十字上ボタンを押したら
                        (Input.GetAxis("cross Y") < -0.5) ||
                        //左スティックを上に傾けたら
                        (Input.GetAxis(Common.GamePad.VERTICAL) > 0))
                    {
                        //タイトルを選択していない状態にする
                        select_title = false;
                        PreparaChangeTitle(select_title);
                    }
                }

                //Spaceキーを押したら
                if ((Input.GetKeyDown(KeyCode.Space)) ||
                    //Aボタンを押したら
                    (Input.GetAxis(Common.GamePad.BUTTON_A) > 0))
                {
                    //フェードアウトを始める
                    sc_screen_fade.SetFadeType(true);
                    sc_screen_fade.SetFadeValue(0.0f);
                    //フェードアウトが始まった事にする
                    start_fade_out = true;

                    //タイトルを選択していなかったら
                    if (select_title == false)
                    {
                        //移ろうとしているシーンの名前を設定する
                        next_scene_name = stage_names[stage_number];
                        //プレイするステージ番号を覚える
                        SharedData.instance.play_stage_number = stage_number;
                    }
                    else
                    {
                        //移ろうとしているシーンの名前を設定する
                        next_scene_name = "TitleScene";
                    }
                }

                //Bボタンを押したら
                if ((Input.GetAxis(Common.GamePad.BUTTON_B) > 0))
                {
                    //フェードアウトを始める
                    sc_screen_fade.SetFadeType(true);
                    sc_screen_fade.SetFadeValue(0.0f);
                    //フェードアウトが始まった事にする
                    start_fade_out = true;
                    //移ろうとしているシーンの名前を設定する
                    next_scene_name = "TitleScene";
                }

            }
            //動いている途中だったら
            else
            {
                //選択フレームを動かす
                go_select_tex.transform.position =
                    last_position +
                    new Vector3(
                    (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.x * 0.5f),
                    (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.y * 0.5f),
                    0.0f);

                //ステージが選択できない状態(ステージ→タイトル　タイトル→ステージ　の場合)
                if (select_stage == false)
                {
                    //タイトルを選択している時
                    if (select_title)
                    {
                        go_select_tex.transform.localScale = Vector3.Lerp(
                        (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale),
                        go_title_button.transform.localScale,
                        ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
                    }
                    else
                    {
                        go_select_tex.transform.localScale = Vector3.Lerp(
                        go_title_button.transform.localScale,
                        (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale),
                        ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
                    }
                    go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);
                }

                //半周していなかったら
                if (angle < 180.0f)
                {
                    angle += speed;
                }
                //半周していたら
                else
                {
                    //タイトルを選択していなかったら
                    if (select_title == false)
                    {
                        //ステージを選択できない状態だったら
                        if (select_stage == false)
                        {
                            //選択できるようにする
                            select_stage = true;
                        }
                        //微調整する
                        go_select_tex.transform.position = go_stage[stage_number].transform.position;
                    }
                    else
                    {
                        go_select_tex.transform.position = go_title_button.transform.position;
                    }
                    //円運動が終わった事にする
                    angle = 0.0f;
                }
            }

            //次のステージに選択するためのカウントが0より上だったら
            if (select_next_stage_count > 0)
            {
                select_next_stage_count--;
            }

            //操作していない時間を計る
            CountNotOperateTime();
        }
        //フェードアウトを始めていたら
        else
        {
            //前景の飾りを作成する
            decoration_scale = Random.Range(0.3f, 3.0f);
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(-15.0f, 15.0f), -7.5f, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f), 10);

            //フェードアウトが終わったら
            if (sc_screen_fade.GetFadeValue() == 1.0f)
            {
                //SharedDataにあるリストに飾りを入れる
                SharedData.instance.SetDecorationList();
                //記憶していた移ろうとしているシーンに移る
                SceneManager.LoadScene(next_scene_name);
            }
        }

        //タイトルを選択していない状態で　ステージを選択できる状態
        if (select_title == false && select_stage == true)
        {
            go_camera.transform.position = new Vector3(go_select_tex.transform.position.x, 0.0f, -10.0f);
            go_title_button.transform.position = new Vector3(go_camera.transform.position.x + 6.5f, go_camera.transform.position.y + (-4.0f), 0.0f);
        }

        //背景の座標をカメラの座標
        go_background.transform.position = new Vector3(go_camera.transform.position.x, go_camera.transform.position.y, 0.0f);
    }

    //左右入力の処理 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void LeftRightInput()
    {
        //入力判断するための変数
        int num = 0;

        //左矢印キーを押したら
        if ((Input.GetKey(KeyCode.LeftArrow)) ||
            //十字左ボタンを押したら
            (Input.GetAxis("cross X") < 0) ||
            //左スティックを左に傾けたら
            (Input.GetAxis(Common.GamePad.HORIZONTAL) < 0))
        {
            //ステージ番号が0より大きかったら
            if (stage_number > 0)
            {
                num = -1;
            }
        }
        //右矢印キーを押したら
        else if (Input.GetKey(KeyCode.RightArrow) ||
            //十字右ボタンを押したら
            (Input.GetAxis("cross X") > 0) ||
            //左スティックを右に傾けたら
            (Input.GetAxis(Common.GamePad.HORIZONTAL) > 0))
        {
            //ステージ番号がクリアしているステージ番号+1より小さかったら
            if (stage_number < clear_stage_number + 1/*stage_number < go_stage.Length - 1*/)
            {
                num = 1;
            }
        }

        //変数が変わっていたら
        if(num!=0)
        {
            //カウントを設定する
            select_next_stage_count = select_next_stage_frame;
            //ステージ番号を変更する準備をする
            PreparaChangeSelectStage(num);
        }
    }

    //選択しているステージを変更する準備 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void PreparaChangeSelectStage(int number_difference)
    {
        //選択していたステージの座標を覚える
        last_position = go_stage[stage_number].transform.position;
        //選択していたステージ番号を覚える
        last_number = stage_number;
        //ステージ番号を変更する
        stage_number += number_difference;
        //円運動を始める
        angle += speed;
        //選択していたステージと選択するステージの距離を求める
        stage_distance = go_stage[stage_number].transform.position - go_stage[last_number].transform.position;
        //ステージフレームに選ばれている番号を教える
        SetSelectStage(false);
    }

    //選択しているステージorタイトルを変更する準備 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void PreparaChangeTitle(bool select)
    {
        //円運動を始める
        angle += speed;
        //タイトルを選択していたら
        if (select)
        {
            //選択していたステージの座標を覚える
            last_position = go_stage[stage_number].transform.position;
            //タイトルと選択するステージの距離を求める
            stage_distance = go_title_button.transform.position - go_stage[stage_number].transform.position;
            //ステージフレームに選ばれている番号を教える
            SetSelectStage(true);
        }
        else
        {
            //選択していたタイトルの座標を覚える
            last_position = go_title_button.transform.position;
            //選択するステージとタイトルの距離を求める
            stage_distance = go_stage[stage_number].transform.position - go_title_button.transform.position;
            //ステージフレームに選ばれている番号を教える
            SetSelectStage(false);
        }
    }

    //選ばれているステージをステージフレーム側に知らせる <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void SetSelectStage(bool select_title)
    {
        //ステージフレームに選ばれている番号を教える
        for (int i = 0; i < go_stage.Length; i++)
        {
            bool select = false;
            //タイトルが選ばれていなかったら
            if (select_title == false)
            {
                //選ばれている番号と同じだったら
                if (i == stage_number)
                {
                    select = true;
                }
            }
            go_stage[i].GetComponent<StageFrameController>().SetNowSelect(select);
        }
    }

    //操作していない時間を計ってタイトルに戻るようにする <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void CountNotOperateTime()
    {
        //操作していなかったら
        if (!(Input.anyKey))
        {
            //操作していない時間を覚えていたら
            if (not_operate_time != -1.0f)
            {
                //***//Debug.Log(Time.time - not_operate_time);
                //操作していない時間がタイトル画面に戻るための無操作時間以上経ったら
                if (Time.time - not_operate_time >= (to_title_not_operate_minute * 60.0f))
                {
                    //フェードアウトを始める
                    sc_screen_fade.SetFadeType(true);
                    sc_screen_fade.SetFadeValue(0.0f);
                    //フェードアウトが始まった事にする
                    start_fade_out = true;
                    //移ろうとしているシーンの名前を設定する
                    next_scene_name = "TitleScene";
                }
            }
            //覚えていなかったら
            else
            {
                //操作していない時間を覚える
                not_operate_time = Time.time;
            }
        }
        //操作していたら
        else
        {
            //操作していない時間を忘れる
            not_operate_time = -1.0f;
        }
    }

    //ステージの数を数えて配列に順番に入れる <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void FindStageObject()
    {
        //ステージの数を初期化する
        int num_stage = 0;
        //オブジェクトの数を数える
        while (true)
        {
            //オブジェクトがなかったら
            if (GameObject.Find("StagePrefab (" + num_stage.ToString() + ")") == null)
            {
                //数えるのをやめる
                break;
            }
            num_stage++;
        }

        //サイズを設定する
        go_stage = new GameObject[num_stage];

        for (int i = 0; i < num_stage; i++)
        {
            //オブジェクトを探す
            go_stage[i] = GameObject.Find("StagePrefab (" + i.ToString() + ")");
        }
    }

    //ステージを全てLineで通す <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void AllStageLinePass()
    {
        //ステージの数に対応して線を引く
        if (go_stage.Length > 1)
        {
            //Lineオブジェクトを探す
            GameObject go_line = GameObject.Find("StageLine");
            //コンポーネントを設定する
            LineRenderer renderer = go_line.GetComponent<LineRenderer>();
            //Lineの座標の数を設定する
            renderer.positionCount = 1 + ((go_stage.Length - 1) * 2);
            //Lineの座標の数分処理する
            for (int i = 0; i < renderer.positionCount; i++)
            {
                //偶数だったら
                if (i % 2 == 0)
                {
                    //ステージの座標に合わせる
                    renderer.SetPosition(i, go_stage[i / 2].transform.position);
                }
                //奇数だったら
                else
                {
                    //xを前のステージの座標に　yを次のステージの座標にする
                    renderer.SetPosition(i, new Vector3(go_stage[i / 2].transform.position.x, go_stage[(i / 2) + 1].transform.position.y, 0.0f));
                }
            }
            //マテリアル設定
            renderer.materials[0].color = Color.red;
            //幅設定
            renderer.startWidth = 1.0f;
            renderer.endWidth = 1.0f;
        }
        else
        {
            Destroy(GameObject.Find("StageLine"));
        }
    }
}
