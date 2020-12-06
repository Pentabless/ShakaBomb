//==============================================================================================
/// File Name	: AppealImage.cs
/// Summary		: Imageにアピール処理を行う
//==============================================================================================
using UnityEngine;
using UnityEngine.UI;
//==============================================================================================
public class AppealImage : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private RectTransform rectTransform = null;
    private Image image = null;
    // 拡大用
    private Vector2 size = Vector2.zero;
    // 初期のサイズ
    private Vector2 originalSize = Vector2.zero;
    // 拡大量
    [SerializeField, Header("拡大量"), Tooltip("拡大する量を指定する"), Range(0.0f, 100.0f)]
    private float scaleValue = 0.0f;
    // 最大サイズ（割合）
    [SerializeField, Header("拡大した際の最大サイズ（割合）"), Tooltip("拡大した際の最大サイズを割合で指定する"), Range(0.0f, 100.0f)]
    private float scalePercentage = 0.0f;
    // 最大サイズ
    private Vector2 maxSize = Vector2.zero;
    // アピールするか
    private bool isAppeal = false;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
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
        if (isAppeal && (maxSize.x >= size.x))
        {
            image.color = GetReduceAlpha(image.color);
            size += new Vector2(scaleValue, scaleValue);
            rectTransform.sizeDelta = new Vector2(size.x, size.y);
        }
        else if((maxSize.x <= size.x))
        {
            image.gameObject.SetActive(false);
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : 初期化処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Init()
    {
        // Imageコンポーネントを取得する
        image = this.gameObject.GetComponent<Image>();
        // RectTransformコンポーネントを取得する
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        // 初期のサイズを保存
        originalSize = rectTransform.sizeDelta;
        // 拡大用
        size = rectTransform.sizeDelta;
        // 最大サイズを求める
        maxSize = ((originalSize * scalePercentage) / 100);
        maxSize = (originalSize + maxSize);
    }



    //------------------------------------------------------------------------------------------
    // summary : アピール処理を行う
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void Play()
    {
        isAppeal = true;
    }



    //------------------------------------------------------------------------------------------
    // summary : Alpha値を減らして返す
    // remarks : 引数で指定されたオブジェクトのAlpha値を減らして返す
    // param   : Color
    // return  : Color
    //------------------------------------------------------------------------------------------
    private Color GetReduceAlpha(Color color)
    {
        // Alpha値を減らす
        color.a -= 0.05f;

        return color;
    }
}
