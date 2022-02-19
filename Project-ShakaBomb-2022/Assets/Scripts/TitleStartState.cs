using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using ImageAnimation;

public class TitleStartState : TitleState
{
    [SerializeField]
    TextMeshProUGUI pressAnyButtonText = default;

    Tween blackBandPanelAnim = default;
    Tween titleLogoAnim = default;
    Tween copyrightAnim = default;

    public override void Initialize()
    {
        return;
    }
    
    public override void Execute()
    {
        Debug.Log("start");
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var sequence = DOTween.Sequence();
            sequence.Append(ImageAnimation.TextAnimation.TweenDoFade(pressAnyButtonText,0.0f,0.5f,Ease.OutExpo));
            sequence.Join(ImageAnimation.TextAnimation.TweenDoFade(directorInstance.CopyrightText,0.0f,0.5f,Ease.InOutQuint));
            sequence.Join(ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,2.5f,0.5f,Ease.InOutQuint));
            sequence.Join(ImageAnimation.ImageAnimation.TweenDoScale(directorInstance.TitleLogo,0.6f,0.5f,Ease.InOutCirc));
            sequence.OnComplete(() => directorInstance.ChangeState(TitleDirector.TitleScreenState.SELECT));

            //ImageAnimation.TextAnimation.TweenDoFade(pressAnyButtonText,0.0f,0.45f,Ease.OutExpo);
            //copyrightAnim = ImageAnimation.TextAnimation.TweenDoFade(directorInstance.CopyrightText,0.0f,0.45f,Ease.InOutQuint);
            //blackBandPanelAnim = ImageAnimation.ImageAnimation.TweenDoScaleY(directorInstance.BlackBandPanel,2.5f,0.45f,Ease.InOutQuint);
            //titleLogoAnim = ImageAnimation.ImageAnimation.TweenDoScale(directorInstance.TitleLogo,0.6f,0.5f,Ease.InOutCirc);
            //titleLogoAnim.OnComplete(() => directorInstance.ChangeState(TitleDirector.TitleScreenState.SELECT));
        }
    }
}



