using UnityEngine;

public class Collection : MonoBehaviour
{
    public PlayerControls collectablesV;
    public GameObject UIMarker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectablesV.Collectables += 1;
            gameObject.SetActive(false);
            UIMarker.SetActive(false);
        }
    }
}
