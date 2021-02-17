//==============================================================================================
/// File Name	: PlayBGM.cs
/// Summary		: 
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class PlayBGM : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField, Header("ステージBGM")]
    private AudioClip bgm = null;
    [SerializeField, Header("ファンファーレ")]
    private AudioClip clip = null;
    [SerializeField]
    private float volum = ConstDecimal.ZERO;
    [SerializeField]
    private int fadeTime = 90;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        if (volum == 0.0f)
            volum = 1.0f;
        SoundPlayer.PlayBGM(bgm, volum);
    }



    //------------------------------------------------------------------------------------------
    // summary : ゴールイベント
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    public void GoalEvent()
    {
        SoundPlayer.Play(clip);
        SoundFadeController.SetFadeOutSpeed(0.0020f);
    }
}
