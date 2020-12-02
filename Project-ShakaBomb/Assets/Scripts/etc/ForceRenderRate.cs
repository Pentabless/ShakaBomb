//==============================================================================================
/// File Name	: ForceRenderRate.cs
/// Summary		: フレームレート設定
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using Common;
//==============================================================================================
public class ForceRenderRate : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    private float m_rate = ConstRenderRate.RATE;
    private float m_currentFrameTime;

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;
        m_currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine("WaitForNextFrame");
    }

    //------------------------------------------------------------------------------------------
    // WaitForNextFrame
    //------------------------------------------------------------------------------------------
    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            m_currentFrameTime += 1.0f / m_rate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = m_currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < m_currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }
}
