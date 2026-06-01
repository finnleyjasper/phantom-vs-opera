using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ScoreComboHudUI : MonoBehaviour
{
    [SerializeField] private float _topOffset = 20f;
    [SerializeField] private float _fontSize = 34f;
    [SerializeField] private float _lineSpacing = 38f;

    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _comboText;
    private int _lastScore = int.MinValue;
    private int _lastCombo = int.MinValue;

    private void Awake()
    {
        BuildHud();
    }

    private void Update()
    {
        Refresh();
    }

    private void BuildHud()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("ScoreComboHudUI: No Canvas found.");
            return;
        }

        var container = new GameObject("Score Combo HUD", typeof(RectTransform));
        container.transform.SetParent(canvas.transform, false);

        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 1f);
        containerRect.anchorMax = new Vector2(0.5f, 1f);
        containerRect.pivot = new Vector2(0.5f, 1f);
        containerRect.anchoredPosition = new Vector2(0f, -_topOffset);
        containerRect.sizeDelta = new Vector2(520f, _lineSpacing * 2f);

        _scoreText = CreateLabel(container.transform, "ScoreLabel", "Score: 0", 0f);
        _comboText = CreateLabel(container.transform, "ComboLabel", "Combo: 0", -_lineSpacing);
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string name, string text, float yOffset)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, yOffset);
        rt.sizeDelta = new Vector2(520f, _lineSpacing);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = _fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    private void Refresh()
    {
        if (_scoreText == null || _comboText == null)
            return;

        int score = GetScore();
        int combo = GetCombo();

        if (score != _lastScore)
        {
            _lastScore = score;
            _scoreText.text = $"Score: {score}";
        }

        if (combo != _lastCombo)
        {
            _lastCombo = combo;
            _comboText.text = $"Combo: {combo}";
        }
    }

    private static int GetScore()
    {
        AudienceSupport audienceSupport = GameManager.Instance != null
            ? GameManager.Instance.AudienceSupport
            : FindFirstObjectByType<AudienceSupport>();

        if (audienceSupport == null)
            return 0;

        return Mathf.RoundToInt(audienceSupport.AudienceSupportValue);
    }

    private static int GetCombo()
    {
        if (GameObserver.Instance == null)
            return 0;

        return GameObserver.Instance.ConsecutivePlatformLandings;
    }
}
