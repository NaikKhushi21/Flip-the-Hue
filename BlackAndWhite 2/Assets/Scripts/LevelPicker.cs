using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        if (MetricManager.instance != null)
        {
            MetricManager.instance.ClearLevel();
        }
        SceneManager.LoadScene(levelName);
    }
}
