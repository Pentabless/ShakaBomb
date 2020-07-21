//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using UnityEngine.UI;
//==============================================================================================
public partial class Floor : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    
    // 最初・最後のポジション
    [SerializeField]
    protected Vector3 startPosition;
    [SerializeField]
    protected Vector3 endPosition;

    [SerializeField]
    private LayerMask passLayerMask = 0;
    private bool passable = false;
    [SerializeField]
    private float passSensitivity = 0.2f;
    private RaycastHit2D[] results = new RaycastHit2D[10];

    protected GameObject thisObj;
    protected BoxCollider2D thisCollider;
    protected PlatformEffector2D platform;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        thisCollider = GetComponent<BoxCollider2D>();
        platform = GetComponent<PlatformEffector2D>();
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
        Pass();
    }

    //------------------------------------------------------------------------------------------
    // 通り抜け処理
    //------------------------------------------------------------------------------------------
    protected void Pass()
    {
        // 下入力時に通り抜けるようにする
        if (Input.GetAxis(Player.VERTICAL) < -passSensitivity)
        {
            passable = true;
            platform.colliderMask &= ~passLayerMask.value;
        }
        
        // 通り抜け中か判定する
        if (passable)
        {
            int hitCount = Physics2D.BoxCastNonAlloc(thisCollider.bounds.center, thisCollider.bounds.size,
                0, Vector2.down, results, 0, passLayerMask.value);

            for(int i = 0; i < hitCount; i++)
            {
                if (results[i].collider.isTrigger && results[i].collider.tag == "Player")
                {
                    continue;
                }
                platform.colliderMask &= ~passLayerMask.value;
                return;
            }
            passable = false;
        }
        else
        {
            platform.colliderMask |= passLayerMask.value;
        }
        
        
    }
}
