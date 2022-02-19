//==============================================================================================
/// File Name	: BubbleChargeController.cs
/// Summary		: 
//==============================================================================================
using UnityEngine;
using Common;
//==============================================================================================
public class BubbleChargeController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // BubbleGeneratorのオブジェクト
    GameObject bubbleGeneratorObject;
    [SerializeField]
    // Bubbleの生成個数
    int num_bubble;

    BubbleGenerator bubbleG;
    bool atOnce = false;



    //------------------------------------------------------------------------------------------
    // Start
    //------------------------------------------------------------------------------------------
    private void Start()
    {
        bubbleG = bubbleGeneratorObject.GetComponent<BubbleGenerator>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ConstPlayer.NAME && !atOnce)
        {
            atOnce = true;
            // バブルを生成(複数個)
            bubbleG.BubbleCreate(this.transform.position, num_bubble, false);
            Destroy(this.gameObject);
            Vector2 effectSize = Vector2.one * 1.5f;
            EffectGenerator.BubbleBurstFX(new BubbleBurstFX.Param(Color.white, effectSize), transform.position, null);
        }
    }

}
