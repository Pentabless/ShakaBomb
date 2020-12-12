/*------------------------------------------------------------*/
/*--ファイル名：StageSelectDirector.cs------------------------*/
/*--概要：ステージ選択シーンの処理(ステージ選択やシーン遷移)--*/
/*------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Common;

public class StageSelectDirector : MonoBehaviour
{
    /*----------*/
    /*--public--*/
    /*----------*/
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

    /*-----------*/
    /*--private--*/
    /*-----------*/
    //カメラオブジェクト
    private GameObject go_camera;
    //選択フレーム
    private GameObject go_select_tex;
    private GameObject go_select_frame;
    //タイトルボタン
    private GameObject go_title_button;
    //ステージの画像
    private GameObject[] go_stage;
    //背景
    private GameObject go_background;
    //選択フレームのコンポーネント
    private RectTransform component_select_frame;
    //画面フェード
    private FadeController sc_screen_fade;
    //背景の飾りジェネレーター
    private BackGroundDecorationGenerator sc_decoration_generator;
    //カメラの映す範囲([0]左下　[1]右上)
    private Vector3[] camera_range;
    //距離
    private Vector3 stage_distance;
    //覚える座標
    private Vector3 last_position;
    //移ろうとしているシーンの名前
    private string next_scene_name;
    //選んでいるステージ番号
    private int stage_number;
    //選んでいたステージ番号
    private int last_number;
    //次のステージに選択するためのカウント
    private int select_next_stage_count;
    //円運動用角度
    private float angle;
    //何も操作しなくなった時の時間
    private float not_operate_time;
    //カメラが映す範囲の幅
    private float camera_width;
    //タイトルボタンを選んでいるか
    private bool select_title;
    //ステージを選択できる状態か
    private bool select_stage;
    //フェードアウトが始まっているか
    private bool start_fade_out;
    //開発している時の画面サイズ
    private Vector2 making_screen_size;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        //オブジェクトを探す
        go_camera = GameObject.Find("Main Camera");
        go_select_tex = GameObject.Find("SelectTex");
        go_select_frame = GameObject.Find("SelectFrame");
        go_title_button = GameObject.Find("TitleButton");
        go_background = GameObject.Find("ProvisionalBackGround");
        GameObject go_screen_fade = GameObject.Find("ScreenFade");
        //コンポーネントを探す
        component_select_frame = go_select_frame.transform.Find("Frame").GetComponent<RectTransform>();
        sc_screen_fade = GameObject.Find("ScreenFade").GetComponent<FadeController>();
        sc_decoration_generator = GameObject.Find("BackGroundDecorationGenerator").GetComponent<BackGroundDecorationGenerator>();

        //ステージ
        FindStageObject();

        //初期化
        stage_distance = Vector2.zero;
        last_position = Vector2.zero;
        next_scene_name = "";
        stage_number = SharedData.instance.playStageNumber;
        last_number = 0;
        select_next_stage_count = 0;
        angle = 0.0f;
        not_operate_time = -1.0f;
        camera_width = 0.0f;
        select_title = false;
        select_stage = true;
        start_fade_out = false;
        making_screen_size = new Vector2(1920.0f, 1050.0f);

        //フェードの初期化
        Vector4 fade_color = go_screen_fade.GetComponent<Image>().color;
        go_screen_fade.GetComponent<Image>().color = new Vector4(fade_color.x, fade_color.y, fade_color.z, 1.0f);

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
        //Cameraの映る範囲をもらう
        camera_range = SharedData.instance.GetCameraRange(go_camera.GetComponent<Camera>());
        //飾りを作成する(背景と前景)
        SharedData.instance.CreatePreviousSceneDecoration(sc_decoration_generator, new Vector3(go_select_tex.transform.position.x, 0.0f));
        //CanvasScalerの設定を変える(画面サイズが変わっても自動的に大きさなどを変更するように)
        SharedData.instance.SetCanvasScaleOption(GameObject.Find("Canvas").GetComponent<CanvasScaler>());

        //Canvasの設定を変える(選択フレーム)
        SharedData.instance.SetCanvasOption(go_select_frame.GetComponent<Canvas>());
        //CanvasScalerの設定を変える
        SharedData.instance.SetCanvasScaleOption(go_select_frame.GetComponent<CanvasScaler>());
        //オブジェクト「Canvas」より前に設定する
        GameObject.Find("SelectFrame").GetComponent<Canvas>().sortingOrder = 10;

        //座標変更
        go_select_tex.transform.position = go_stage[SharedData.instance.playStageNumber].transform.position;
        component_select_frame.position = new Vector3(go_select_tex.transform.position.x, go_select_tex.transform.position.y, component_select_frame.position.z);
        //拡大率変更
        go_select_tex.transform.position = (go_stage[SharedData.instance.playStageNumber].transform.localScale.x * go_stage[SharedData.instance.playStageNumber].transform.Find("StageFrame").transform.localScale) + new Vector3(0.5f, 0.5f, 0.0f); ;
        component_select_frame.localScale = go_select_tex.transform.localScale;
        Debug.Log(go_camera.transform.position);
        //カメラの移る幅を渡す
        camera_width = camera_range[1].x - camera_range[0].x;
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        //背景の飾りを作成する
        float decoration_scale = Random.Range(0.3f, 3.0f);
        sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(go_stage[0].transform.position.x - (camera_width / 2), go_stage[go_stage.Length - 1].transform.position.x + (camera_width / 2)), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), -10);
        //フェードアウトを始めていなかったら
        if (start_fade_out == false)
        {
            //選択している画像が動いていなかったら
            if (angle == 0.0f)
            {
                //タイトルを選んでいなかったら
                if (select_title == false)
                {
                    //座標を微調整する
                    go_select_tex.transform.position = go_stage[stage_number].transform.position;
                    component_select_frame.position = new Vector3(go_select_tex.transform.position.x, go_select_tex.transform.position.y, component_select_frame.position.z);
                    //拡大率を変える
                    go_select_tex.transform.localScale = (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale) + new Vector3(0.5f, 0.5f, 0.0f);
                    component_select_frame.localScale = (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale) + new Vector3(0.5f, 0.5f, 0.0f);
                    //画面の大きさによって拡大率を変える
                    //component_select_frame.localScale = new Vector3(component_select_frame.localScale.x * (making_screen_size.x / Screen.width), component_select_frame.localScale.y * (making_screen_size.y / Screen.height), 1.0f);


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
                        (Input.GetAxis(ConstGamePad.VERTICAL) < 0))
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
                    go_select_tex.transform.position = go_title_button.transform.position;
                    component_select_frame.position = new Vector3(go_select_tex.transform.position.x, go_select_tex.transform.position.y, component_select_frame.position.z);
                    //上矢印キーを押したら
                    if ((Input.GetKeyDown(KeyCode.UpArrow)) ||
                        //十字上ボタンを押したら
                        (Input.GetAxis("cross Y") < -0.5) ||
                        //左スティックを上に傾けたら
                        (Input.GetAxis(ConstGamePad.VERTICAL) > 0))
                    {
                        //タイトルを選択していない状態にする
                        select_title = false;
                        PreparaChangeTitle(select_title);
                    }
                }

                //Spaceキーを押したら
                if ((Input.GetKeyDown(KeyCode.Space)) ||
                    //Aボタンを押したら
                    (Input.GetAxis(ConstGamePad.BUTTON_A) > 0))
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
                        SharedData.instance.playStageNumber = stage_number;
                    }
                    else
                    {
                        //移ろうとしているシーンの名前を設定する
                        next_scene_name = "TitleScene";
                    }
                }

                //Bボタンを押したら
                if ((Input.GetAxis(ConstGamePad.BUTTON_B) > 0))
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
                //選択フレームの更新(移動と拡大率変更)
                UpdateSelectFrame(select_title);

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
                        component_select_frame.position = new Vector3(go_select_tex.transform.position.x, go_select_tex.transform.position.y, component_select_frame.position.z);
                    }
                    else
                    {
                        go_select_tex.transform.position = go_title_button.transform.position;
                        component_select_frame.position = new Vector3(go_title_button.transform.position.x, go_title_button.transform.position.y, component_select_frame.position.z);
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
            sc_decoration_generator.CreateDecoration(new Vector3(Random.Range(go_camera.transform.position.x - (camera_width / 2), go_camera.transform.position.x + (camera_width / 2)), camera_range[0].y - decoration_scale, 0.0f), new Vector3(decoration_scale, decoration_scale, decoration_scale), new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f), 10);

            //フェードアウトが終わったら
            if (sc_screen_fade.GetFadeValue() == 1.0f)
            {
                //SharedDataにあるリストに飾りを入れる
                SharedData.instance.SetDecorationList(go_camera.transform.position);
                //記憶していた移ろうとしているシーンに移る
                SceneManager.LoadScene(next_scene_name);
            }
        }

        //タイトルを選択していない状態で　ステージを選択できる状態
        if (select_title == false && select_stage == true)
        {
            go_camera.transform.position = new Vector3(go_select_tex.transform.position.x, 0.0f, -10.0f);
            //go_title_button.transform.position = new Vector3(go_camera.transform.position.x + 6.5f, go_camera.transform.position.y + (-4.0f), 0.0f);
        }

        //背景の座標をカメラの座標
        go_background.transform.position = new Vector3(go_camera.transform.position.x, go_camera.transform.position.y, 0.0f);
    }



    //------------------------------------------------------------------------------------------
    // summary : LeftRightInput
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void LeftRightInput()
    {
        //入力判断するための変数
        int num = 0;

        //左矢印キーを押したら
        if ((Input.GetKey(KeyCode.LeftArrow)) ||
            //十字左ボタンを押したら
            (Input.GetAxis("cross X") < 0) ||
            //左スティックを左に傾けたら
            (Input.GetAxis(ConstGamePad.HORIZONTAL) < 0))
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
            (Input.GetAxis(ConstGamePad.HORIZONTAL) > 0))
        {
            //ステージ番号がクリアしているステージ番号+1より小さかったら
            if (stage_number < clear_stage_number + 1/*stage_number < go_stage.Length - 1*/)
            {
                num = 1;
            }
        }

        //変数が変わっていたら
        if (num != 0)
        {
            //カウントを設定する
            select_next_stage_count = select_next_stage_frame;
            //ステージ番号を変更する準備をする
            PreparaChangeSelectStage(num);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : PreparaChangeSelectStage
    // remarks : none
    // param   : numberDifference
    // return  : none
    //------------------------------------------------------------------------------------------
    private void PreparaChangeSelectStage(int numberDifference)
    {
        //選択していたステージの座標を覚える
        last_position = go_stage[stage_number].transform.position;
        //選択していたステージ番号を覚える
        last_number = stage_number;
        //ステージ番号を変更する
        stage_number += numberDifference;
        //円運動を始める
        angle += speed;
        //選択していたステージと選択するステージの距離を求める
        stage_distance = go_stage[stage_number].transform.position - go_stage[last_number].transform.position;
        //ステージフレームに選ばれている番号を教える
        SetSelectStage(false);
    }



    //------------------------------------------------------------------------------------------
    // summary : PreparaChangeTitle
    // remarks : none
    // param   : select
    // return  : none
    //------------------------------------------------------------------------------------------
    private void PreparaChangeTitle(bool select)
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



    //------------------------------------------------------------------------------------------
    // summary : SetSelectStage
    // remarks : none
    // param   : selectTitle
    // return  : none
    //------------------------------------------------------------------------------------------
    private void SetSelectStage(bool selectTitle)
    {
        //ステージフレームに選ばれている番号を教える
        for (int i = 0; i < go_stage.Length; i++)
        {
            bool select = false;
            //タイトルが選ばれていなかったら
            if (selectTitle == false)
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



    //------------------------------------------------------------------------------------------
    // summary : CountNotOperateTime
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void CountNotOperateTime()
    {
        //操作していなかったら
        if (!(Input.anyKey))
        {
            //操作していない時間を覚えていたら
            if (not_operate_time != -1.0f)
            {
                //***//Debug.Log(Time.time - not_operate_time);
                //操作していない時間がタイトル画面に戻るための無操作時間以上経ったら
                if (Time.time - not_operate_time >= (to_title_not_operate_minute * Application.targetFrameRate))
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



    //------------------------------------------------------------------------------------------
    // summary : FindStageObject
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void FindStageObject()
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



    //------------------------------------------------------------------------------------------
    // summary : AllStageLinePass
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void AllStageLinePass()
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



    //------------------------------------------------------------------------------------------
    // summary : UpdateSelectFrame
    // remarks : none
    // param   : select
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateSelectFrame(bool select)
    {
        //選択フレームを動かす
        go_select_tex.transform.position =
            last_position +
            new Vector3(
            (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.x * 0.5f),
            (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.y * 0.5f),
            0.0f);
        component_select_frame.position = new Vector3(go_select_tex.transform.position.x, go_select_tex.transform.position.y, component_select_frame.position.z);
        //始点
        Vector3 start_button;
        //終点
        Vector3 end_button;
        //ステージが選択できない状態(ステージ→タイトル　タイトル→ステージ　の場合)
        if (select_stage == false)
        {
            //タイトルを選んでいたら
            if (select)
            {
                //始点…GameStartボタン
                start_button = (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale);
                //終点…ExitGameボタン
                end_button = go_title_button.transform.localScale;
            }
            else
            {
                //始点…ExitGameボタン
                start_button = go_title_button.transform.localScale;
                //終点…GameStartボタン
                end_button = (go_stage[stage_number].transform.localScale.x * go_stage[stage_number].transform.Find("StageFrame").transform.localScale);
            }

            //大きさ変更
            go_select_tex.transform.localScale = Vector3.Lerp(
                start_button,
                end_button,
                ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
            //ボタンより少し大きくする
            go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);

            component_select_frame.transform.localScale = go_select_tex.transform.localScale;
        }
    }
}
