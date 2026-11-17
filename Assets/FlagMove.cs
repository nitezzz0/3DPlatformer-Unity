using UnityEngine;

public class FlagMove : MonoBehaviour
{
    private Vector3 originalPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (transform.position.y == originalPos.y)
            {
                transform.position = new Vector3(transform.position.x, originalPos.y + 0.7f, transform.position.z);
                GetComponent<Collider>().enabled = false;

            }
        }
    }

}
