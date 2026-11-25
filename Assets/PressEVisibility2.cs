using UnityEngine;
using UnityEngine.UI;

public class PressEVisibility2 : MonoBehaviour
{
    private Canvas canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        canvas = GetComponent<Canvas>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            {
                canvas.enabled = false;
            }
        }
        

}
