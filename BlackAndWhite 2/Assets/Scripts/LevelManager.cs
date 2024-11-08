using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // This method ensures the LevelManager persists across scenes
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        // Escape key will quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    // Restart the current level
    public void RestartLevel()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        if (MetricManager.instance != null)
        {
            MetricManager.instance.AddToResets(1);
        }
        SceneManager.LoadScene(currentScene.name);
    }

    // Quit the application
    public void Quit()
    {
        Application.Quit();
    }

    // Method to be called when the player hits the trap (or any other event that triggers a restart)
    public void ReloadLevel()
    {
        // Reload the current scene
        RestartLevel();
    }
}
