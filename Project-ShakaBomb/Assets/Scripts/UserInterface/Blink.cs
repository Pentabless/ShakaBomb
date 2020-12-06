//==============================================================================================
/// File Name	: Blink.cs
/// Summary		: Image、Textを点滅させる
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
public class Blink : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // const variable
    //------------------------------------------------------------------------------------------
    private enum ObjType
    {
        TEXT,
        IMAGE
    }



    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private float speed = 0.0f;
    private Text text = null;
    private Image image = null;
    private float time = 0.0f;
    private ObjType thisObjType = ObjType.TEXT;
    private bool isBlink = false;
    // 通過確認
    private bool isPassed = false;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        // 初期化処理
        Init();
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        // オブジェクトのAlpha値を更新
        UpdateAlpha();

        // ブリンクを停止した際に指定のオブジェクトのAlpha値をリセット
        ResetAlpha();
    }



    //------------------------------------------------------------------------------------------
    // summary : Alpha値を更新してColorを返す
    // remarks : none
    // param   : Color
    // return  : Color
    //------------------------------------------------------------------------------------------
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;

        return color;
    }



    //------------------------------------------------------------------------------------------
    // summary : ブリンク処理を停止する
    // remarks : 外部からブリンク処理を停止させるための関数
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void StopBlink()
    {
        // ブリンク処理を停止する
        isBlink = false;
    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        // ブリンク処理を行うか
        isBlink = true;

        // アタッチしてるオブジェクトを判別
        DetermineObject();
    }



    //------------------------------------------------------------------------------------------
    // summary : Alpha値を最大にして返す
    // remarks : 引数で指定されたオブジェクトのAlpha値を最大にして返す
    // param   : Color
    // return  : Color
    //------------------------------------------------------------------------------------------
    private Color GetMaxAlpha(Color color)
    {
        color.a = 1.0f;

        return color;
    }



    //------------------------------------------------------------------------------------------
    // summary : オブジェクトのAlpha値を更新
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void UpdateAlpha()
    {
        if (isBlink && thisObjType == ObjType.IMAGE)
        {
            image.color = GetAlphaColor(image.color);
        }
        else if (isBlink && thisObjType == ObjType.TEXT)
        {
            text.color = GetAlphaColor(text.color);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ブリンクを停止した際に指定のオブジェクトのAlpha値をリセット
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void ResetAlpha()
    {
        if (!isBlink && !isPassed && thisObjType == ObjType.IMAGE)
        {
            isPassed = true;
            image.color = GetMaxAlpha(image.color);
        }
        else if (!isBlink && !isPassed && thisObjType == ObjType.TEXT)
        {
            isPassed = true;
            text.color = GetMaxAlpha(text.color);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : アタッチしてるオブジェクトを判別
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void DetermineObject()
    {
        if (this.gameObject.GetComponent<Image>())
        {
            thisObjType = ObjType.IMAGE;
            image = this.gameObject.GetComponent<Image>();
        }
        else if (this.gameObject.GetComponent<Text>())
        {
            thisObjType = ObjType.TEXT;
            text = this.gameObject.GetComponent<Text>();
        }
    }
}
