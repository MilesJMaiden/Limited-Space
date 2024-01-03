using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the pause menu UI object
    private bool isGamePaused = false;
    private CinemachineLook cinemachineLook; // Reference to the CinemachineLook script

    private void Awake()
    {
        // Ensure the pause menu is not visible when the game starts
        pauseMenuUI.SetActive(false);
        cinemachineLook = FindObjectOfType<CinemachineLook>(); // Find the CinemachineLook script
    }

    private void Update()
    {
        // Check for the pause action
        if (Input.GetKeyDown(KeyCode.Escape)) // or your specific input action
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game time
        isGamePaused = false;
        cinemachineLook.LockCursor(); // Lock the cursor when the game resumes
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game time
        isGamePaused = true;
        cinemachineLook.UnlockCursor(); // Unlock the cursor when the game is paused
    }

    public void QuitGame()
    {
        Application.Quit();
        // If in the editor, use: UnityEditor.EditorApplication.isPlaying = false;
    }

    // Additional methods as needed...
}
