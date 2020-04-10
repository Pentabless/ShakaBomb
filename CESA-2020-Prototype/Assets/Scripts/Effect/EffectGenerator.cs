//==============================================================================================
/// File Name	: EffectGenerator.cs
/// Summary		: エフェクト生成用クラス
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Common;
//==============================================================================================
public class EffectGenerator : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    // エフェクトプレハブ保持用オブジェクト
    static EffectPrefabHolder prefabHolder = null;
    

	//------------------------------------------------------------------------------------------
    // 初期化
	//------------------------------------------------------------------------------------------
    static private void Init()
    {
<<<<<<< HEAD
#if UNITY_EDITOR
        bubbleBurstFX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BubbleBurstFX.prefab");
#endif
=======
        prefabHolder = GameObject.Find(EffectPrefabHolder.NAME).GetComponent<EffectPrefabHolder>();
>>>>>>> origin/EffectPrefabHolder
    }

    //------------------------------------------------------------------------------------------
    // 泡の破裂エフェクトの生成
    //------------------------------------------------------------------------------------------
    static public GameObject BubbleBurstFX(BubbleBurstFX.Param param, in Vector3 pos, Transform parent = null)
    {
        if (!prefabHolder)
        {
            Init();
        }
        
        var go = Instantiate(prefabHolder.bubbleBurstFX, pos, Quaternion.identity, parent);
        var fx = go.GetComponent<BubbleBurstFX>();
        fx.SetParam(param);
        fx.Play();

        return go;
    }

}
