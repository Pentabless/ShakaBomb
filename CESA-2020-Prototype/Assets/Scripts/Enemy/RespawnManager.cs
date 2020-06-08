//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public class RespawnManager : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    CameraController cameraController;

    GameObject enemy;


    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        RespawnEnemy();
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if(enemy == null)
        {
            if (cameraController.RespawnApproval)
                RespawnEnemy();
        }
    }

    private void RespawnEnemy()
    {
        enemy = Instantiate(enemyPrefab.gameObject);
        enemy.transform.position = this.transform.position;
    }
}
