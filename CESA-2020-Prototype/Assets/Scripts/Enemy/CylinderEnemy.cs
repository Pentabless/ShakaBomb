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

    GameObject gus;
    float count;
    bool stopRespawn = false;

	//------------------------------------------------------------------------------------------
    // Awake
	//------------------------------------------------------------------------------------------
    private void Awake()
    {
        Respawn();
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == Player.NAME)
        {
            stopRespawn = true;
            count = Common.Decimal.ZERO;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == Player.NAME)
        {
            stopRespawn = false;
        }
    }
}
