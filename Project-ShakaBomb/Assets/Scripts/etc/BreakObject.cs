using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(this.gameObject);
            GenerateBurstEffect();
        }
    }

    private void GenerateBurstEffect()
    {
        EffectGenerator.BubbleBurstFX(
            new BubbleBurstFX.Param(GetComponent<SpriteRenderer>().color, transform.localScale),
            transform.localPosition,
            null);
    }
}
