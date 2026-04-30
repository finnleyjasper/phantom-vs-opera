
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudienceSupportBarUI : MonoBehaviour
{
    // Private Variables
        [SerializeField] private Image _filling;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Transform _comboBarRoot;
        [SerializeField] private TextMeshProUGUI _comboText;

        private AudienceSupport _audienceSupport;


    void Awake()
    {
        // find ui elements
        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image image in images)
        {
            if (image.name == "Filling")
            {
                _filling = image;
            }
        }

        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name.ToLowerInvariant().Contains("combo"))
            {
                _comboText = label;
            }
            else if (_text == null)
            {
                _text = label;
            }
        }

        if (_comboBarRoot == null)
            _comboBarRoot = _text != null ? _text.transform.parent : transform;

        EnsureComboTextExists();
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
        Debug.Log("Audience support bar is " + audienceSupportFillingAmount);
        UpdateText();
    }

    // Method to Update Audience Bar Text
    private void UpdateText()
    {
        int total = Mathf.RoundToInt(_audienceSupport.AudienceSupportValue);
        _text.text = "Audience Support: " + total;

        if (GameObserver.Instance != null && GameObserver.Instance.IsComboActive)
        {
            int comboPercent = Mathf.RoundToInt(GameObserver.Instance.CurrentComboPercent);
            _comboText.text = "Multiplier\n+" + comboPercent + "%";
        }
        else if (_comboText != null)
        {
            _comboText.text = "Multiplier\n+0%";
        }
    }

    private void EnsureComboTextExists()
    {
        if (_comboText != null || _comboBarRoot == null || _text == null) return;

        GameObject comboGO = new GameObject("Combo Text", typeof(RectTransform));
        comboGO.transform.SetParent(_comboBarRoot, false);
        comboGO.transform.localPosition = _text.transform.localPosition + new Vector3(1.9f, 1.2f, 0f);
        comboGO.transform.localScale = _text.transform.localScale;

        RectTransform comboRect = comboGO.GetComponent<RectTransform>();
        comboRect.sizeDelta = new Vector2(180f, 56f);

        _comboText = comboGO.AddComponent<TextMeshProUGUI>();
        _comboText.font = _text.font;
        _comboText.fontSize = _text.fontSize * 0.95f;
        _comboText.fontStyle = FontStyles.Bold;
        _comboText.alignment = TextAlignmentOptions.Center;
        _comboText.enableWordWrapping = false;
        _comboText.text = "Multiplier\n+0%";
    }
}
