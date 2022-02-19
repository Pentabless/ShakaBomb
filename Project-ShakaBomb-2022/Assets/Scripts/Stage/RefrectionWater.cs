//==============================================================================================
/// File Name	: ReflectionWater.cs
/// Summary		: 反射する水面
//==============================================================================================
using UnityEngine;
//==============================================================================================
[ExecuteInEditMode]
public class RefrectionWater : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // レンダラー
    private new Renderer renderer = null;
    // メインカメラ
    private Camera mainCamera = null;
    // タイリング
    [SerializeField]
    private Vector2 tiling = Vector2.one;
    // スクロール速度
    [SerializeField]
    private float scrollSpeed = 0.5f;
    // テクスチャに乗算する色
    [SerializeField, Header("テクスチャに乗算する色")]
    private Color filterColor = Color.white;
    // テクスチャにブレンドする色
    [SerializeField, Header("テクスチャにブレンドする色")]
    private Color blendColor = Color.clear;
    // 彩度の倍率
    [SerializeField, Header("彩度の倍率")]
    private float saturationRate = 1.0f;
    // 明度の倍率
    [SerializeField, Header("明度の倍率")]
    private float brightnessRate = 1.0f;
    // 揺らぎテクスチャのオフセット
    private float offset = 0.0f;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        offset += Time.deltaTime * scrollSpeed;
        float minWorldSpaceY = renderer.bounds.max.y;
        float minScreenSpaceY = mainCamera.WorldToScreenPoint(new Vector3(0, minWorldSpaceY, 0)).y;
        float topEdge = minScreenSpaceY / mainCamera.pixelHeight;

        renderer.sharedMaterial.SetColor("_Tint", filterColor);
        renderer.sharedMaterial.SetColor("_BlendColor", blendColor);
        renderer.sharedMaterial.SetFloat("_SaturationRate", saturationRate);
        renderer.sharedMaterial.SetFloat("_BrightnessRate", brightnessRate);
        renderer.sharedMaterial.SetFloat("_TopEdgePosition", topEdge);
        renderer.sharedMaterial.SetTextureScale("_Displacement", tiling);
        renderer.sharedMaterial.SetTextureOffset("_Displacement", new Vector2(offset, 0));
    }
}
