using UnityEngine;

public class rotateOBG : MonoBehaviour
{
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y+rotateSpeed, gameObject.transform.localEulerAngles.z);
    }
}
