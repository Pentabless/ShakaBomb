using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearDirector : MonoBehaviour
{
    //BGM
    public AudioClip game_clear_bgm;
    //ステージ選択シーンへ
    private GameObject go_select_stage;
    //ステージ選択シーンへ初期位置
    Vector3 select_stage_start_pos;
    //円運動するための角度
    float select_stage_angle;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_select_stage = GameObject.Find("SelectStageButton");
        //初期位置を覚える
        select_stage_start_pos = new Vector3(-200.0f, 100.0f, -2100.0f);

        //BGMを流す
        SoundPlayer.PlayBGM(game_clear_bgm, 0.5f);
        //フェードインさせる
        FadeManager.fadeColor = Color.black;
        FadeManager.FadeIn(1.5f);

    }

    // Update is called once per frame
    void Update()
    {
        go_select_stage.GetComponent<RectTransform>().anchoredPosition = new Vector3(select_stage_start_pos.x + (Mathf.Sin(select_stage_angle - 90.0f) * 30.0f), select_stage_start_pos.y, select_stage_start_pos.z);

        select_stage_angle += 0.1f;

        //何か入力をしたら
        if (Input.anyKeyDown && !FadeManager.isFadeOut)
        {
            //ステージ選択シーンへ
            FadeManager.FadeOut("NewStageSelectScene", 2.5f);
            //BGMをフェードアウトする
            SoundFadeController.SetFadeOutSpeed(-0.005f);
        }
    }
}
