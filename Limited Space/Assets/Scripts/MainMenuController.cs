using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Sandbox"); 
    }

    public void OpenControls()
    {
        // Logic to open controls menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}