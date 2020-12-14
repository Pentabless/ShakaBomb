//==============================================================================================
/// File Name	: DoorToStage.cs（修正予定）
/// Summary		: 
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class DoorToStage : MonoBehaviour
{
    //シャッターのスプライト
    public Sprite[] shutter_sprite;
    //ドアフレームのスプライト
    public Sprite[] door_frame_sprite;
    //ランプのスプライト
    public Sprite[] lamp_sprite;
    //シャッター音
    public AudioClip sound;
    //シャッターが当たる速さ
    public float up_speed;



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // 対応するステージ番号
    [SerializeField]
    int numStage;
    GameObject shutter;      //シャッター
    GameObject door_frame;   //ドアフレーム
    GameObject lamp;         //ランプ
    CameraController cameraController; // カメラ
    //シャッターが上がる最大の高さ
    const float ShutterMaxHeight = 3.0f;
    //シャッターが上がるアニメーションをするか
    bool animate_shutter = false;
    //プレイヤーがドアに当たっているか
    bool touchPlayer = false;
    //シャッターの上がり具合
    float shutter_up = 0.0f;
    //シャッターの音を鳴らしたか
    bool play_sutter_sound = true;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        //自身の番号のステージのデータを貰う
        bool can_play = SharedData.instance.GetCanPlay(numStage - 1);
        int purification = SharedData.instance.GetPurification(numStage - 1);
        int rank = SharedData.instance.GetPercentRank(purification);

        //子オブジェクトを探す
        shutter = transform.GetChild(1).gameObject;      //シャッター
        door_frame = transform.GetChild(2).gameObject;   //ドアフレーム
        lamp = transform.GetChild(3).gameObject;         //ランプ

        cameraController = GameObject.Find(ConstCamera.CONTROLLER).GetComponent<CameraController>();

        Sprite shutter_tex=null;
        Sprite door_tex=null;
        Sprite lamp_tex=null;

        //画像を変更する
        switch (rank)
        {
            case 0:
                //プレイできるか
                if (can_play)
                {
                    //シャッター：きたない
                    shutter_tex = shutter_sprite[0];
                    //ドアフレーム：きたない
                    door_tex = door_frame_sprite[0];
                    //ランプ：全部赤
                    lamp_tex = lamp_sprite[1];
                    //クリアしていたら
                    if (SharedData.instance.GetClear(numStage - 1))
                    {
                        //シャッターの上がり具合
                        shutter_up = ShutterMaxHeight;
                    }
                    else
                    {
                        //シャッターの上がり具合
                        shutter_up = 0.0f;
                        //シャッターのアニメーションをする
                        animate_shutter = true;
                        //音を鳴らしていない状態にする
                        play_sutter_sound = false;
                        //// 音を入れる
                        //SoundPlayer.Play(sound);
                    }
                }
                else
                {
                    //シャッター：きたない
                    shutter_tex = shutter_sprite[0];
                    //ドアフレーム：きたない
                    door_tex = door_frame_sprite[0];
                    //ランプ：光っていない
                    lamp_tex = lamp_sprite[0];
                    //シャッターの上がり具合
                    shutter_up = 0.0f;
                }
                break;
            case 1:
                //シャッター：きれい
                shutter_tex = shutter_sprite[1];
                //ドアフレーム：きたない
                door_tex = door_frame_sprite[0];
                //ランプ：1つ光っている
                lamp_tex = lamp_sprite[2];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
            case 2:
                //シャッター：きれい
                shutter_tex = shutter_sprite[1];
                //ドアフレーム：きれい
                door_tex = door_frame_sprite[1];
                //ランプ：2つ光っている
                lamp_tex = lamp_sprite[3];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
            case 3:
                //シャッター：きれい
                shutter_tex = shutter_sprite[1];
                //ドアフレーム：きれい
                door_tex = door_frame_sprite[1];
                //ランプ：3つ光っている
                lamp_tex = lamp_sprite[4];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
            default:
                shutter_tex = null;
                door_tex = null;
                lamp_tex = null;
                break;
        }

        //画像を設定する
        shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_tex;
        door_frame.GetComponent<SpriteRenderer>().sprite = door_tex;
        lamp.GetComponent<SpriteRenderer>().sprite = lamp_tex;

        //シャッターの上がり具合を影響させる
        shutter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.0f, shutter_up, 0.0f);
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        //シャッターが上がるアニメーションをする
        if (animate_shutter)
        {
            var topLeft = cameraController.GetScreenTopLeft();
            var bottomRight = cameraController.GetScreenBottomRight();
            if (topLeft.x < transform.position.x && transform.position.x < bottomRight.x &&
                topLeft.y > transform.position.y && transform.position.y > bottomRight.y)
            {
                shutter_up += up_speed;
                //シャッターの音を鳴らしていなかったら
                if(play_sutter_sound==false)
                {
                    // 音を鳴らす
                    SoundPlayer.Play(sound,0.5f);
                    //音を鳴らしたことにする
                    play_sutter_sound = true;
                }
            }
        }
        //シャッターの上がり具合を影響させる
        shutter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.0f, shutter_up, 0.0f);

        var goStage = Input.GetAxis(ConstPlayer.VERTICAL);

        if (goStage >= 1.0f && touchPlayer || Input.GetKeyDown(KeyCode.UpArrow) && touchPlayer)
        {
            //プレイできるドアだったら
            if (SharedData.instance.GetCanPlay(numStage - 1))
            {
                // ToDo:静的な変数に代入
                Data.stage_number = numStage;
                GameObject.Find(ConstDirector.STAGE_SELECT).GetComponent<StageSelectDirector>().DecideStage();
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : OnTriggerStay2D
    // remarks : none
    // param   : Collider2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == ConstPlayer.NAME)
        {
            touchPlayer = true;
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : OnTriggerExit2D
    // remarks : none
    // param   : Collider2D
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        touchPlayer = false;
    }



    //------------------------------------------------------------------------------------------
    // summary : GetStageNumber
    // remarks : 扉のステージ番号を渡す
    // param   : none
    // return  : int
    //------------------------------------------------------------------------------------------
    public int GetStageNumber()
    {
        return numStage;
    }



    //------------------------------------------------------------------------------------------
    // summary : GetTouchPlayer
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool GetTouchPlayer()
    {
        return touchPlayer;
    }
}
