using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearDirector : MonoBehaviour
{
    //BGM
    public AudioClip game_clear_bgm;

    // Start is called before the first frame update
    void Start()
    {
        //BGMを流す
        SoundPlayer.PlayBGM(game_clear_bgm,0.5f);
        //フェードインさせる
        FadeManager.fadeColor = Color.black;
        FadeManager.FadeIn(1.5f);

    }

    // Update is called once per frame
    void Update()
    {
        //何か入力をしたら
        if(Input.anyKeyDown)
        {
            //ステージ選択シーンへ
            FadeManager.FadeOut("NewStageSelectScene", 2.5f);
            //BGMをフェードアウトする
            SoundFadeController.SetFadeOutSpeed(-0.005f);
        }
    }
}
