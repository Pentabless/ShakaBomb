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
        prefabHolder = GameObject.Find(EffectPrefabHolder.NAME).GetComponent<EffectPrefabHolder>();

        if (!prefabHolder)
        {
            Debug.Log("EffectPrefabHolder.prefabをシーンに追加してください");
        }
    }

    //------------------------------------------------------------------------------------------
    // 泡の破裂エフェクトの生成
    //------------------------------------------------------------------------------------------
    static public GameObject BubbleBurstFX(BubbleBurstFX.Param param, Vector3 pos, Transform parent = null)
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

    //------------------------------------------------------------------------------------------
    // ブースト移動の軌跡エフェクトの生成
    //------------------------------------------------------------------------------------------
    static public GameObject BoostTrailFX(BoostTrailFX.Param param, Vector3 pos, Transform parent = null)
    {
        if (!prefabHolder)
        {
            Init();
        }

        var go = Instantiate(prefabHolder.boostTrailFX);
        go.transform.position = pos;
        go.transform.parent = parent;
        var fx = go.GetComponent<BoostTrailFX>();
        fx.SetParam(param);
        fx.Play();

        return go;
    }

    //------------------------------------------------------------------------------------------
    // クリーンエフェクトの生成
    //------------------------------------------------------------------------------------------
    static public GameObject CleanFX(CleanFX.Param param, Vector3 pos, Transform parent = null)
    {
        if (!prefabHolder)
        {
            Init();
        }

        var go = Instantiate(prefabHolder.cleanFX, pos, Quaternion.identity, parent);
        var fx = go.GetComponent<CleanFX>();
        fx.SetParam(param);
        fx.Play();

        return go;
    }
}
