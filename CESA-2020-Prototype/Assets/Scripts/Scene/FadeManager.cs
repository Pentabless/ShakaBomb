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
                                                   
    private static float alpha = 0f;            // フェード用Imageの透明度
                                                   
    public static bool isFadeIn = false;        // フェード処理のフラグ
    public static bool isFadeOut = false;          
                                                   
    private static float fadeTime = 1f;         // フェードにかかる時間
                                                   
    private static int nextSceneID = -1;        // 遷移先のシーンID
    private static string nextSceneName = "";   // 遷移先のシーン名

    // フェード用のCanvasとImage生成
    private static void Init()
    {
        // フェード用のCanvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasFade");
        fadeCanvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        FadeCanvasObject.AddComponent<FadeManager>();

        // 最前面になるよう適当なソートオーダー設定
        fadeCanvas.sortingOrder = 100;

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
    public static void FadeIn()
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        // 画面を黒くする
        fadeImage.color = Color.black;
        isFadeIn = true;
        isFadeOut = false;
        alpha = 1f;
    }

    // フェードアウト開始
    public static void FadeOut()
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        // シーン遷移を行わないようにする
        nextSceneID = -1;
        // 画面の色を透明にする
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
        isFadeIn = false;
        alpha = 0f;
    }

    // フェードアウト開始（シーン遷移）
    public static void FadeOut(int next_scene_id)
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        // 遷移先のシーンIDの登録
        nextSceneID = next_scene_id;
        // 画面の色を透明にする
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
        isFadeIn = false;
        alpha = 0f;
    }

    // フェードアウト開始（シーン遷移）
    public static void FadeOut(string next_scene_name)
    {
        // 生成済みか確認
        if (fadeImage == null)
        {
            Init();
        }
        // シーン名でのシーン遷移を行う
        nextSceneName = next_scene_name;
        nextSceneID = -2;
        // 画面の色を透明にする
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
        isFadeIn = false;
        alpha = 0f;
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
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
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
                if (nextSceneID != -1)
                {
                    // シーン名を使うか判定
                    if (nextSceneID == -2)
                    {
                        // 次のシーンへ遷移
                        SceneManager.LoadScene(nextSceneName);

                    }
                    else
                    {
                        // 次のシーンへ遷移
                        SceneManager.LoadScene(nextSceneID);
                    }
                }
            }

            // フェード用Imageの透明度設定
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
        }
    }
}
