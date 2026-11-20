using System.Collections;
using UnityEngine;

public class Death : MonoBehaviour
{
    public savePos savepos;
    public LayerMask deathlayer;
    public Transform groundCheck;
    public bool PlayerDead;

    public GameObject UICanvas;
    public GameObject deathCanvas;

    void Update()
    {
        // 1. Trigger death
        if (!PlayerDead && isDead())
        {
            PlayerDead = true;
            savepos.playb = 0;

            UICanvas.SetActive(false);
            if (deathCanvas != null) deathCanvas.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        if (PlayerDead && savepos.playb == 1)
        {
            PlayerDead = false;

            UICanvas.SetActive(true);
            if (deathCanvas != null) deathCanvas.SetActive(false);

            StartCoroutine(LockCursorNextFrame());
        }
    }

    IEnumerator LockCursorNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool isDead() => Physics.CheckSphere(groundCheck.position, 1f, deathlayer);
}
