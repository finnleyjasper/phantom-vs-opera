using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Start()
    {
        if (GameResult.result == "win")
        {
            resultText.text = "You Win!";
        }
        else if (GameResult.result == "lose")
        {
            resultText.text = "You Lose!";
        }
        else
        {
            resultText.text = "Game Over";
        }
    }
}