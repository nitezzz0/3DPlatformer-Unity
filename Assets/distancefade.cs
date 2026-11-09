using UnityEngine;

public class DistanceFade : MonoBehaviour
{
    [Header("Target References")]
    [SerializeField] private Transform playerCamera;

    [Header("Distance Settings")]
    [SerializeField] private float fadeStartDistance = 10f;
    [SerializeField] private float fadeEndDistance = 20f;

    [Header("View Cone (Reaper) Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float viewFadeThreshold = 0.7f;

    private Material targetMaterial;
    private int colorId;

    void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError($"No Renderer found on {gameObject.name}. Script disabled.", this);
            enabled = false;
            return;
        }

        targetMaterial = rend.material;

        // Pick whichever color property this shader actually uses
        if (targetMaterial.HasProperty("_BaseColor"))
            colorId = Shader.PropertyToID("_BaseColor");   // URP / HDRP
        else if (targetMaterial.HasProperty("_Color"))
            colorId = Shader.PropertyToID("_Color");       // Built-in Standard
        else
        {
            Debug.LogError($"{targetMaterial.shader.name} has no _BaseColor or _Color property.", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        float dist = Vector3.Distance(playerCamera.position, transform.position);
        float range = Mathf.Max(0.0001f, fadeEndDistance - fadeStartDistance);
        float distanceAlpha = Mathf.Clamp01(1f - (dist - fadeStartDistance) / range);

        Vector3 dir = (transform.position - playerCamera.position).normalized;
        float dot = Vector3.Dot(playerCamera.forward, dir);

        float viewAlpha = 1f;
        if (dot > viewFadeThreshold)
            viewAlpha = Mathf.Clamp01((1f - dot) / (1f - viewFadeThreshold));

        Color c = targetMaterial.GetColor(colorId);
        c.a = distanceAlpha * viewAlpha;
        targetMaterial.SetColor(colorId, c);
    }

    void OnDestroy()
    {
        if (targetMaterial != null) Destroy(targetMaterial);
    }
}