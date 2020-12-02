//==============================================================================================
/// File Name	: Floor.cs 
/// Summary		: 
//==============================================================================================
using UnityEngine;
using Common;
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
    // summary : Awake
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        thisCollider = GetComponent<BoxCollider2D>();
        platform = GetComponent<PlatformEffector2D>();
    }



    //------------------------------------------------------------------------------------------
    // summary : Update
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Update()
    {
        Pass();
    }



    //------------------------------------------------------------------------------------------
    // summary : 通り抜け処理
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    protected void Pass()
    {
        // 下入力時に通り抜けるようにする
        if (Input.GetAxis(ConstPlayer.VERTICAL) < -passSensitivity)
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
