using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public GameObject healthUI;
    public GameObject healthUI1;
    public GameObject healthUI2;
    public GameObject healthUI3;
    void Start()
    {
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Damage")
        {
            health =- 1;
        }
    }

    private void Update()
    {
        if (health == 4)
        {
            healthUI.GetComponent<Renderer>().enabled = true;
            healthUI1.GetComponent<Renderer>().enabled = true;
            healthUI2.GetComponent<Renderer>().enabled = true;
            healthUI3.GetComponent<Renderer>().enabled = true;
        }

        if (health == 3)
        {
            healthUI.GetComponent<Renderer>().enabled = false;
            healthUI1.GetComponent<Renderer>().enabled = true;
            healthUI2.GetComponent<Renderer>().enabled = true;
            healthUI3.GetComponent<Renderer>().enabled = true;
        }

        if (health == 2)
        {
            healthUI.GetComponent<Renderer>().enabled = false;
            healthUI1.GetComponent<Renderer>().enabled = false;
            healthUI2.GetComponent<Renderer>().enabled = true;
            healthUI3.GetComponent<Renderer>().enabled = true;
        }

        if (health == 1)
        {
            healthUI.GetComponent<Renderer>().enabled = false;
            healthUI1.GetComponent<Renderer>().enabled = false;
            healthUI2.GetComponent<Renderer>().enabled = false;
            healthUI3.GetComponent<Renderer>().enabled = true;
        }
    }
}
