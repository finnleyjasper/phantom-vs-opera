using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Drives the Score label in audienceBar.uxml from <see cref="AudienceSupport"/>.
/// Requires a <see cref="UIDocument"/> on the same GameObject with that UXML assigned.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class AudienceBarUI : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _scoreLabelName = "Score";

    private Label _scoreLabel;
    private AudienceSupport _audienceSupport;
    private int _lastDisplayedScore = int.MinValue;

    private void Awake()
    {
        if (_uiDocument == null)
            _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        // UIDocument builds the visual tree on the next frame(s).
        for (int i = 0; i < 10; i++)
        {
            BindUi();
            if (_scoreLabel != null)
                break;
            yield return null;
        }

        RefreshScore(force: true);
    }

    private void Update()
    {
        RefreshScore();
    }

    private void BindUi()
    {
        if (_uiDocument == null)
            return;

        VisualElement root = _uiDocument.rootVisualElement;
        if (root == null)
            return;

        _scoreLabel = root.Q<Label>(_scoreLabelName);
        if (_scoreLabel == null)
            Debug.LogWarning($"AudienceBarUI: Label '{_scoreLabelName}' not found. Check audienceBar.uxml.");

        if (_audienceSupport == null)
            _audienceSupport = FindFirstObjectByType<AudienceSupport>();
    }

    private void RefreshScore(bool force = false)
    {
        if (_scoreLabel == null)
        {
            BindUi();
            if (_scoreLabel == null)
                return;
        }

        if (_audienceSupport == null)
            _audienceSupport = FindFirstObjectByType<AudienceSupport>();
        if (_audienceSupport == null)
            return;

        int score = Mathf.RoundToInt(_audienceSupport.AudienceSupportValue);
        if (!force && score == _lastDisplayedScore)
            return;

        _lastDisplayedScore = score;
        _scoreLabel.text = score.ToString();
    }
}
