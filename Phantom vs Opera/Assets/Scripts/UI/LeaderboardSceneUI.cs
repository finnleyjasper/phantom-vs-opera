using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays <see cref="LeaderboardStorage"/> entries on the Game Over scene.
/// </summary>
public class LeaderboardSceneUI : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private string _playAgainScene = "Act 1";
    [SerializeField] private string _mainMenuScene = "Main Menu";

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI _entriesText;
    [SerializeField] private TextMeshProUGUI _arcadeHintsText;

    [Header("Arcade hints")]
    [SerializeField] private string _hintRestartText = "PRESS R TO RESTART";
    [SerializeField] private string _hintMainMenuText = "PRESS M TO MAIN MENU";
    [SerializeField] private float _hintLineSpacing = 0f;
    [SerializeField] private float _blinkToggleSeconds = 0.28f;
    [SerializeField] private float _alphaWhenOn = 1f;
    [SerializeField] private float _alphaWhenOff = 0f;

    [Header("Runtime UI")]
    [SerializeField] private bool _buildUiWhenNoText = true;
    [SerializeField] private string _panelTitle = "Leaderboard";
    [SerializeField] private int _canvasSortOrder = 50;

    [Header("Typography")]
    [SerializeField] private float _titleFontSize = 52f;
    [SerializeField] private float _entriesFontSize = 42f;
    [SerializeField] private float _hintsFontSize = 36f;
    [SerializeField] private float _entryLineSpacing = 12f;

    private TextMeshProUGUI _titleText;

    private void Start()
    {
        if (_buildUiWhenNoText && _entriesText == null)
            BuildRuntimeLeaderboardUi();

        ApplyTypography();
        RefreshEntriesText();
    }

    private void ApplyTypography()
    {
        if (_titleText != null)
        {
            _titleText.fontSize = _titleFontSize;
            _titleText.fontStyle = FontStyles.Bold;
        }

        if (_entriesText != null)
        {
            _entriesText.fontSize = _entriesFontSize;
            _entriesText.lineSpacing = _entryLineSpacing;
        }

        if (_arcadeHintsText != null)
            _arcadeHintsText.fontSize = _hintsFontSize;
    }

    private void Update()
    {
        UpdateArcadeHintsBlink();

        if (WasKeyPressedThisFrame(Key.R) || Input.GetKeyDown(KeyCode.R))
            LoadScene(_playAgainScene);
        if (WasKeyPressedThisFrame(Key.M) || Input.GetKeyDown(KeyCode.M))
            LoadScene(_mainMenuScene);
    }

    private static bool WasKeyPressedThisFrame(Key key)
    {
        return Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame;
    }

    private void UpdateArcadeHintsBlink()
    {
        if (_arcadeHintsText == null) return;

        float interval = Mathf.Max(0.05f, _blinkToggleSeconds);
        bool lit = (Mathf.FloorToInt(Time.unscaledTime / interval) % 2) == 0;
        float a = lit ? _alphaWhenOn : _alphaWhenOff;
        SetTmpAlpha(_arcadeHintsText, a);
    }

    private static void SetTmpAlpha(TextMeshProUGUI tmp, float a)
    {
        Color c = tmp.color;
        c.a = a;
        tmp.color = c;
    }

    private static void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        SceneManager.LoadScene(sceneName);
    }

    public void RefreshEntriesText()
    {
        if (_entriesText == null) return;

        var entries = LeaderboardStorage.GetTopEntries();
        var sb = new StringBuilder();

        if (entries == null || entries.Count == 0)
            sb.Append("No scores yet.");
        else
        {
            for (int i = 0; i < entries.Count; i++)
            {
                LeaderboardEntry e = entries[i];
                sb.AppendLine($"{i + 1}. {e.playerName}  —  {Mathf.RoundToInt(e.score)}");
            }
        }

        _entriesText.text = sb.ToString().TrimEnd();
    }

    private void BuildRuntimeLeaderboardUi()
    {
        var canvasGo = new GameObject("LeaderboardCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = _canvasSortOrder;
        canvasGo.AddComponent<GraphicRaycaster>();

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        RectTransform panelRt = panel.AddComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = new Vector2(40f, 40f);
        panelRt.offsetMax = new Vector2(-40f, -40f);

        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.78f);

        var vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(24, 24, 24, 24);
        vlg.spacing = 16f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;

        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(panel.transform, false);
        _titleText = titleGo.AddComponent<TextMeshProUGUI>();
        _titleText.text = _panelTitle;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.alignment = TextAlignmentOptions.Center;
        _titleText.color = Color.white;
        var titleLe = titleGo.AddComponent<LayoutElement>();
        titleLe.preferredHeight = 72f;

        var bodyGo = new GameObject("Entries");
        bodyGo.transform.SetParent(panel.transform, false);
        _entriesText = bodyGo.AddComponent<TextMeshProUGUI>();
        _entriesText.alignment = TextAlignmentOptions.Top;
        _entriesText.color = new Color(0.92f, 0.92f, 0.92f);
        _entriesText.enableWordWrapping = true;
        var bodyLe = bodyGo.AddComponent<LayoutElement>();
        bodyLe.flexibleHeight = 1f;
        bodyLe.minHeight = 280f;

        var hintsGo = new GameObject("ArcadeHints");
        hintsGo.transform.SetParent(panel.transform, false);
        var hintsLe = hintsGo.AddComponent<LayoutElement>();
        hintsLe.preferredHeight = 80f;

        _arcadeHintsText = hintsGo.AddComponent<TextMeshProUGUI>();
        _arcadeHintsText.text = $"{_hintRestartText}\n{_hintMainMenuText}";
        _arcadeHintsText.fontStyle = FontStyles.Bold;
        _arcadeHintsText.alignment = TextAlignmentOptions.Center;
        _arcadeHintsText.color = Color.white;
        _arcadeHintsText.enableWordWrapping = false;
        _arcadeHintsText.lineSpacing = _hintLineSpacing;
    }
}
