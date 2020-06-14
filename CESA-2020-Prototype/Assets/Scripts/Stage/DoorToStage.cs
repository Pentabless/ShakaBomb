//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using UnityEngine.SceneManagement;
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
    bool touch_player = false;
    //シャッターの上がり具合
    float shutter_up = 0.0f;
    //シャッターの音を鳴らしたか
    bool play_sutter_sound = true;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {

    }

    //------------------------------------------------------------------------------------------
    // Start
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

        cameraController = GameObject.Find(Common.Camera.CONTROLLER).GetComponent<CameraController>();

        //画像を変更する
        switch (rank)
        {
            case 0:
                //プレイできるか
                if (can_play)
                {
                    //シャッター：きたない
                    shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_sprite[0];
                    //ドアフレーム：きたない
                    door_frame.GetComponent<SpriteRenderer>().sprite = door_frame_sprite[0];
                    //ランプ：全部赤
                    lamp.GetComponent<SpriteRenderer>().sprite = lamp_sprite[1];
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
                    shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_sprite[0];
                    //ドアフレーム：きたない
                    door_frame.GetComponent<SpriteRenderer>().sprite = door_frame_sprite[0];
                    //ランプ：光っていない
                    lamp.GetComponent<SpriteRenderer>().sprite = lamp_sprite[0];
                    //シャッターの上がり具合
                    shutter_up = 0.0f;
                }
                break;
            case 1:
                //シャッター：きれい
                shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_sprite[1];
                //ドアフレーム：きたない
                door_frame.GetComponent<SpriteRenderer>().sprite = door_frame_sprite[0];
                //ランプ：1つ光っている
                lamp.GetComponent<SpriteRenderer>().sprite = lamp_sprite[2];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
            case 2:
                //シャッター：きれい
                shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_sprite[1];
                //ドアフレーム：きれい
                door_frame.GetComponent<SpriteRenderer>().sprite = door_frame_sprite[1];
                //ランプ：2つ光っている
                lamp.GetComponent<SpriteRenderer>().sprite = lamp_sprite[3];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
            case 3:
                //シャッター：きれい
                shutter.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = shutter_sprite[1];
                //ドアフレーム：きれい
                door_frame.GetComponent<SpriteRenderer>().sprite = door_frame_sprite[1];
                //ランプ：3つ光っている
                lamp.GetComponent<SpriteRenderer>().sprite = lamp_sprite[4];
                //シャッターの上がり具合
                shutter_up = ShutterMaxHeight;
                break;
        }

        //シャッターの上がり具合を影響させる
        shutter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.0f, shutter_up, 0.0f);

        //親オブジェクトがあって　親オブジェクトの拡大率が 1,1,1でない時
        if (transform.parent != null)
        {
            if (transform.parent.localScale != new Vector3(1.0f, 1.0f, 1.0f))
            {
                //親オブジェクトの拡大率の影響をなくす
                transform.localScale = new Vector3(1.0f / transform.parent.localScale.x, 1.0f / transform.parent.localScale.y, 1.0f);
            }
        }
    }

    //------------------------------------------------------------------------------------------
    // Update
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
                    SoundPlayer.Play(sound);
                    //音を鳴らしたことにする
                    play_sutter_sound = true;
                }
            }
        }
        //シャッターの上がり具合を影響させる
        shutter.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0.0f, shutter_up, 0.0f);

        var goStage = Input.GetAxis(Player.VERTICAL);

        if (goStage >= 1.0f && touch_player || Input.GetKeyDown(KeyCode.UpArrow) && touch_player)
        {
            //プレイできるドアだったら
            if (SharedData.instance.GetCanPlay(numStage - 1))
            {
                // ToDo:静的な変数に代入
                Data.stage_number = numStage;
                GameObject.Find(NewStageSelectDirector.NAME).GetComponent<NewStageSelectDirector>().DecideStage();
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == Player.NAME)
        {
            touch_player = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        touch_player = false;
    }

    //扉のステージ番号を渡す
    public int GetStageNumber()
    {
        return numStage;
    }

    public bool GetTouchPlayer()
    {
        return touch_player;
    }
}
