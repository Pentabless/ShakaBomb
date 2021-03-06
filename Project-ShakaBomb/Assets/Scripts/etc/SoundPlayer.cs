﻿using UnityEngine;

//サウンド再生クラス
public class SoundPlayer : MonoBehaviour
{
    private static AudioSource audioSource = null;
    private static GameObject soundPlayerObject = null;

    //サウンド再生用オブジェクト作成
    private static void Init()
    {
        soundPlayerObject = new GameObject("SoundPlayer");
        audioSource = soundPlayerObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        soundPlayerObject.AddComponent<SoundFadeController>();  //フェード用
    }

    // オブジェクトを取得する
    public static AudioSource GetAudioSource()
    {
        if (!soundPlayerObject)
        {
            Init();
        }
        return audioSource;
    }

    //サウンドの再生
    public static void Play(AudioClip audioClip, float volumeScale = 1.0f)
    {
        if (audioSource == null)
        {
            Init();
        }
        audioSource.PlayOneShot(audioClip, volumeScale);
    }

    //BGMの再生
    public static void PlayBGM(AudioClip audioClip, float volumeScale = 1.0f)
    {
        if (audioSource == null)
        {
            Init();
        }
        audioSource.clip = audioClip;
        audioSource.volume = volumeScale;
        audioSource.Play();
    }

    //サウンドの停止
    public static void Stop()
    {
        if (audioSource == null)
        {
            Init();
        }
        audioSource.Stop();
    }
}
