//==============================================================================================
/// File Name	: TitleScreen.cs
/// Summary		: タイトルスクリーン
//==============================================================================================
using UnityEngine;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
//==============================================================================================
public class TitleScreen : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
	//------------------------------------------------------------------------------------------

    [SerializeField]
    private TitleView _titleView = null;

    private KeyAction _keyAction = null;

    private bool isPlayAnimation = false;

    private void Start() 
    {
        SetTitle();
    }
    private void Update() 
    {
        if(isPlayAnimation)
            return;

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            _keyAction.InputZ?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            _keyAction.InputQ?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            _keyAction.InputW?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            _keyAction.InputE?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            _keyAction.InputX?.Invoke();
        }
    }

    /// <summary>
    /// タイトルの設定
    /// </summary>
    private void SetTitle()
    {
        _keyAction = new KeyAction();
        _keyAction.InputZ = () => StartAnimation().Forget();
    }

    /// <summary>
    /// スタートアニメーション
    /// </summary>
    /// <returns></returns>
    private async UniTask StartAnimation()
    {
        isPlayAnimation = true;
        var sequence = DOTween.Sequence();
            sequence.Append(_titleView.PressAnyButtonText.DOFade(0.0f, 0.5f).SetEase(Ease.OutExpo));
            sequence.Join(_titleView.CopyrightText.DOFade(0.0f, 0.5f).SetEase(Ease.InOutQuint));
            sequence.Join(_titleView.BlackBandPanel.rectTransform.DOScaleY(2.5f, 0.5f).SetEase(Ease.InOutQuint));
            sequence.Join(_titleView.TitleLogo.rectTransform.DOScale(0.6f,0.5f).SetEase(Ease.InOutCirc));

        await sequence.AsyncWaitForCompletion();

        OpenSelectMenu();
    }

    private void CloseAnimation()
    {
        isPlayAnimation = true;
        var sequence = DOTween.Sequence();
            sequence.Append(_titleView.CopyrightText.DOFade(1.0f, 0.5f).SetEase(Ease.InOutQuint));
            sequence.Join(_titleView.BlackBandPanel.rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.InOutQuint));
            sequence.Join(_titleView.TitleLogo.rectTransform.DOScale(0.75f,0.5f).SetEase(Ease.InOutCirc));

        sequence.OnComplete(() => isPlayAnimation = false);
    }

    /// <summary>
    /// 選択メニューを開く
    /// </summary>
    private void OpenSelectMenu()
    {
        _titleView.MenuObject.SetActive(true);
        _titleView.ExitButton.SetActive(true);
        isPlayAnimation = false;
        _keyAction.InputX = CloseSelectMenu;
    }

    /// <summary>
    /// 選択メニューを閉じる
    /// </summary>
    private void CloseSelectMenu()
    {
        _titleView.MenuObject.SetActive(false);
        _titleView.ExitButton.SetActive(false);
        CloseAnimation();
    }
}

/// <summary>
/// キーアクション用モデルクラス
/// </summary>
public class KeyAction
{
    public Action InputZ;

    public Action InputX;

    public Action InputQ;

    public Action InputW;

    public Action InputE;
}
