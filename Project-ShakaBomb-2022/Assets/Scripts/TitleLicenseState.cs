using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleLicenseState : TitleState
{
    [SerializeField]
    List<TextMeshProUGUI> textList = default;

    [SerializeField]
    List<RawImage> imageList = default;

    public override void Initialize()
    {
        return;
    }
    
    public override void Execute()
    {
        Debug.Log("ライセンス画面");
        if (Input.GetKeyDown(KeyCode.X))
        {
            directorInstance.ChangeState(TitleDirector.TitleScreenState.SELECT);
        }
    }
}
