using System.Collections;
using UnityEngine;

public class SmoothShak : MonoBehaviour
{
    public Transform ObjectTransform;

    public PlatformCollider targetPlatform;

    public float shakeAmount = 0.1f;
    public float shakeSpeed = 2.0f;
    public float shakeAmountR = 0.1f;
    public float shakeSpeedR = 2.0f;

    public Transform cameraTransform;
    public float rotationSpeed = 10f;

    Vector3 originalPos;
    Vector3 originalRos;
    private float noiseTimer;
    private float noiseTimerR;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Awake()
    {
        if (ObjectTransform == null)
        {
            ObjectTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {

        originalRos = ObjectTransform.localEulerAngles;

        noiseTimer = Random.Range(0f, 100f);
    }
    void FixedUpdate()
    {
        if (targetPlatform.canDrive)
            {
                noiseTimerR += Time.deltaTime * shakeSpeedR;

          

                float cameraYangle = cameraTransform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0, cameraYangle, 0);
                Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(smoothedRotation);

                float x2 = (Mathf.PerlinNoise(noiseTimerR, 0f) - 0.5f) * shakeAmountR;
                float z2 = (Mathf.PerlinNoise(noiseTimerR, noiseTimerR) - 0.5f) * shakeAmountR;
                ObjectTransform.localEulerAngles = new Vector3(x2, smoothedRotation.eulerAngles.y, z2);
            }

        else
        {
            noiseTimerR += Time.deltaTime * shakeSpeedR;

            float x2 = (Mathf.PerlinNoise(noiseTimerR, 0f) - 0.5f) * shakeAmountR;
            float z2 = (Mathf.PerlinNoise(noiseTimerR, noiseTimerR) - 0.5f) * shakeAmountR;
            ObjectTransform.localEulerAngles = new Vector3(x2, ObjectTransform.eulerAngles.y, z2);
        }
    }
}