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
    // 泡の破裂エフェクト
    static GameObject bubbleBurstFX;

	//------------------------------------------------------------------------------------------
    // 初期化
	//------------------------------------------------------------------------------------------
    static private void Init()
    {
#if UNITY_EDITOR
        bubbleBurstFX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BubbleBurstFX.prefab");
#endif
    }

    //------------------------------------------------------------------------------------------
    // 泡の破裂エフェクトの生成
    //------------------------------------------------------------------------------------------
    static public GameObject BubbleBurstFX(BubbleBurstFX.Param param, in Vector3 pos, Transform parent)
    {
        if (!bubbleBurstFX)
        {
            Init();
        }
        
        var go = Instantiate(bubbleBurstFX, pos, Quaternion.identity, parent);
        var fx = go.GetComponent<BubbleBurstFX>();
        fx.SetParam(param);
        fx.Play();

        return go;
    }

}
