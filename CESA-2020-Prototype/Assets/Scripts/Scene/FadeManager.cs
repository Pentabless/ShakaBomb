using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//フェード処理用スクリプト
public class FadeManager : MonoBehaviour
{
    private static Canvas fadeCanvas;           // フェード用Canvas
    private static Image fadeImage;             // フェード用Image
                                                   
    private static float alpha = 0.0f;            // フェード用Imageの透明度
                                                   
    public static bool isFadeIn = false;        // フェード処理のフラグ
    public static bool isFadeOut = false;          
                                                   
    private static float fadeTime = 1.0f;         // フェードにかかる時間
                                                   
    private static int nextSceneId = -1;        // 遷移先のシーンID
    private static string nextSceneName = "";   // 遷移先のシーン名

    public static Color fadeColor { get; set; } = new Color(0, 0, 0, 1); // フェード用カラー

    // フェード用のCanvasとImage生成
    private static void Init()
    {
        // フェード用のCanvas生成
        GameObject fadeCanvasObject = new GameObject("CanvasFade");
        fadeCanvas = fadeCanvasObject.AddComponent<Canvas>();
        fadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvasObject.AddComponent<FadeManager>();

        // 最前面になるよう適当なソートオーダー設定
        fadeCanvas.sortingOrder = 110;
        // カメラの割り当て
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        fadeCanvas.planeDistance = 8;

        // フェード用のImage生成
        fadeImage = new GameObject("ImageFade").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchoredPosition = Vector3.zero;

        fadeImage.rectTransform.sizeDelta = new Vector2(9999, 9999);
    }

    // フェード用のCanvasを取得する
    public static Canvas GetCanvas()
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        return fadeCanvas;
    }

    // フェードイン開始
    public static void FadeIn(float fadeTime = 1.0f)
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        FadeManager.fadeTime = Mathf.Max(fadeTime, 0.001f);
        fadeImage.color = fadeColor;
        isFadeIn = true;
        isFadeOut = false;
        alpha = 1f;
    }

    // フェードアウト開始（シーン遷移）
    public static void FadeOut(string nextSceneName, float fadeTime = 1.0f)
    {
        FadeManager.nextSceneName = nextSceneName;
        FadeOut(-2, fadeTime);
    }

    // フェードアウト開始（シーン遷移）
    public static void FadeOut(int nextSceneId, float fadeTime = 1.0f)
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        // 遷移先のシーンIDの登録
        FadeManager.nextSceneId = nextSceneId;
        FadeManager.fadeTime = Mathf.Max(fadeTime, 0.001f);
        // 画面の色を透明にする
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
        isFadeIn = false;
        alpha = 0f;
    }

    // フェードアウト開始
    public static void FadeOut(float fadeTime = 1.0f)
    {
        FadeOut(-1, fadeTime);
    }

    

    void Update()
    {
        // フラグ有効なら毎フレームフェードイン/アウト処理
        if (isFadeIn)
        {
            // 経過時間から透明度計算
            alpha -= Time.deltaTime / fadeTime;

            // フェードイン終了判定
            if (alpha <= 0f)
            {
                isFadeIn = false;
                alpha = 0f;
                fadeCanvas.enabled = false;
            }

            // フェード用Imageの透明度設定
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeColor.a * alpha);
        }
        else if (isFadeOut)
        {
            // 経過時間から透明度計算
            alpha += Time.deltaTime / fadeTime;

            // フェードアウト終了判定
            if (alpha >= 1f)
            {
                isFadeOut = false;
                alpha = 1f;

                // シーン遷移判定
                if (nextSceneId == -2)
                {
                    // 次のシーンへ遷移
                    SceneManager.LoadScene(nextSceneName);
                }
                else if(nextSceneId != -1)
                {
                    // 次のシーンへ遷移
                    SceneManager.LoadScene(nextSceneId);
                }
            }

            // フェード用Imageの透明度設定
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeColor.a * alpha);
        }
    }
}
