using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays <see cref="LeaderboardStorage"/> entries on the Game Over (or menu) scene.
/// If no <see cref="_entriesText"/> is assigned, builds a simple full-screen overlay with list + arcade-style blinking hints (R / M).
/// </summary>
/// <remarks>
/// Populate scores with <see cref="LeaderboardStorage.RecordRun"/> when a run ends (e.g. from <c>GameManager.GameOver</c>).
/// </remarks>
public class LeaderboardSceneUI : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private string _playAgainScene = "Main";
    [SerializeField] private string _mainMenuScene = "Main Menu";

    [Header("Optional UI")]
    [Tooltip("If null and build-at-runtime is enabled, a list is created automatically.")]
    [SerializeField] private TextMeshProUGUI _entriesText;

    [Tooltip("Optional: one TMP with two lines (R/M hints) for blinking. Leave empty when using runtime UI.")]
    [SerializeField] private TextMeshProUGUI _arcadeHintsText;

    [Header("Arcade hints")]
    [SerializeField] private string _hintRestartText = "PRESS R TO RESTART";
    [SerializeField] private string _hintMainMenuText = "PRESS M TO MAIN MENU";
    [Tooltip("TMP line spacing between the two lines (0 = default; negative = tighter, positive = looser).")]
    [SerializeField] private float _hintLineSpacing = 0f;
    [Tooltip("Seconds per flash state (on, then off, then on…). Smaller = faster blink.")]
    [SerializeField] private float _blinkToggleSeconds = 0.28f;
    [SerializeField] private float _alphaWhenOn = 1f;
    [SerializeField] private float _alphaWhenOff = 0f;

    [Header("Runtime UI")]
    [SerializeField] private bool _buildUiWhenNoText = true;
    [SerializeField] private string _panelTitle = "Leaderboard";
    [SerializeField] private int _canvasSortOrder = 50;

    private void Start()
    {
        if (_buildUiWhenNoText && _entriesText == null)
            BuildRuntimeLeaderboardUi();

        RefreshEntriesText();
    }

    private void Update()
    {
        UpdateArcadeHintsBlink();

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.rKey.wasPressedThisFrame)
            LoadScene(_playAgainScene);
        if (kb.mKey.wasPressedThisFrame)
            LoadScene(_mainMenuScene);
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

    /// <summary>Rebuilds the list from <see cref="LeaderboardStorage"/>.</summary>
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

        RectTransform rootRt = canvasGo.GetComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

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
        var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = _panelTitle;
        titleTmp.fontSize = 36;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;
        var titleLe = titleGo.AddComponent<LayoutElement>();
        titleLe.preferredHeight = 52f;

        var bodyGo = new GameObject("Entries");
        bodyGo.transform.SetParent(panel.transform, false);
        _entriesText = bodyGo.AddComponent<TextMeshProUGUI>();
        _entriesText.fontSize = 24;
        _entriesText.alignment = TextAlignmentOptions.Top;
        _entriesText.color = new Color(0.92f, 0.92f, 0.92f);
        _entriesText.enableWordWrapping = true;
        var bodyLe = bodyGo.AddComponent<LayoutElement>();
        bodyLe.flexibleHeight = 1f;
        bodyLe.minHeight = 160f;

        var hintsGo = new GameObject("ArcadeHints");
        hintsGo.transform.SetParent(panel.transform, false);
        var hintsLe = hintsGo.AddComponent<LayoutElement>();
        hintsLe.preferredHeight = 52f;

        _arcadeHintsText = hintsGo.AddComponent<TextMeshProUGUI>();
        _arcadeHintsText.text = $"{_hintRestartText}\n{_hintMainMenuText}";
        _arcadeHintsText.fontSize = 30;
        _arcadeHintsText.fontStyle = FontStyles.Bold;
        _arcadeHintsText.alignment = TextAlignmentOptions.Center;
        _arcadeHintsText.color = Color.white;
        _arcadeHintsText.enableWordWrapping = false;
        _arcadeHintsText.lineSpacing = _hintLineSpacing;
        _arcadeHintsText.paragraphSpacing = 0f;
    }
}
