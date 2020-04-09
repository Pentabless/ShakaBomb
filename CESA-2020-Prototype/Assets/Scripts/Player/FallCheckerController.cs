using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCheckerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        //床だったら
        if (collision.tag == "Ground")
        {
            transform.GetComponentInParent<EnemyController>().SetFloorChecker(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //床だったら
        if (collision.tag == "Ground")
        {
            transform.GetComponentInParent<EnemyController>().SetFloorChecker(false);
        }
    }

}
