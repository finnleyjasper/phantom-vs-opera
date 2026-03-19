using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    [Header("Game State")]
    public GameManager.GameState setGameState;

    public void LoadScene()
    {
        // Update GameManager state if instance exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(setGameState);
        }
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene to load");
        }
    }
}