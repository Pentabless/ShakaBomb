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
public class PlayBGM : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    [Header("ステージBGM")]
    AudioClip bgm;
    [SerializeField]
    [Header("ファンファーレ")]
    AudioClip clip;
    [SerializeField]
    float volum;
    [SerializeField]
    int fadeTime = 90;

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
        if (volum == 0.0f)
            volum = 1.0f;
        SoundPlayer.PlayBGM(bgm, volum);
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
    }

    public void GoalEvent()
    {
        SoundPlayer.Play(clip);
        StartCoroutine(FadeBGM());
        // ToDoファンファーレを入れる
    }

    private IEnumerator FadeBGM()
    {
        while(SoundFadeController.GetAudioSource().volume >= 0.0f)
        {
            SoundFadeController.SetFadeOutSpeed(0.0025f);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
