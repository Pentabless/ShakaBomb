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
public class CylinderEnemy : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    GameObject gusPre;
    [SerializeField]
    float moveDistance;
    [SerializeField]
    float respawnCount;
    [SerializeField]
    float appearCount;
    [SerializeField]
    float destroyCount;

    GameObject gus;
    float count;
    bool stopRespawn = false;

    private CameraController cameraController = null;
    [SerializeField]
    AudioClip spawnSE = null;
    private float spawnDelay = 1.5f;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        //Respawn();  //ガスエネミーを出現させる

        ////出現する時間のずれが0じゃなかったら
        //if (appearCount != 0.0f)
        //{
        //    //さっき作ったガスエネミーを消しておく
        //    Destroy(gus.gameObject);
        //    //出現する時間を少しずらしておく
        //    count = respawnCount-appearCount;
        //}

        //出現する時間を少しずらしておく
        count = respawnCount - appearCount;
    }

    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        cameraController = GameObject.Find(Common.Camera.CONTROLLER).GetComponent<CameraController>();
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        if (spawnDelay > 0)
        {
            spawnDelay -= Time.deltaTime;
            return;
        }

        if (gus == null && !stopRespawn)
        {
            count += Time.deltaTime;

            if (count >= respawnCount)
            {
                var topLeft = cameraController.GetScreenTopLeft();
                var bottomRight = cameraController.GetScreenBottomRight();
                if (topLeft.x < transform.position.x && transform.position.x < bottomRight.x &&
                    topLeft.y > transform.position.y && transform.position.y > bottomRight.y)
                {
                    Respawn();
                }
                count = Common.Decimal.ZERO;
            }
        }
    }

    private void Respawn()
    {
        gus = GameObject.Instantiate(gusPre.gameObject);
        gus.transform.position = this.transform.position;
        gus.GetComponent<GusEnemy>().arrivalPosition = new Vector3(gus.transform.position.x,
                                          gus.transform.position.y + moveDistance,
                                          gus.transform.position.z);
        gus.GetComponent<GusEnemy>().destroy_count = destroyCount;
        
        SoundPlayer.Play(spawnSE, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Player.NAME)
        {
            stopRespawn = true;
            count = Common.Decimal.ZERO;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Player.NAME)
        {
            stopRespawn = false;
        }
    }
}
