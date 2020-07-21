//==============================================================================================
/// File Name	: SingletonMonoBehaviour.cs
/// Summary		: Sceneに1つしか存在できないGameObjectを作成
//==============================================================================================
/// Usage       : SingletonにしたいScriptに継承させる
//==============================================================================================
using System;
using UnityEngine;
//==============================================================================================
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    private bool dontDestroyOnLoad = false;
    private static T instance;


 
    //------------------------------------------------------------------------------------------
    // Instance
    //------------------------------------------------------------------------------------------
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません。");
                }
            }

            return instance;
        }
    }



    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる
        // アタッチされている場合は破棄する
        if (this != Instance)
        {
            Destroy(this);

            Debug.LogError(
                typeof(T) +
                " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました。" +
                " アタッチされているGameObjectは " + Instance.gameObject.name + " です。");
            return;
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
