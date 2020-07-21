//==============================================================================================
/// File Name	: BubbleFadeFX.cs
/// Summary		: バブルフェードエフェクト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class BubbleFadeFX : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private ParticleSystem system = null;   // パーティクルシステム
    private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    private int particleCount = 0;

    [SerializeField]
    private GameObject bubbleBurstFX2 = null;   // 破裂エフェクト
    private GameObject fxInstance = null;       // 破裂エフェクトの実体
    private BubbleBurstFX fxController = null;  // 破裂エフェクトのスクリプト

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        system = GetComponent<ParticleSystem>();
        fxInstance = Instantiate(bubbleBurstFX2, new Vector3(-999,0,0), Quaternion.identity, transform);
        fxController = fxInstance.GetComponent<BubbleBurstFX>();
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        if (!system.isPlaying)
        {
            return;
        }

        if (system.particleCount == 0 && particleCount == 0)
        {
            return;
        }
        
        // 前フレームのデータのライフタイムと経過時間を比較して判定する
        for(int i = 0; i < particleCount; i++)
        {
            if (particles[i].remainingLifetime < Time.deltaTime)
            {
                fxController.SetParam(new BubbleBurstFX.Param(particles[i].GetCurrentColor(system), particles[i].GetCurrentSize3D(system)));
                fxController.Emit(system.transform.TransformPoint(particles[i].position));
            }
        }

        // パーティクルデータの保存
        particleCount = system.particleCount;
        system.GetParticles(particles);
    }

    //------------------------------------------------------------------------------------------
    // エフェクトの再生
    //------------------------------------------------------------------------------------------
    public void Play()
    {
        system.Play();
    }

}
