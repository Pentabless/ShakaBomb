//==============================================================================================
/// File Name	: TitleScreen.cs
/// Summary		: タイトルスクリーン
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class TitleScreen : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
	//------------------------------------------------------------------------------------------

    [SerializeField]
    private TitleView _titleView = null;

    private void Update() 
    {
        // if (Input.GetKeyDown(KeyCode.Z)) 
        // {
        //     _titleView.DataMenu.SetActive(true);
        // }
        // if(Input.GetKeyDown(KeyCode.X))
        // {
        //     _titleView.Footer.SetActive(true);
        // }
        // if(Input.GetKeyDown(KeyCode.C))
        // {
        //     _titleView.HeaderTitle.transform.parent.gameObject.SetActive(true);
        // }
    }
}
