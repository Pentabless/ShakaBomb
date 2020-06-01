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
    protected float speed;

    [SerializeField]
    private float radius;
    [SerializeField]
    private bool rightOrLeft;
    [SerializeField]
    private float second;
    [SerializeField]
    FloorStatus floorStatus;

    [SerializeField]
    private LayerMask passLayerMask = 0;
    private bool passable = false;
    [SerializeField]
    private float passSensitivity = 0.2f;
    private RaycastHit2D[] result = new RaycastHit2D[10];

    protected GameObject thisObj;
    protected BoxCollider2D thisCollider;
    protected PlatformEffector2D platform;


    private Floor currentObj;
    private NormalFloor normalFloor;
    private MoveFloor moveFloor;
    private RotationFloor rotationFloor;
    private CircleRotationFloor circleRotationFloor;
    private GenerateFloor generateFloor;
    private RideOnFloor rideOnFloor;
    private FallFloor fallFloor;
    private FloatFloor floatFloor;

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
        thisObj = this.gameObject;

        switch (floorStatus)
        {
            case FloorStatus.Normal:
                normalFloor = new NormalFloor();
                currentObj = normalFloor;
                break;
            case FloorStatus.Move:
                moveFloor = new MoveFloor(this.gameObject, startPosition, endPosition);
                moveFloor.speed = speed;
                currentObj = moveFloor;
                break;
            case FloorStatus.RidoOn:
                rideOnFloor = new RideOnFloor(this.gameObject, startPosition, endPosition,second);
                currentObj = rideOnFloor;
                break;
            case FloorStatus.CircleRotation:
                circleRotationFloor = new CircleRotationFloor(this.gameObject, radius, speed, rightOrLeft);
                currentObj = circleRotationFloor;
                break;
            case FloorStatus.Fall:
                fallFloor = new FallFloor(this.gameObject, startPosition, endPosition,second);
                currentObj = fallFloor;
                break;
            case FloorStatus.Rotation:
                rotationFloor = new RotationFloor(this.gameObject, second);
                currentObj = rotationFloor;
                break;
            case FloorStatus.Generate:
                generateFloor = new GenerateFloor(this.gameObject,(int)second);
                generateFloor.ActiveFlag = true;
                currentObj = generateFloor;
                break;
            case FloorStatus.FloatFloor:
                floatFloor = new FloatFloor(this.gameObject,startPosition,endPosition, second);
                currentObj = floatFloor;
                break;
            default:
                break;
        }
    }

    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        currentObj.Execute();

        Pass();
    }

    //------------------------------------------------------------------------------------------
    // 通り抜け処理
    //------------------------------------------------------------------------------------------
    private void Pass()
    {
        // めり込み対策
        if (passable)
        {
            int hitCount = Physics2D.BoxCastNonAlloc(thisCollider.bounds.center, thisCollider.bounds.size,
                0, Vector2.down, result, 0, passLayerMask.value);
            if (hitCount > 0)
            {
                platform.colliderMask &= ~passLayerMask.value;
                return;
            }
            passable = false;
        }

        // 下入力時に通り抜けるする
        if (Input.GetAxis(Player.VERTICAL) < -passSensitivity)
        {
            platform.colliderMask &= ~passLayerMask.value;
        }
        else
        {
            platform.colliderMask |= passLayerMask.value;
        }
        
    }
}
