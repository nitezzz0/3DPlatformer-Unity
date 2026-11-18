using UnityEngine;
using UnityEngine.SceneManagement;

public class MianMenuManager : MonoBehaviour
{
    public string gameScene = "SampleScene";

    private float playb = 0;

    private bool Settings;

    public Canvas SettingsCanva;

    public void PlayGame()
    {

        playb = 1;
    }

    public void QuitGame()
    {
        playb = 2;
    }

    public void SettingsSet()
    {
        Settings = !Settings;
    }

    private void Update()
    {
        if (playb == 2)
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }

        if (playb == 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameScene);
        }

        if (Settings)
        {
            SettingsCanva.enabled = true;
        }

        else
        {
            SettingsCanva.enabled = false;
        }
            
    }
}
