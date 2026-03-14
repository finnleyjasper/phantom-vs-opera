using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string endSceneName;

    void Update()
    {
        // Debug win key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Debug: Player Wins");
            GoToEndScreen("win");
        }

        // Debug lose key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Debug: Player Loses");
            GoToEndScreen("lose");
        }
    }

    public void GoToEndScreen(string result)
    {
        Debug.Log("Game Ended: " + result);

        GameResult.result = result;

        SceneManager.LoadScene(endSceneName);
    }
}