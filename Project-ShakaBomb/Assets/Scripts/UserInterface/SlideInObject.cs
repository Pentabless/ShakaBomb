//==============================================================================================
/// File Name	: SlideInObject.cs
/// Summary		: アタッチしたオブジェクトをスライドインする
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class SlideInObject : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // スライドの速度
    [SerializeField, Header("スライドの速度(0.0 - 10.0)"), Tooltip("スライドの速度を設定する"), Range(0.0f ,10.0f)]
    private float slideSpeed = ConstDecimal.ZERO;
    // スライドの限界
    [SerializeField, Header("スライドの幅"), Tooltip("スライドする幅を設定する")]
    private float slideLimit = ConstDecimal.ZERO;
    // 初期ポジション
    private Vector3 initialPosition = Vector3.zero;
    // スライドインの可否
    private bool isSlideIn = false;
    // スライドアウトの可否
    private bool isSlideOut = false;
    // 表示状態
    private bool isDisplay = false;



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
        // スライドイン処理（右から左）
        SlideInRightToLeft();

        // スライドアウト処理（左から右）
        SlideOutLeftToRight();
    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        isSlideIn = false;
        isSlideOut = false;
        isDisplay = false;
        initialPosition = this.transform.position;
    }



    //------------------------------------------------------------------------------------------
    // summary : スライドイン処理を開始する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void PlayIn()
    {
        isSlideIn = true;
    }



    //------------------------------------------------------------------------------------------
    // summary : スライドアウト処理を開始する
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void PlayOut()
    {
        isSlideOut = true;
    }



    //------------------------------------------------------------------------------------------
    // summary : スライドイン処理
    // remarks : X軸（右から左へ）
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SlideInRightToLeft()
    {
        if (!isDisplay && isSlideIn)
        {
            // スライドイン
            this.gameObject.transform.position -= new Vector3(slideSpeed, ConstDecimal.ZERO, ConstDecimal.ZERO);

            var positionX = this.transform.position.x;
            var limit = (initialPosition.x - slideLimit);

            if (positionX < limit)
            {
                isSlideIn = false;
                isDisplay = true;
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : スライドアウト処理
    // remarks : X軸（左から右へ）
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void SlideOutLeftToRight()
    {
        if (isDisplay && isSlideOut)
        {
            // スライドアウト
            this.gameObject.transform.position += new Vector3(slideSpeed, ConstDecimal.ZERO, ConstDecimal.ZERO);

            var positionX = this.transform.position.x;
            var limit = initialPosition.x;

            if (positionX > limit)
            {
                isSlideOut = false;
                isDisplay = false;
            }
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : スライドインオブジェクトの表示状態を返す
    // remarks : none
    // param   : none
    // return  : bool
    //------------------------------------------------------------------------------------------
    public bool GetObjectState()
    {
        return isDisplay;
    }
}
