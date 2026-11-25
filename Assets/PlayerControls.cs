using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    public Transform player;

    public float Collectables;
    public Text Collectablestxt;

    public float fallMultiplier = 2.5f;
    private bool isFalling;

    public float movementSpeed = 5f;
    public float jumpForce = 5f;

    public float coyoteTime = 0.15f;     
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask ground;
    public LayerMask bounce;

    public Animator anim;
    private Vector2 moveInput;
    private bool isLoadingScene;

    public PlatformCollider platformco;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool grounded = isGrounded();

 
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        isFalling = !grounded && rb.linearVelocity.y < -5f;

        anim.SetBool("Onground", grounded);
        anim.SetBool("Falling", isFalling);
        anim.SetBool("onBoat", platformco.canDrive);

        Collectablestxt.text = Collectables.ToString() + "/3";
        
        if (Collectables >= 3 && !isLoadingScene)
        {
            isLoadingScene = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene("EndScreen");
        }

        
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        rb.linearVelocity = new Vector3(moveDirection.x * movementSpeed, rb.linearVelocity.y, moveDirection.z * movementSpeed);


        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);


            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }


        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1), ForceMode.Acceleration);
        }


        if (isBounce())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 11.2f, rb.linearVelocity.z);
            coyoteTimeCounter = 0f; 
        }
    }

    bool isGrounded() => Physics.CheckSphere(groundCheck.position, groundCheckRadius, ground);
    bool isBounce() => Physics.CheckSphere(groundCheck.position, groundCheckRadius, bounce);
}
