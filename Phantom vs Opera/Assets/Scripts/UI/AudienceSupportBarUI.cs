using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudienceSupportBarUI : MonoBehaviour
{
    // Private Variables
        [SerializeField] private Image _filling;

        [SerializeField] private TextMeshProUGUI _text;

        private AudienceSupport _audienceSupport;


    void Awake()
    {
        // find ui elements
        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image image in images)
        {
            if (image.name == "Background")
            {
                _filling = image;
            }
            else if (image.name == "Filling")
            {
                _filling = image;
            }
        }

        _text = GetComponentInChildren<TextMeshProUGUI>(true);
        _audienceSupport = FindFirstObjectByType<AudienceSupport>();
    }

    private void Start()
    {
        UpdateText();
    }

    void Update()
    {
        float audienceSupportFillingAmount = (float)_audienceSupport.AudienceSupportValue / GameManager.Instance.MaxAudienceSupport;
        _filling.fillAmount = audienceSupportFillingAmount; // Setting fill amount value to current audience support value
        UpdateText();
    }

    // Method to Update Audience Bar Text
    private void UpdateText()
    {
        int total = Mathf.RoundToInt(_audienceSupport.AudienceSupportValue);
        _text.text = "Audience Support: " + total;
    }

}
