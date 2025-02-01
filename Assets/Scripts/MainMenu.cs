using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scene1"); // Replace with your game scene name
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}