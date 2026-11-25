using UnityEngine;

public class Collection : MonoBehaviour
{
    public AudioClip collectSound;
    public PlayerControls collectablesV;
    public GameObject UIMarker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {


            collectablesV.Collectables += 1;
            UIMarker.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
