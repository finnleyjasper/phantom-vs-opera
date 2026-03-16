using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Start()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Win)
        {
            resultText.text = "You Win!";
        }
        else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Win)
        {
            resultText.text = "You Lose!";
        }
        else
        {
            resultText.text = "Game Over";
        }
    }
}
