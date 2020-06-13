using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // カメラの揺れている量
    public Vector3 shakeMove { private set; get; } = Vector3.zero;
    [SerializeField]
    // カメラの揺れる間隔
    private float shakeDelay = 1 / 60.0f;

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeMove = new Vector3(x, y, 0);

            elapsed += shakeDelay;

            yield return new WaitForSeconds(shakeDelay);
        }

        shakeMove = Vector3.zero;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }
}
