using UnityEngine;
using UnityEngine.UIElements;

public class platformMove : MonoBehaviour
{
    [SerializeField] private float targetZS = 10f;
    [SerializeField] private float targetZF = 10.0f;
    [SerializeField] private float speed = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y , targetZS);
    }

    // Update is called once per frame
    void Update()
    {

     LeanTween.moveZ(gameObject, targetZF, speed).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong();

    }
}
