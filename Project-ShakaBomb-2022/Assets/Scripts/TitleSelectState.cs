using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using ImageAnimation;
public class TitleSelectState : TitleState
{

    [SerializeField]
    List<TextMeshProUGUI> menuTextList = default;

    Tween blackBandPanelAnim = default;
    Tween titleLogoAnim = default;
    Tween copyrightAnim = default;

    public override void Initialize()
    {
        return;
    }
    
    public override void Execute()
    {
        Debug.Log("select");
        if (Input.GetKeyDown(KeyCode.X))
        {
            ReturnTransitionProcess();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            BeforeTransitionProcess(TitleDirector.TitleScreenState.DATA_SELECT);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            BeforeTransitionProcess(TitleDirector.TitleScreenState.OPTION);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            BeforeTransitionProcess(TitleDirector.TitleScreenState.LICENSE);
        }
    }

    private void BeforeTransitionProcess(TitleDirector.TitleScreenState nextState)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,20.0f,0.45f,Ease.InOutQuint));
        sequence.Join(ImageAnimation.ImageAnimation.TweenDoFade(directorInstance.TitleLogo,0.0f,0.5f,Ease.OutQuint));
        sequence.OnComplete(() => directorInstance.ChangeState(nextState));

        //blackBandPanelAnim = ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,20.0f,0.45f,Ease.InOutQuint);
        //titleLogoAnim = ImageAnimation.ImageAnimation.TweenDoFade(directorInstance.TitleLogo,0.0f,0.5f,Ease.OutQuint);
        //titleLogoAnim.OnComplete(() => directorInstance.ChangeState(nextState));
    }

    private void ReturnTransitionProcess()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(ImageAnimation.TextAnimation.TweenDoFade(directorInstance.CopyrightText,1.0f,0.5f,Ease.InOutQuint));
        sequence.Join(ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,1.0f,0.5f,Ease.InOutQuint));
        sequence.Join(ImageAnimation.ImageAnimation.TweenDoScale(directorInstance.TitleLogo,0.75f,0.5f,Ease.InOutCirc));
        sequence.OnComplete(() => directorInstance.ChangeState(TitleDirector.TitleScreenState.START));

        //copyrightAnim = ImageAnimation.TextAnimation.TweenDoFade(directorInstance.CopyrightText,1.0f,0.45f,Ease.InOutQuint);
        //blackBandPanelAnim = ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,1.0f,0.45f,Ease.InOutQuint);
        //titleLogoAnim = ImageAnimation.ImageAnimation.TweenDoScale(directorInstance.TitleLogo,0.75f,0.5f,Ease.InOutCirc);
        //titleLogoAnim.OnComplete(() => directorInstance.ChangeState(TitleDirector.TitleScreenState.START));
    }
}
