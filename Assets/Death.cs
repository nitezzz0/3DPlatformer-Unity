using System.Collections;
using UnityEngine;

public class Death : MonoBehaviour
{
    public LayerMask deathlayer;
    public Transform groundCheck;
    public bool PlayerDead;
    private bool HideCanvas;
    public Canvas UICanvas;
    public GameObject deathCanvas; 
    public float fadeDuration = 1.0f;


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
            yield return null;
            PlayerDead = false;
        }

    }

    bool isDead() => Physics.CheckSphere(groundCheck.position, 1f, deathlayer);
}
