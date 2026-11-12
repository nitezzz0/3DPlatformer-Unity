using UnityEngine;

public class WorldPositionAngle2D : MonoBehaviour
{
    public Transform objectA; 
    public Transform objectB;

    public float smoothSpeed = 10f;

    void Update()
    {

        Vector3 direction = objectB.position - objectA.position;


        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle+90);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
