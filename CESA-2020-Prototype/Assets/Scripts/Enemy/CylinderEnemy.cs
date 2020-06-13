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

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        Respawn();  //ガスエネミーを出現させる

        //出現する時間のずれが0じゃなかったら
        if (appearCount != 0.0f)
        {
            //さっき作ったガスエネミーを消しておく
            Destroy(gus.gameObject);
            //出現する時間を少しずらしておく
            count = respawnCount-appearCount;
        }
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {

        if (gus.gameObject == null && !stopRespawn)
        {
            count += Time.deltaTime;

            if (count >= respawnCount)
            {
                Respawn();
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
