using UnityEngine;

public class Collection : MonoBehaviour
{
    public PlayerControls collectablesV;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectablesV.Collectables += 1;
            gameObject.SetActive(false);
        }
    }
}
