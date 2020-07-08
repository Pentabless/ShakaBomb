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
using System.Linq;
using UnityEngine.UI;
//==============================================================================================
public class PrologueDirector : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    List<GameObject> images;
    [SerializeField]
    float fadeTime;

    float count;
    int imageIndex = 0;
    bool isCalled = false;

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
        // 最初のもの以外SetActiveをFalse
        var other = images.Skip(1);
        foreach(var obj in other)
        {
            obj.SetActive(false);
        }
        FadeManager.FadeIn();
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        // フェードがかかっていないとき更新
        if(!FadeManager.isFadeIn && !FadeManager.isFadeOut)
        {
            count += Time.deltaTime;
        }

        if (count >= fadeTime)
        {
            // フェード開始
            FadeManager.FadeOut();
            // カウント初期化
            count = 0.0f;
            isCalled = true;
            //　画像が最後か
            if (imageIndex == images.Count() - 1)
            {
                // サウンドのフェードを開始
                SoundFadeController.SetFadeOutSpeed(0.01f);
            }
        }

        // フェードが完了したかどうかの確認
        if (!FadeManager.isFadeOut && isCalled)
        {
            // 画像の切り替え
            images[imageIndex].SetActive(false);
            // 添え字の更新
            imageIndex++;
            if (imageIndex != images.Count())
            {
                // 次画像の表示
                images[imageIndex].SetActive(true);
                // フェードイン開始
                FadeManager.FadeIn();
                isCalled = false;
            }
            else
            {
                SceneManager.LoadScene("TutorialScene");
            }
        }
    }
}
