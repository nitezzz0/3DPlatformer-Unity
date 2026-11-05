using System.Collections;
using UnityEngine;

public class SmoothShak : MonoBehaviour
{
    public Transform ObjectTransform;

    [Header("Floating Settings")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 2.0f;
    public float shakeAmountR = 0.1f;
    public float shakeSpeedR = 2.0f;

    Vector3 originalPos;
    Vector3 originalRos;
    private float noiseTimer;
    private float noiseTimerR;

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
        originalRos = ObjectTransform.localEulerAngles;

        noiseTimer = Random.Range(0f, 100f);
    }

    void Update()
    {

        noiseTimer += Time.deltaTime * shakeSpeed;
        noiseTimerR += Time.deltaTime * shakeSpeedR;

        // Generate smooth, non-stop pseudo-random offsets using Perlin Noise
        float x = (Mathf.PerlinNoise(noiseTimer, 0f) - 0.5f) * shakeAmount;
        float y = (Mathf.PerlinNoise(0f, noiseTimer) - 0.5f) * shakeAmount;
        float z = (Mathf.PerlinNoise(noiseTimer, noiseTimer) - 0.5f) * shakeAmount;

        float x2 = (Mathf.PerlinNoise(noiseTimerR, 0f) - 0.5f) * shakeAmountR;
        float y2 = (Mathf.PerlinNoise(0f, noiseTimerR) - 0.5f) * shakeAmountR;
        float z2 = (Mathf.PerlinNoise(noiseTimerR, noiseTimerR) - 0.5f) * shakeAmountR;


        ObjectTransform.localPosition = originalPos + new Vector3(x, y, z);
        ObjectTransform.localEulerAngles = originalRos + new Vector3(x2, y2, z2);
    }


    public void MakeShake()
    {

    }
}