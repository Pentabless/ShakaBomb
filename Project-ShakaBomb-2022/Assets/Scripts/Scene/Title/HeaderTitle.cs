//==============================================================================================
/// File Name	: HeaderTitle.cs
/// Summary		: タイトルシーンのヘッダータイトル
//==============================================================================================
using UnityEngine;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
//==============================================================================================
public class HeaderTitle : MonoBehaviour
{
	//------------------------------------------------------------------------------------------
    // member variable
	//------------------------------------------------------------------------------------------
    [SerializeField]
    private List<TitleObject> _titleObjects = null;

    /// <summary>
    /// ヘッダータイトルの設定
    /// </summary>
    /// <param name="id"></param>
    public void SetHeaderTitle(int id) 
    {
        foreach(var title in _titleObjects)
        {
            if(title.Id == id)
                title.Obj.SetActive(true);
            else
                title.Obj.SetActive(false);
        }
    }
}

[Serializable]
public class TitleObject
{
    /// <summary>
    /// 固有のID
    /// </summary>
    public int Id;

    /// <summary>
    /// タイトルのオブジェクト
    /// </summary>
    public GameObject Obj;
}