using UnityEngine;
using UnityEngine.UI;

public class PressEVisibility : MonoBehaviour
{
    private Canvas canvas;
    public GameObject Player;
    private Rigidbody rb;
    public Transform groundCheck;
    public LayerMask Boat;
    public Text text;
    private bool changeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = Player.GetComponent<Rigidbody>();
        canvas = GetComponent<Canvas>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded())
        {
            canvas.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                changeText = !changeText;
            }
        }

        if (isGrounded() == false ) 
        {
            canvas.enabled = false;
        }

        if (changeText)
        {
            text.text = "To leave the boat";
        }

        else
        {
            text.text = "To sail the boat";
        }
    }

    bool isGrounded() => Physics.CheckSphere(groundCheck.position, 1f, Boat);
}
