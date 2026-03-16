using UnityEngine;
using UnityEngine.SceneManagement;

public class DEBUGPlaySceneManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string endSceneName;

    void Update()
    {
        // Debug win key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Debug: Player Wins");
            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }

        // Debug lose key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Debug: Player Loses");
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }
}
