using UnityEngine;

public class RudderFollowTurn : MonoBehaviour
{

    public Transform boat;                  


    public Vector3 rudderAxis = Vector3.up;    
    public float maxAngle = 30f;               
    public float sensitivity = 0.5f;          
    public float smoothTime = 0.15f;                          

    public float turnThreshold = 10f;         

    Quaternion baseLocalRotation;
    Quaternion lastBoatRotation;
    float currentAngle;
    float angleVelocity;

    void Start()
    {
        if (boat == null) boat = transform.parent;
        baseLocalRotation = transform.localRotation;   
        lastBoatRotation = boat.rotation;
    }

    void LateUpdate()
    {
        if (Time.deltaTime <= 0f) return;


        Quaternion delta = boat.rotation * Quaternion.Inverse(lastBoatRotation);
        lastBoatRotation = boat.rotation;

        delta.ToAngleAxis(out float angle, out Vector3 axis);
        angle = Mathf.DeltaAngle(0f, angle);


        float yawRate = angle * Vector3.Dot(axis, boat.up) / Time.deltaTime;

        float target = 0f;
        float excess = Mathf.Abs(yawRate) - turnThreshold;
        if (excess > 0f)
        {
            target = Mathf.Sign(yawRate) * excess * sensitivity;
            target = Mathf.Clamp(target, -maxAngle, maxAngle);
        }

        currentAngle = Mathf.SmoothDamp(currentAngle, target, ref angleVelocity, smoothTime);

  
        transform.localRotation = baseLocalRotation * Quaternion.AngleAxis(currentAngle, rudderAxis);
    }
}