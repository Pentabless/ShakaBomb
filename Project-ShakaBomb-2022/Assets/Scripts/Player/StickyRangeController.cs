using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyRangeController : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //本体でない別の泡だったら
        if (collision.tag == "Sticky" && collision != GetComponentInParent<BubbleController>())
        {
            GetComponentInParent<BubbleController>().StickyTriggerEnter(collision);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        //本体でない別の泡だったら
        if (collision.tag == "Sticky" && collision != GetComponentInParent<BubbleController>())
        {
            GetComponentInParent<BubbleController>().StickyTriggerExit(collision);
        }
    }
}
