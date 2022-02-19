//==============================================================================================
/// File Name	: EffectPrefabHolder.cs
/// Summary		: エフェクトプレハブ保持用オブジェクト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class EffectPrefabHolder : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    public const string NAME = "EffectPrefabHolder";

    // 泡の破裂エフェクト
    [SerializeField]
    private GameObject BubbleBurstFX = null;
    public GameObject bubbleBurstFX { get { return BubbleBurstFX; } private set { BubbleBurstFX = value; } }

    // ブースト移動の軌跡エフェクト
    [SerializeField]
    private GameObject BoostTrailFX = null;
    public GameObject boostTrailFX { get { return BoostTrailFX; } private set { BoostTrailFX = value; } }

    // クリーンエフェクト
    [SerializeField]
    private GameObject CleanFX = null;
    public GameObject cleanFX { get { return CleanFX; } private set { CleanFX = value; } }

}
