using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class BoatControls : MonoBehaviour
{

    private Rigidbody rb;
    public Transform player;

    public float movementSpeed = 5;

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

    }

}
