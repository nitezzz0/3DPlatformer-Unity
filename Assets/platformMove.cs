using System.Collections;
using UnityEngine;

public class SmoothShake : MonoBehaviour
{
    public Transform ObjectTransform;

    [Header("Floating Settings")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 2.0f;

    Vector3 originalPos;
    private float noiseTimer;

    void Awake()
    {
        if (ObjectTransform == null)
        {
            ObjectTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = ObjectTransform.localPosition;

        noiseTimer = Random.Range(0f, 100f);
    }

    void Update()
    {

        noiseTimer += Time.deltaTime * shakeSpeed;

        // Generate smooth, non-stop pseudo-random offsets using Perlin Noise
        float x = (Mathf.PerlinNoise(noiseTimer, 0f) - 0.5f) * shakeAmount;
        float y = (Mathf.PerlinNoise(0f, noiseTimer) - 0.5f) * shakeAmount;
        float z = (Mathf.PerlinNoise(noiseTimer, noiseTimer) - 0.5f) * shakeAmount;


        ObjectTransform.localPosition = originalPos + new Vector3(x, y, z);
    }


    public void MakeShake()
    {

    }
}