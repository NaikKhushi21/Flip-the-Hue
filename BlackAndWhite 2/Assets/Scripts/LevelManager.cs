using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void RestartLevel()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        MetricManager.instance.AddToResets(1);
        SceneManager.LoadScene(currentScene.name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
