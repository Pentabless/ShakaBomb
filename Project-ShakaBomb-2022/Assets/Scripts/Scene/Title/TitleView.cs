//==============================================================================================
/// File Name	: TitleView.cs
/// Summary		: タイトルビュー
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    /// <summary>
    /// ヘッダータイトル
    /// </summary>
    public HeaderTitle HeaderTitle;

    /// <summary>
    /// データメニューオブジェクト
    /// </summary>
    public GameObject DataMenu;

    /// <summary>
    /// フッターオブジェクト
    /// </summary>
    public GameObject Footer;

    /// <summary>
    /// メニューオブジェクト
    /// </summary>
    public GameObject MenuObject;

    /// <summary>
    /// 閉じるボタン
    /// </summary>
    public GameObject ExitButton;

    /// <summary>
    /// スタートボタン
    /// </summary>
    public TextMeshProUGUI PressAnyButtonText;

    /// <summary>
    /// コピーライトテキスト
    /// </summary>
    public TextMeshProUGUI CopyrightText;

    /// <summary>
    /// ブラックパネル
    /// </summary>
    public RawImage BlackBandPanel;

    /// <summary>
    /// タイトルロゴ
    /// </summary>
    public Image TitleLogo;
}
