using UnityEngine;

public class savePos : MonoBehaviour
{
    public Transform groundCheck;

    public Death death;

    public LayerMask CheckPointlayer;

    public GameObject Player;

    public GameObject CheckPoint;

    private Vector3 Playerpos;

    public GameObject flag;

    private Transform flagOriginal;

    private float zVelocity = 0f;


    void Start()
    {
        flagOriginal = flag.transform;
    }

    
    bool playerCollided() => Physics.CheckSphere(groundCheck.position, 1f, CheckPointlayer);

    private void Update()
    {
        if (playerCollided())
        {
            Playerpos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);

        
        }

        if (death.PlayerDead)
        {
            Player.transform.position = Playerpos;

        }

    }

}
