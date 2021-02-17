//==============================================================================================
/// File Name	: PlayDirector.cs
/// Summary		: プレイシーンの進行管理
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class PlayDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    // プレイシーンの状態
    enum PlayState
    {
        START,      // 開始演出
        PLAYING,    // プレイ中
        GOAL,       // ゴール演出
        FAILED,     // クリア失敗演出
        RESULT,     // リザルト演出
        TUTORIAL_GOAL,       // チュートリアル専用ゴール演出
        TUTORIAL_RESULT,     // チュートリアル専用リザルト演出
    }



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // ステージのタイムデータ
    [SerializeField]
    private TimeData timeData;
    // 残り時間
    [SerializeField]
    private float time = 0.0f;
    // イベント時にフェードアウトを適用させるオブジェクト
    [SerializeField]
    private List<GameObject> uiObjects;
    // プレイヤー
    private GameObject player;
    private PlayerController playerController;
    // ワイプエフェクト
    private WipeCamera wipeCamera;
    // ポーズマネージャー
    private PauseManager pauseManager;
    // クリア失敗用フレーム
    private FailedFrameController failedFrameController;
    // ゲームの進行状況
    private PlayState state;
    // 待ち時間用タイマー
    private float waitTime = 0.0f;
    // タイムデータ
    [System.Serializable]
    public struct TimeData
    {
        public float timeLimit; // 制限時間
        public float star3Time; // 星3のタイム
        public float star2Time; // 星2のタイム
        public float star1Time; // 星1のタイム
    }



    //------------------------------------------------------------------------------------------
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        state = PlayState.START;
        Data.time = time = timeData.timeLimit;
        Data.timeLimit = timeData.timeLimit;
        Data.star_num = 0;
    }



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(ConstPlayer.NAME);
        playerController = player.GetComponent<PlayerController>();
        GameObject go = GameObject.Find(PauseManager.NAME);
        if (!go)
        {
            Debug.Log("PauseManagerをシーンに追加してください");
        }
        pauseManager = go.GetComponent<PauseManager>();
        go = GameObject.Find(ConstFailedFrame.NAME);
        if (!go)
        {
            Debug.Log("FailedFrameをシーンに追加してください\nPauseManagerのignoreObjectsにFailedFrameを追加してください");
        }
        failedFrameController = go.GetComponent<FailedFrameController>();

        // シーン開始時にフェードインする
        FadeManager.FadeIn(0.01f);
        wipeCamera = GameObject.Find(ConstCamera.MAIN_CAMERA).GetComponent<WipeCamera>();
        wipeCamera.StartFadeIn(player.transform.position, 1.0f);
        waitTime = 1.0f;

    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        // ステートで処理を分岐する
        switch (state)
        {
            case PlayState.START:   UpdateStart();      break;
            case PlayState.PLAYING: UpdatePlaying();    break;
            case PlayState.GOAL:    UpdateGoal();       break;
            case PlayState.FAILED:  UpdateFailed();     break;
            case PlayState.RESULT:  UpdateResult();     break;
            case PlayState.TUTORIAL_GOAL: UpdateTutorialGoal(); break;
            case PlayState.TUTORIAL_RESULT: UpdateTutorialResult(); break;
            default:
                DebugLogger.Log("PlayDirecotr:StateError");
                break;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : スタート中の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateStart()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.PLAYING;
            GameObject go = GameObject.Find(AreaNameScript.NAME);
            if (go)
            {
                var areaNameScript = go.GetComponent<AreaNameScript>();
                if (Data.stage_number > 0)
                {
                    string text = "第" + (Data.stage_number) + "区画";
                    areaNameScript.ShowAreaName(text, 1.0f, 2.0f);
                    // ポーズ可能にする
                    pauseManager.EnablePauseButton(true);
                }
                else if (Data.stage_number == 0)
                {
                    areaNameScript.ShowAreaName("チュートリアル", 1.0f, 2.0f);
                    areaNameScript.SetOutlineColor(new Color(0.08f, 0.36f, 0, 0.5f));
                    areaNameScript.SetBackColor(new Color(0.3f, 0.9f, 0, 0.5f));
                    areaNameScript.SetBackScale(new Vector3(1.5f, 1, 1));
                    // ポーズ不可にする
                    pauseManager.EnablePauseButton(false);
                }
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : プレイ中の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdatePlaying()
    {
        time -= Time.deltaTime;

        if (wipeCamera.currentEffect != WipeCamera.PostEffects.Alert &&
            time <= timeData.timeLimit / 6)
        {
            wipeCamera.StartAlert();
        }

        // 時間切れになったらクリア失敗になる
        if (time <= 0.0f)
        {
            time = 0.0f;
            TimeUp();
        }
        Data.time = time;
    }



    //------------------------------------------------------------------------------------------
    // summary : ゴール時の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateGoal()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.RESULT;
            waitTime = 0.0f;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : チュートリアルのゴール時処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateTutorialGoal()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            state = PlayState.TUTORIAL_RESULT;
            waitTime = 0.0f;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : クリア失敗時の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateFailed()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            waitTime = 999.0f;
            failedFrameController.EnableFrame(FailedFrameController.FailedType.TIME_UP);
            pauseManager.ResetFilterColor();
            pauseManager.Pause(1.0f);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : リザルト時の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateResult()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            FadeManager.fadeColor = Color.clear;
            FadeManager.FadeOut(ConstScene.RESULT, ConstScene.FADE_TIME);
            wipeCamera.StartFadeOut(player.transform.position, 1.4f);
            waitTime = 99.0f;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : チュートリアルのリザルト時の処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateTutorialResult()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0.0f)
        {
            FadeManager.fadeColor = Color.clear;
            FadeManager.FadeOut(ConstScene.STAGE_SELECT, ConstScene.FADE_TIME);
            wipeCamera.StartFadeOut(player.transform.position, 1.4f);
            waitTime = 99.0f;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ゴールイベント
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void Goal()
    {
        state = PlayState.GOAL;
        waitTime = 2.0f;
        // ポーズ不可にする
        pauseManager.EnablePauseButton(false);
        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // プレイヤーをゴールの位置に移動させる
        playerController.EnableAutoControl(true);
        playerController.SetTargetPos(GameObject.Find(ConstGoal.NAME).transform.position);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());

        if(wipeCamera.currentEffect == WipeCamera.PostEffects.Alert)
        {
            wipeCamera.StartAlert();
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : チュートリアルのゴールイベント
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void TutorialGoal()
    {
        state = PlayState.TUTORIAL_GOAL;
        waitTime = 2.0f;
        // ポーズ不可にする
        pauseManager.EnablePauseButton(false);
        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // プレイヤーをゴールの位置に移動させる
        playerController.EnableAutoControl(true);
        playerController.SetTargetPos(GameObject.Find("TutorialGoal").transform.position);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());
    }



    //------------------------------------------------------------------------------------------
    // summary : タイムアップ時のイベント
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void TimeUp()
    {
        state = PlayState.FAILED;
        waitTime = 3.0f;
        // ポーズ不可にする
        pauseManager.EnablePauseButton(false);
        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
        // UIをフェードアウトさせる
        StartCoroutine(FadeOutUICoroutine());
    }



    //------------------------------------------------------------------------------------------
    // summary : UIのフェード処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private IEnumerator FadeOutUICoroutine()
    {
        var texts = Utility.GetAllComponents<Text>(uiObjects, true);
        var images = Utility.GetAllComponents<Image>(uiObjects, true);

        var textColor = Utility.GetColors(texts);
        var imageColor = Utility.GetColors(images);

        for (int i = 1; i <= 90; ++i)
        {
            float alpha = (90 - i) / 90.0f;
            Color color;
            for(int j = 0; j < texts.Count; j++)
            {
                color = textColor[j];
                color.a *= alpha;
                texts[j].color = color;
            }
            for (int j = 0; j < images.Count; j++)
            {
                color = imageColor[j];
                color.a *= alpha;
                images[j].color = color;
            }
           
            yield return null;
        }
    }
}
