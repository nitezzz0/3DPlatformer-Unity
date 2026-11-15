using System.Collections;
using UnityEngine;

public class Death : MonoBehaviour
{
    public LayerMask deathlayer;
    public Transform groundCheck;
    public bool PlayerDead;
    private bool HideCanvas;
    public Canvas UICanvas;
    public CanvasGroup deathCanvasGroup; 
    public float fadeDuration = 1.0f;

    void Start()
    {

        deathCanvasGroup.alpha = 0f;
        deathCanvasGroup.interactable = false;
        deathCanvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (isDead())
        {
            PlayerDead = true;
            
        }

        if (PlayerDead && !HideCanvas)
        {

            UICanvas.enabled = !UICanvas.enabled;
            HideCanvas = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            StartCoroutine(FadeInDeathCanvas());
        }
    }

    IEnumerator FadeInDeathCanvas()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            deathCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
            PlayerDead = false;
        }

        deathCanvasGroup.alpha = 1f;
        deathCanvasGroup.interactable = true;
        deathCanvasGroup.blocksRaycasts = true;
    }

    bool isDead() => Physics.CheckSphere(groundCheck.position, 1f, deathlayer);
}
