using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DeathScreen : MonoBehaviour
{
    public Death death;

    public string propertyName = "_CloudsPower";

    public float startValue = 3f;
    public float endValue = 0f;
    public float duration = 2.0f;

    private Material instanceMaterial;
    private Coroutine transitionCoroutine;
    private bool hasTriggered = false;

    private void Start()
    {

        instanceMaterial = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {

        if (death.PlayerDead && !hasTriggered)
        {
            hasTriggered = true;
            FadeOutCloudPower();
        }
    }

    public void FadeOutCloudPower()
    {

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(AnimateFloatRoutine(startValue, endValue, duration));
    }

    private IEnumerator AnimateFloatRoutine(float start, float end, float time)
    {
        float elapsed = 0f;
        instanceMaterial.SetFloat(propertyName, start);

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float percentageComplete = elapsed / time;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            instanceMaterial.SetFloat(propertyName, currentValue);

            yield return null;
        }

        instanceMaterial.SetFloat(propertyName, end);
    }

    private void OnDestroy()
    {


     Destroy(instanceMaterial);

    }
}
