using Unity.Cinemachine;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{
    private bool IsCollidingWithTarget;

    public bool canDrive;

    public GameObject PlayerPositionObject;

    public CinemachineCamera virtualCamera;

    public Transform Boatcamera;

    public GameObject player;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            IsCollidingWithTarget = true;

        }
    }

    // Update is called once per frame
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            IsCollidingWithTarget = false;

        }

    }

    void Update()
    {

        if (IsCollidingWithTarget && Input.GetKeyDown(KeyCode.E))
        {
            canDrive = !canDrive;
        }

        if (canDrive)
        { 
            player.gameObject.transform.SetParent(transform);
            virtualCamera.Follow = Boatcamera;
            player.gameObject.GetComponent<PlayerControls>().enabled = false;
            player.transform.position = new Vector3(PlayerPositionObject.transform.position.x, player.transform.position.y, PlayerPositionObject.transform.position.z);
        }

        else
        {
            player.gameObject.transform.SetParent(null);
            player.gameObject.GetComponent<PlayerControls>().enabled = true;
            virtualCamera.Follow = player.transform;
        }

    }
}
