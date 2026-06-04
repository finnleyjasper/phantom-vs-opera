using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Arcade-style name entry after a run. Shows score, placement, 3-letter initials, then continues to Game Over.
/// </summary>
public class NameRecordSceneUI : MonoBehaviour
{
    [Header("Copy")]
    [SerializeField] private string _titleDefault = "Your Score";
    [SerializeField] private string _titleNewHigh = "New High Score!";
    [SerializeField] private string _continueHintLine = "PRESS SPACE TO CONTINUE";

    [Header("Arcade hint blink")]
    [SerializeField] private float _blinkToggleSeconds = 0.28f;
    [SerializeField] private float _alphaWhenOn = 1f;
    [SerializeField] private float _alphaWhenOff = 0f;

    [Header("Layout")]
    [SerializeField] private int _canvasSortOrder = 50;
    [SerializeField] private int _initialLength = 3;

    [Header("Navigation")]
    [SerializeField] private string _leaderboardSceneName = "Game Over";

    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _placementText;
    private TextMeshProUGUI _initialsText;
    private TextMeshProUGUI _continueHintTmp;

    private float _score;
    private int _placement;
    private bool _isNewHighScore;
    private string _initials = "";
    private bool _hasContinued;

    private void Start()
    {
        BuildUi();

        if (!GameManager.TryTakePendingLeaderboardScore(out _score))
        {
            LoadGameOver();
            return;
        }

        _placement = LeaderboardStorage.GetPlacementForScore(_score);
        _isNewHighScore = LeaderboardStorage.IsNewHighScore(_score);
        RefreshDisplay();
    }

    private void Update()
    {
        UpdateContinueHintBlink();
        HandleInitialsInput();

        if (!WasSpacePressedThisFrame()) return;

        CommitAndContinue();
    }

    private void HandleInitialsInput()
    {
        if (WasBackspacePressedThisFrame() && _initials.Length > 0)
        {
            _initials = _initials.Substring(0, _initials.Length - 1);
            RefreshDisplay();
            return;
        }

        if (_initials.Length >= _initialLength) return;

        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (WasLetterPressedThisFrame(c))
            {
                _initials += c;
                RefreshDisplay();
                return;
            }
        }
    }

    private void CommitAndContinue()
    {
        if (_hasContinued) return;
        _hasContinued = true;

        LeaderboardStorage.RecordRun(_score, FormatInitialsForSave(_initials));
        LoadGameOver();
    }

    private static string FormatInitialsForSave(string raw)
    {
        string upper = string.IsNullOrEmpty(raw) ? "AAA" : raw.ToUpperInvariant();
        if (upper.Length >= 3) return upper.Substring(0, 3);
        return upper.PadRight(3, 'A');
    }

    private void LoadGameOver()
    {
        string sceneName = _leaderboardSceneName;
        if (LevelLoader.Instance != null && !string.IsNullOrEmpty(LevelLoader.Instance.EndSceneName))
            sceneName = LevelLoader.Instance.EndSceneName;

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("NameRecordSceneUI: leaderboard scene name is missing.");
            _hasContinued = false;
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private static bool WasSpacePressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;

        return Input.GetKeyDown(KeyCode.Space);
    }

    private static bool WasBackspacePressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
            return true;

        return Input.GetKeyDown(KeyCode.Backspace);
    }

    private static bool WasLetterPressedThisFrame(char letter)
    {
        if (letter < 'A' || letter > 'Z') return false;

        if (Keyboard.current != null)
        {
            Key key = Key.A + (letter - 'A');
            if (Keyboard.current[key].wasPressedThisFrame)
                return true;
        }

        return Input.GetKeyDown(KeyCode.A + (letter - 'A'));
    }

    private void RefreshDisplay()
    {
        if (_titleText != null)
            _titleText.text = _isNewHighScore ? _titleNewHigh : _titleDefault;

        if (_scoreText != null)
            _scoreText.text = Mathf.RoundToInt(_score).ToString();

        if (_placementText != null)
            _placementText.text = LeaderboardStorage.FormatOrdinal(_placement);

        if (_initialsText != null)
        {
            var slots = new char[_initialLength];
            for (int i = 0; i < _initialLength; i++)
                slots[i] = i < _initials.Length ? _initials[i] : '_';
            _initialsText.text = new string(slots);
        }
    }

    private void UpdateContinueHintBlink()
    {
        if (_continueHintTmp == null) return;

        float interval = Mathf.Max(0.05f, _blinkToggleSeconds);
        bool lit = (Mathf.FloorToInt(Time.unscaledTime / interval) % 2) == 0;
        float a = lit ? _alphaWhenOn : _alphaWhenOff;
        Color c = _continueHintTmp.color;
        c.a = a;
        _continueHintTmp.color = c;
    }

    private void BuildUi()
    {
        var canvasGo = new GameObject("NameRecordCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = _canvasSortOrder;
        canvasGo.AddComponent<GraphicRaycaster>();

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGo.transform, false);
        var bgRt = bg.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.black;
        bgImg.raycastTarget = false;

        var stack = new GameObject("Stack");
        stack.transform.SetParent(canvasGo.transform, false);
        var stackRt = stack.AddComponent<RectTransform>();
        stackRt.anchorMin = new Vector2(0.5f, 0.5f);
        stackRt.anchorMax = new Vector2(0.5f, 0.5f);
        stackRt.pivot = new Vector2(0.5f, 1f);
        stackRt.anchoredPosition = new Vector2(0f, 420f);
        stackRt.sizeDelta = new Vector2(900f, 700f);

        var vlg = stack.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 28f;

        _titleText = CreateTmp(stack.transform, "Title", _titleDefault, 52, FontStyles.Bold, 72f);
        _scoreText = CreateTmp(stack.transform, "Score", "0", 96, FontStyles.Bold, 120f);
        _placementText = CreateTmp(stack.transform, "Placement", "1st", 40, FontStyles.Normal, 56f);
        _initialsText = CreateTmp(stack.transform, "Initials", "___", 72, FontStyles.Bold, 96f);
        _initialsText.characterSpacing = 24f;

        var spacer = new GameObject("Spacer");
        spacer.transform.SetParent(stack.transform, false);
        var spacerLe = spacer.AddComponent<LayoutElement>();
        spacerLe.preferredHeight = 48f;

        _continueHintTmp = CreateTmp(stack.transform, "ContinueHint", _continueHintLine, 30, FontStyles.Bold, 52f);
    }

    private static TextMeshProUGUI CreateTmp(
        Transform parent,
        string name,
        string text,
        float fontSize,
        FontStyles style,
        float preferredHeight)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;
        return tmp;
    }
}
