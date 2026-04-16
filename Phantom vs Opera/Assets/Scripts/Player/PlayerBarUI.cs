using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarUI : MonoBehaviour
{
        [Header("Health Bar Filling")]
        [SerializeField] private Image _healthBarFilling;

        [Header("Success Bar Filling")]
        [SerializeField] private Image _successBarFilling;
        
        [Header("Health Bar Text")]
        [SerializeField] private TextMeshProUGUI _healthBarText;

        [Header("Success Bar Text")]
        [SerializeField] private TextMeshProUGUI _successBarText;

    public Player player;

    // Initializing Health/Success Text 
    void Start() 
    {
        UpdatePlayerBarText();
    }

    // Method to Update Health/Success Text
    private void UpdatePlayerBarText()
    {
        _healthBarText.text = "Health: " + player.HealthBar;
        _successBarText.text = "Success: " + player.SuccessBar;
    }

    // Method for Updating Player Health Bar UI 
    public void UpdatePlayerHealthUI()
    {
        float healthFillingAmount = player.MaxHealth > 0 ? (float)player.HealthBar / player.MaxHealth : 0f;
        _healthBarFilling.fillAmount = healthFillingAmount; // Setting fill amount value to current health 
        UpdatePlayerBarText();
    }

    // Method for Updating Player Success Bar UI 
    public void UpdatePlayerSuccessUI()
    {
        float successFillingAmount = player.MaxSuccessMeter > 0f ? player.SuccessBar / player.MaxSuccessMeter : 0f; 
        _successBarFilling.fillAmount = successFillingAmount; // Setting fill amount value to current success 
        UpdatePlayerBarText();
    }
}
