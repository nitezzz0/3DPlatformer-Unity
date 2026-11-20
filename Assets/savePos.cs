using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject DeathCanvas;

    public float playb = 1;

    public string gameScene = "MainMenu";


    void Start()
    {
        flagOriginal = flag.transform;
        DeathCanvas.SetActive(false);
    }

    
    bool playerCollided() => Physics.CheckSphere(groundCheck.position, 1f, CheckPointlayer);

    private void Update()
    {
        if (playerCollided())
        {
            Playerpos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);


        }

        if (death.PlayerDead && Player.transform.position != Playerpos)
        {

            Player.transform.position = Playerpos;
            DeathCanvas.SetActive(true);


        }

        if (playb == 2)
        {
            SceneManager.LoadScene(gameScene);

        }

        if (playb == 1)
        {
            DeathCanvas.SetActive(false);
        }




    }

    public void retry()
    {

        playb = 1;

    }

    public void Exit()
    {
        playb = 2;

    }




}
