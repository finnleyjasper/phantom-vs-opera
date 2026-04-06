using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarUI : MonoBehaviour
{
    // Private Variables 
        private int _maxHealth = 10;
        private int _maxSuccess = 10;

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
        float healthFillingAmount = (float)player.HealthBar / _maxHealth;
        _healthBarFilling.fillAmount = healthFillingAmount; // Setting fill amount value to current health 
        UpdatePlayerBarText();
    }

    // Method for Updating Player Success Bar UI 
    public void UpdatePlayerSuccessUI()
    {
        float successFillingAmount = (float)player.SuccessBar / _maxSuccess; 
        _successBarFilling.fillAmount = successFillingAmount; // Setting fill amount value to current success 
        UpdatePlayerBarText();
    }
}
