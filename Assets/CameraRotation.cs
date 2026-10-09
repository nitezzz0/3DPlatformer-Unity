using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform cameraTransform;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform != null)
        {
            float cameraYangle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, cameraYangle, 0);
            rb.MoveRotation(targetRotation);
        }
    }
}
