using UnityEngine;
using UnityEngine.UIElements;

public class followplayer : MonoBehaviour
{
    public Transform Player;
    public Transform Dome;

    // Update is called once per frame
    void Update()
    {
        Dome.transform.position = new Vector3(Player.transform.position.x, Dome.transform.position.y , Player.transform.position.z);
    }
}
