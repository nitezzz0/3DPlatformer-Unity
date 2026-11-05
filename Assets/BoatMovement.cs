using UnityEngine;

public class BoatControls : MonoBehaviour
{
    public float movementSpeed = 50f;
    public float waterDrag = 2f;      
    public PlatformCollider targetPlatform;

    private Rigidbody rb;
    private Vector2 input;
    private float startY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startY = rb.position.y;
        rb.useGravity = false;


        rb.linearDamping = waterDrag;
    }

    void Update()
    {
        // Reset input to zero if not colliding, so it drifts to a stop using drag
        if (targetPlatform.canDrive)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            input = Vector2.zero;
        }
    }

    void FixedUpdate()
    {

        Vector3 moveDirection = (transform.right * 0 + transform.forward * input.y).normalized;

        rb.AddForce(moveDirection * movementSpeed, ForceMode.Acceleration);

        Vector3 pos = rb.position;
        pos.y = startY;
        rb.position = pos;
    }
}
