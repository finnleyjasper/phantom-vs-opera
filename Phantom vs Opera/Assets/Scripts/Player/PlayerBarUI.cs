using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarUI : MonoBehaviour
{
    // Private Variables 
        [Header("Audience Support Bar Filling")]
        [SerializeField] private Image _audienceSupportFilling;
        
        [Header("Audience Support Text")]
        [SerializeField] private TextMeshProUGUI _audienceSupportBarText;

        [SerializeField] private AudienceSupport audiencesupport;

    // Audience Bar Text
    private void Start()
    {
        UpdatePlayerBarText();
    }

    // Method to Update Health/Success Text
    private void UpdatePlayerBarText()
    {
        _audienceSupportBarText.text = "Audience Support: " + audiencesupport.AudienceSupportValue;
    }

    // Method for Updating Player Health Bar UI 
    public void UpdateAudienceSupportUI()
    {
        float audienceSupportFillingAmount = (float)audiencesupport.AudienceSupportValue / audiencesupport.AudienceSupportMax;
        _audienceSupportFilling.fillAmount = audienceSupportFillingAmount; // Setting fill amount value to current health 
        UpdatePlayerBarText();
    }
}
