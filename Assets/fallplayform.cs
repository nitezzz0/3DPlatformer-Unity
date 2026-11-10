using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    public Transform Object;

    private float startingY;

    private bool IsCollidingWithPlayer;

    public float fallTimer;

    private float StartfallTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Object = GetComponent(typeof(Transform)) as Transform;
        startingY = Object.localPosition.y;
        StartfallTimer = fallTimer;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            IsCollidingWithPlayer = true;

        }
    }

    private void Update()
    {
        if (IsCollidingWithPlayer)
        {
            fallTimer = fallTimer - Time.deltaTime;
        }

        if (fallTimer <= 0)
        {
            IsCollidingWithPlayer = false;
            fallTimer = StartfallTimer;
            LeanTween.moveY(gameObject, 5.0f, 1.5f).setEase(LeanTweenType.easeInQuad).setLoopPingPong(1);

        }

    }
}
