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

}
