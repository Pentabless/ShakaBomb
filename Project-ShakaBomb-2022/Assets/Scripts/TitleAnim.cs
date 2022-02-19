using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace ImageAnimation{
    #region スケールY
    public class ImageAnimation
    {
        static public Tween TweenDoScaleY(RawImage rawImage, float a, float b, DG.Tweening.Ease ease)
        {
            return rawImage.rectTransform.DOScaleY(a, b).SetEase(ease);
        }

        static public Tween TweenDoScale(Image image, float a, float b, DG.Tweening.Ease ease)
        {
            return image.rectTransform.DOScale(a, b).SetEase(ease);
        }

        static public Tween TweenDoFade(Image image,float a, float b, DG.Tweening.Ease ease)
        {
            return image.DOFade(a, b).SetEase(ease);
        }
    }
    #endregion

    #region フェード
    public class TextAnimation
    {
        static public Tween TweenDoFade(TextMeshProUGUI tmp,float a, float b, DG.Tweening.Ease ease)
        {
            return tmp.DOFade(a, b).SetEase(ease);
        }
    }
    #endregion
}