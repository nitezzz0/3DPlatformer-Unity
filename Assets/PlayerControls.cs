using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControls : MonoBehaviour
{

    private Rigidbody rb;
    public Transform orientation;
    public Transform player;
    public Transform playerOBJ;
    public float rotationSpeed;

    public float movementSpeed = 5;
    public float jumpForce = 5;

    public Transform groundCheck;
    public LayerMask ground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = UnityEngine.Input.GetAxisRaw("Horizontal");
        float VerticalInput = UnityEngine.Input.GetAxisRaw("Vertical");

        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            playerOBJ.forward = Vector3.Slerp(playerOBJ.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        rb.linearVelocity = new Vector3(horizontalInput * movementSpeed, rb.linearVelocity.y, VerticalInput * movementSpeed);

        if (UnityEngine.Input.GetButton("Jump") && isGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

    }

    bool isGrounded() => Physics.CheckSphere(groundCheck.position, 1f, ground);

}
