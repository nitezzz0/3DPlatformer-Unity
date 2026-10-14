using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.UI;
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
            healthUI.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI1.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI2.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI3.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

        if (health == 3)
        {
            healthUI.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI1.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI2.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI3.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

        if (health == 2)
        {
            healthUI.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI1.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI2.GetComponent<UnityEngine.UI.Image>().enabled = true;
            healthUI3.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

        if (health == 1)
        {
            healthUI.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI1.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI2.GetComponent<UnityEngine.UI.Image>().enabled = false;
            healthUI3.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
    }
}
