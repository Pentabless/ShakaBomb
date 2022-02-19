//==============================================================================================
/// File Name	: StageDataController.cs
/// Summary		: 
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
using Common;
//==============================================================================================
public class StageDataController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    //ドアオブジェクト
    private GameObject[] door_obj;
    ////テキストオブジェクト
    //private GameObject[] text_obj;
    //プレイヤー
    private PlayerController playerController;

    //ステージ番号のロゴ
    private Image image_stage_number_logo;
    //ステージ番号のテキスト
    private Text text_stage_number;
    //ステージ番号の汚れ
    private Image image_dirty_logo;
    //操作説明のロゴ
    private Image image_operation_logo;
    //ロゴの回転角度
    private float number_logo_angle;
    //操作説明の回転角度
    private float operation_logo_angle;
    //ロゴの回転向き
    private bool logo_angle_direction;



    //------------------------------------------------------------------------------------------
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        //**ステージのドア**//*************************************************************
        //シーン内にあるドアオブジェクトを全部探す
        door_obj = GameObject.FindGameObjectsWithTag("StageDoor");
        //ステージデータの初期化をする(初期化していたら何もしない)
        SharedData.instance.SetStageDataSize(door_obj.Length);    //ドアオブジェクトの数だけ
    }



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        //自身のCanvasの設定をする
        SharedData.instance.SetCanvasOption(GetComponent<Canvas>());
        //自身のCanvasScalerの設定をする
        SharedData.instance.SetCanvasScaleOption(GetComponent<CanvasScaler>());

        //**ステージ番号のロゴ**//*********************************************************
        //ステージ番号のロゴを探す
        image_stage_number_logo = GameObject.Find("StageNumberLogo").GetComponent<Image>();
        //ステージ番号のロゴの回転角度を設定する(最初は見えない)
        number_logo_angle = 90.0f;
        logo_angle_direction = false;   //プレイヤーがどのドアにも当たっていない
        image_stage_number_logo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(number_logo_angle, 0.0f, 0.0f);

        //**ステージ番号のテキスト**//*****************************************************
        //ステージ番号のテキストを探す
        text_stage_number = GameObject.Find("StageNumber").GetComponent<Text>();
        //ステージ番号のテキストを初期化
        text_stage_number.text = "ステージ??";

        //**ステージ番号の汚れ**//*****************************************************
        //汚れを探す
        image_dirty_logo = GameObject.Find("LogoDirty").GetComponent<Image>();

        //**操作説明のロゴ**//*****************************************************
        image_operation_logo = GameObject.Find("SmallUILogo").GetComponent<Image>();
        //操作説明のロゴの回転角度を設定する(最初は見えない)
        operation_logo_angle = 110.0f;
        image_operation_logo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(operation_logo_angle, 0.0f, 0.0f);


        playerController = GameObject.Find(ConstPlayer.NAME).GetComponent<PlayerController>();
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        logo_angle_direction = false;
        bool enableJump = true;
        //ドアの数だけ繰り返す
        for (int i = 0; i < door_obj.Length; i++)
        {
            //ドアにプレイヤーが当たっていたら
            if (door_obj[i].GetComponent<DoorToStage>().GetTouchPlayer())
            {
                logo_angle_direction = true;
                int stage_num = door_obj[i].GetComponent<DoorToStage>().GetStageNumber();
                text_stage_number.text = "第" + (stage_num).ToString() + "区画";

                Color color = image_dirty_logo.color;
                //プレイできないステージだったら
                if (SharedData.instance.GetCanPlay(stage_num - 1) == false)
                {
                    //汚れを出す
                    image_dirty_logo.color = new Color(color.r, color.g, color.b, 1.0f);
                }
                else
                {
                    //汚れを出さない
                    image_dirty_logo.color = new Color(color.r, color.g, color.b, 0.0f);
                    enableJump = false;
                }

                break;
            }
        }

        //見えるように回転する時
        if (logo_angle_direction)
        {
            number_logo_angle -= 3.0f;
            //汚れを出していなかったら
            if (image_dirty_logo.color.a != 1.0f)
            {
                operation_logo_angle -= 3.0f;
            }
            //0.0度まで
            if (number_logo_angle < 0.0f)
            {
                number_logo_angle = 0.0f;
            }
            //0.0度まで
            if (operation_logo_angle < 0.0f)
            {
                operation_logo_angle = 0.0f;
            }
        }
        else
        {
            number_logo_angle += 3.0f;
            //汚れを出していなかったら
            if (image_dirty_logo.color.a != 1.0f)
            {
                //少しずらして戻す
                if (number_logo_angle > 20.0f)
                {
                    operation_logo_angle += 3.0f;
                }
            }
            //90.0度まで
            if (number_logo_angle > 90.0f)
            {
                number_logo_angle = 90.0f;
            }
            //110.0度まで
            if (operation_logo_angle > 110.0f)
            {
                operation_logo_angle = 110.0f;
            }
        }
        //回転更新
        image_stage_number_logo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(number_logo_angle, 0.0f, 0.0f);
        image_operation_logo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(operation_logo_angle, 0.0f, 0.0f);
    }



    //------------------------------------------------------------------------------------------
    // summary : SortDoorObjectList
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private GameObject[] SortDoorObjectList()
    {
        //ドアの数分の配列を作る
        GameObject[] obj_list = new GameObject[door_obj.Length];

        //用意した配列の数だけ繰り返す
        for (int i = 0; i < obj_list.Length; i++)
        {
            //ドアの数だけ繰り返す
            for (int j = 0; j < door_obj.Length; j++)
            {
                //配列の要素数がドアのステージ番号とあっていたら
                if (i + 1 == door_obj[j].GetComponent<DoorToStage>().GetStageNumber())
                {
                    //配列に入れる
                    obj_list[i] = door_obj[j];
                    break;
                }
            }
        }

        return obj_list;
    }
}
