//==============================================================================================
/// File Name	: GameClearDirector.cs
/// Summary		: ゲームクリアシーンの管理を行う
//==============================================================================================
using UnityEngine;
//==============================================================================================
public class GameClearDirector : MonoBehaviour
{
    // BGMファイル
    [SerializeField, Header("BGMファイル"),Tooltip("BGMファイルをアタッチする")]
    private AudioClip allClearBGM = null;



    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        // BGMを再生する
        SoundPlayer.PlayBGM(allClearBGM, 0.5f);

        // フェードインを行う
        FadeManager.fadeColor = Color.black;
        FadeManager.FadeIn(1.5f);
    }

    

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
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
