using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControls : MonoBehaviour
{

    private Rigidbody rb;
    public Transform player;

    public float fallMultiplier;

    public float movementSpeed = 5;
    public float jumpForce = 5;

    public Transform groundCheck;
    public LayerMask ground;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 input = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));

        Vector3 MoveDirectiron = (transform.right * input.x) + (transform.forward * input.y);

        rb.linearVelocity = new Vector3(MoveDirectiron.x * movementSpeed, rb.linearVelocity.y, MoveDirectiron.z * movementSpeed);

        if (UnityEngine.Input.GetButton("Jump") && isGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }


    }

    bool isGrounded() => Physics.CheckSphere(groundCheck.position, 1f, ground);

}
