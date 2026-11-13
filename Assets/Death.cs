using UnityEngine;

public class Death : MonoBehaviour
{

    public LayerMask death;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == death)
        {


        }

    }
}

