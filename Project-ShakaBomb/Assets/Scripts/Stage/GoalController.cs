//==============================================================================================
/// File Name	: GoalController.cs
/// Summary		: ゴール
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class GoalController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    PlayDirector playDirector = null;
    PlayBGM bgm = null;
    private bool once = false;



    //------------------------------------------------------------------------------------------
    // summary : Start
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        bgm = GameObject.Find("PlaySceneBgm").GetComponent<PlayBGM>();
        if(!bgm)
        {
            DebugLogger.Log("BGMをアタッチしてください");
        }
    }



    //------------------------------------------------------------------------------------------
    // summary : ゴールにプレイヤーが触れた
    // remarks : none
    // param   : none
    // return  : none
    //------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (once)
        {
            return;
        }    

        if (col.tag == ConstPlayer.NAME)
        {
            // 通常の状態でない場合はゴール出来ない
            var playerController = col.gameObject.GetComponentInParent<PlayerController>();
            if (playerController.IsDead() || Data.time < 0)
            {
                return;
            }

            playDirector.Goal();
            bgm.GoalEvent();
            once = true;
        }
    }
}
