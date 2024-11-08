using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
