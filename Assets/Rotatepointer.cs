using UnityEngine;

public class Rotatepointer : MonoBehaviour
{
    public Transform Camera;
    public float smoothSpeed = 10f; 

    void Update()
    {

     Quaternion targetRotation = Quaternion.Euler(0, 0, -Camera.eulerAngles.y);

     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

    }
}
