//==============================================================================================
/// File Name	: StageSelectDirecotr.cs（修正予定）
/// Summary		: ステージセレクト制御スクリプト
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class StageSelectDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    // ステート
    private enum StageSelectState
    {
        START,
        PLAYING,
        DECIDE,
        FADE_OUT,
    }



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // プレイヤー
    private GameObject player = null;
    private PlayerController playerController = null;

    // ワイプエフェクト
    private WipeCamera wipeCamera = null;
    // ポーズマネージャー
    private PauseManager pauseManager = null;
    // ポーズ用フレーム
    //private FailedFrameController failedFrameController;
    
    [SerializeField]
    // ドアラッパー
    private GameObject doorWrapper = null;

    // ゲームの進行状況
    private StageSelectState state = StageSelectState.START;
    // 待ち時間用タイマー
    private float waitTime = 0.0f;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        Data.time = 100;

        player = GameObject.Find(ConstPlayer.NAME);
        playerController = player.GetComponent<PlayerController>();

        if (Data.stage_number > 0)
        {
            var doors = doorWrapper.GetComponentsInChildren<DoorToStage>();
            foreach(var door in doors)
            {
                if (door.GetStageNumber() == Data.stage_number)
                {
                    player.transform.position = door.transform.position;
                    GameObject.Find(ConstCamera.CONTROLLER).GetComponent<CameraController>().ResetCameraPos(player.transform.position);
                    break;
                }
            }
        }

        GameObject go = GameObject.Find(PauseManager.NAME);
        if (!go)
        {
            Debug.Log("PauseManagerをシーンに追加してください");
        }

        pauseManager = go.GetComponent<PauseManager>();
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
        switch (state)
        {
            case StageSelectState.START:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    state = StageSelectState.PLAYING;
                }
                break;
            case StageSelectState.PLAYING:
                // ポーズ可能にする
                pauseManager.EnablePauseButton(true);
                break;
            case StageSelectState.DECIDE:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    state = StageSelectState.FADE_OUT;
                }
                break;
            case StageSelectState.FADE_OUT:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0.0f)
                {
                    FadeManager.fadeColor = Color.clear;
                    FadeManager.FadeOut("PlayScene", 1.5f);
                    wipeCamera.StartFadeOut(player.transform.position, 1.4f);
                    waitTime = 99.0f;
                }
                break;
            default:
                DebugLogger.Log("StageSelectScene:StateError");
                break;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ステージの決定
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void DecideStage()
    {
        if (state != StageSelectState.PLAYING)
        {
            return;
        }

        if (!playerController.IsGround())
        {
            return;
        }

        state = StageSelectState.DECIDE;
        waitTime = 0.5f;

        // ポーズ不可にする
        pauseManager.EnablePauseButton(false);
        // プレイヤーの入力を停止する
        playerController.EnableControl(false);
    }
}
