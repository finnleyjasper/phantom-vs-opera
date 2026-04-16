using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// F1 toggles left/right debug panels. F1+M toggles full-screen debug menu.
/// </summary>
public class LaneDespawnDebugUI : MonoBehaviour
{
    public static bool IsFullMenuOpen { get; private set; }

    [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
    [SerializeField] private KeyCode _fullMenuModifierKey = KeyCode.M;
    [SerializeField] private int _fallbackLaneCount = 5;
    [SerializeField] private float _panelWidth = 340f;
    [SerializeField] private int _canvasSortOrder = 125;

    private Canvas _rootCanvas;
    private RectTransform _sidePanelsRoot;
    private RectTransform _fullScreenMenuPanel;
    private TextMeshProUGUI[] _laneTexts;
    private TextMeshProUGUI[] _spawnLaneTexts;
    private TextMeshProUGUI _playerHealthText;
    private TextMeshProUGUI _playerMaxHealthText;
    private TextMeshProUGUI _playerSuccessText;
    private TextMeshProUGUI _playerSpeedText;
    private TextMeshProUGUI _playerHitText;
    private TextMeshProUGUI _menuMaxHealthValueText;
    private TextMeshProUGUI _menuMaxSuccessValueText;
    private TextMeshProUGUI _menuSpeedValueText;
    private TextMeshProUGUI _menuPlatformSpeedValueText;
    private TextMeshProUGUI _menuAttackSpeedValueText;
    private Player _player;
    private PlayerController _playerController;
    private PlatformManager _platformManager;
    private FallingAttackSpawner _attackSpawner;
    private int _lastKnownHealth = -1;
    private int _hitRegisterCount;
    private int _laneCount;
    private bool _visible;
    private bool _fullMenuVisible;
    private bool _pausedByDebugMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureExists()
    {
        if (FindFirstObjectByType<LaneDespawnDebugUI>() != null) return;

        var go = new GameObject("LaneDespawnDebugUI");
        DontDestroyOnLoad(go);
        go.AddComponent<LaneDespawnDebugUI>();
    }

    private void Awake()
    {
        var spawner = FindFirstObjectByType<PlatformSpawner>();
        _laneCount = spawner != null && spawner.laneTransforms != null && spawner.laneTransforms.Count > 0
            ? spawner.laneTransforms.Count
            : _fallbackLaneCount;
        _player = FindFirstObjectByType<Player>();
        _playerController = FindFirstObjectByType<PlayerController>();
        _platformManager = FindFirstObjectByType<PlatformManager>();
        _attackSpawner = FindFirstObjectByType<FallingAttackSpawner>();
        _lastKnownHealth = _player != null ? _player.HealthBar : -1;

        BuildUi();
        UpdatePlayerStatsText();
        SetSidePanelsVisible(false);
        SetFullMenuVisible(false);
    }

    private void OnEnable()
    {
        PlatformManager.OnPlatformDespawning += OnPlatformDespawning;
        PlatformSpawner.OnPlatformSpawned += OnPlatformSpawned;
    }

    private void OnDisable()
    {
        PlatformManager.OnPlatformDespawning -= OnPlatformDespawning;
        PlatformSpawner.OnPlatformSpawned -= OnPlatformSpawned;
    }

    private void Update()
    {
        if (_fullMenuVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            _fullMenuVisible = false;
            SetFullMenuVisible(false);
            return;
        }

        if (Input.GetKey(_toggleKey) && Input.GetKeyDown(_fullMenuModifierKey))
        {
            _fullMenuVisible = !_fullMenuVisible;
            SetFullMenuVisible(_fullMenuVisible);
            SetSidePanelsVisible(false);
            _visible = false;
            return;
        }

        if (Input.GetKeyDown(_toggleKey))
        {
            _visible = !_visible;
            SetSidePanelsVisible(_visible);
            if (_visible)
            {
                _fullMenuVisible = false;
                SetFullMenuVisible(false);
            }
        }

        UpdatePlayerStatsText();
    }

    private void SetSidePanelsVisible(bool visible)
    {
        if (_sidePanelsRoot != null)
            _sidePanelsRoot.gameObject.SetActive(visible);
    }

    private void SetFullMenuVisible(bool visible)
    {
        if (_fullScreenMenuPanel != null)
            _fullScreenMenuPanel.gameObject.SetActive(visible);
        IsFullMenuOpen = visible;

        if (visible)
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPaused)
            {
                GameManager.Instance.PauseGame();
                _pausedByDebugMenu = true;
            }
            return;
        }

        if (_pausedByDebugMenu && GameManager.Instance != null && GameManager.Instance.IsPaused)
            GameManager.Instance.ResumeGame();

        _pausedByDebugMenu = false;
        RefreshDebugMenuValues();
    }

    private void OnPlatformDespawning(MusicPlatform platform)
    {
        if (platform == null) return;

        int lane = platform.laneIndex;
        if (lane < 0 || lane >= _laneCount || _laneTexts == null || lane >= _laneTexts.Length)
            return;

        _laneTexts[lane].text = FormatPlatformLine(platform);
    }

    private void OnPlatformSpawned(MusicPlatform platform)
    {
        if (platform == null) return;

        int lane = platform.laneIndex;
        if (lane < 0 || lane >= _laneCount || _spawnLaneTexts == null || lane >= _spawnLaneTexts.Length)
            return;

        _spawnLaneTexts[lane].text = FormatPlatformLine(platform);
    }

    private static string FormatPlatformLine(MusicPlatform platform)
    {
        string note = string.IsNullOrEmpty(platform.noteName)
            ? $"pitch {platform.pitch}"
            : $"{platform.noteName} (p{platform.pitch})";

        int lane = platform.laneIndex;
        string laneLabel = lane >= 0 ? $"Lane {lane + 1}" : "Lane ?";
        return $"{laneLabel}: {note}  ·  len {platform.length:F2}s";
    }

    private void BuildUi()
    {
        var canvasGo = new GameObject("LaneDespawnDebugCanvas");
        canvasGo.transform.SetParent(transform, false);

        _rootCanvas = canvasGo.AddComponent<Canvas>();
        _rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _rootCanvas.sortingOrder = _canvasSortOrder;

        canvasGo.AddComponent<GraphicRaycaster>();

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        var rootRt = canvasGo.GetComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        _sidePanelsRoot = CreateFullScreenRoot(canvasGo.transform, "SidePanelsRoot");
        var leftPanel = CreateSidePanel(_sidePanelsRoot, "LeftPanel", true);
        var rightPanel = CreateSidePanel(_sidePanelsRoot, "RightPanel", false);

        var leftVlg = leftPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        leftVlg.padding = new RectOffset(14, 14, 14, 14);
        leftVlg.spacing = 10f;
        leftVlg.childAlignment = TextAnchor.UpperLeft;
        leftVlg.childControlWidth = true;
        leftVlg.childControlHeight = false;
        leftVlg.childForceExpandWidth = true;
        leftVlg.childForceExpandHeight = false;

        var title = CreateTmpText(leftPanel, "Title", "Last despawn per lane", 22, FontStyles.Bold);
        var le = title.gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = 36f;

        _laneTexts = new TextMeshProUGUI[_laneCount];
        for (int i = 0; i < _laneCount; i++)
        {
            var row = CreateTmpText(leftPanel, $"Lane{i + 1}", $"Lane {i + 1}: —", 18, FontStyles.Normal);
            row.alignment = TextAlignmentOptions.TopLeft;
            row.enableWordWrapping = true;
            AddRowLayout(row);
            _laneTexts[i] = row;
        }

        var playerStatsTitle = CreateTmpText(leftPanel, "PlayerStatsTitle", "Player Stats", 20, FontStyles.Bold);
        AddRowLayout(playerStatsTitle);

        _playerHealthText = CreateTmpText(leftPanel, "PlayerHealth", "Player Health: —", 18, FontStyles.Normal);
        _playerMaxHealthText = CreateTmpText(leftPanel, "PlayerMaxHealth", "Player Max Health: —", 18, FontStyles.Normal);
        _playerSuccessText = CreateTmpText(leftPanel, "PlayerSuccess", "Success Meter: —", 18, FontStyles.Normal);
        _playerSpeedText = CreateTmpText(leftPanel, "PlayerSpeed", "Player Speed: —", 18, FontStyles.Normal);
        _playerHitText = CreateTmpText(leftPanel, "PlayerHits", "Hit Register: —", 18, FontStyles.Normal);
        AddRowLayout(_playerHealthText);
        AddRowLayout(_playerMaxHealthText);
        AddRowLayout(_playerSuccessText);
        AddRowLayout(_playerSpeedText);
        AddRowLayout(_playerHitText);

        var rightVlg = rightPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        rightVlg.padding = new RectOffset(14, 14, 14, 14);
        rightVlg.spacing = 10f;
        rightVlg.childAlignment = TextAnchor.UpperLeft;
        rightVlg.childControlWidth = true;
        rightVlg.childControlHeight = false;
        rightVlg.childForceExpandWidth = true;
        rightVlg.childForceExpandHeight = false;

        var rightTitle = CreateTmpText(rightPanel, "RightTitle", "Latest spawn per lane", 22, FontStyles.Bold);
        var rightTitleLe = rightTitle.gameObject.AddComponent<LayoutElement>();
        rightTitleLe.preferredHeight = 36f;

        _spawnLaneTexts = new TextMeshProUGUI[_laneCount];
        for (int i = 0; i < _laneCount; i++)
        {
            var row = CreateTmpText(rightPanel, $"SpawnLane{i + 1}", $"Lane {i + 1}: —", 18, FontStyles.Normal);
            row.alignment = TextAlignmentOptions.TopLeft;
            row.enableWordWrapping = true;
            AddRowLayout(row);
            _spawnLaneTexts[i] = row;
        }

        _fullScreenMenuPanel = CreateFullScreenRoot(canvasGo.transform, "FullScreenDebugMenu");
        var fullMenuBackground = _fullScreenMenuPanel.gameObject.AddComponent<Image>();
        fullMenuBackground.color = new Color(0.05f, 0.05f, 0.08f, 0.92f);

        var fullMenuTitle = CreateTmpText(_fullScreenMenuPanel, "FullMenuTitle", "Debug Menu", 38, FontStyles.Bold);
        fullMenuTitle.alignment = TextAlignmentOptions.Top;
        var fullMenuTitleRect = fullMenuTitle.rectTransform;
        fullMenuTitleRect.anchorMin = new Vector2(0.5f, 1f);
        fullMenuTitleRect.anchorMax = new Vector2(0.5f, 1f);
        fullMenuTitleRect.pivot = new Vector2(0.5f, 1f);
        fullMenuTitleRect.anchoredPosition = new Vector2(0f, -36f);
        fullMenuTitleRect.sizeDelta = new Vector2(600f, 60f);

        BuildFullDebugMenuControls(_fullScreenMenuPanel);
        RefreshDebugMenuValues();
    }

    private void UpdatePlayerStatsText()
    {
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_playerController == null) _playerController = FindFirstObjectByType<PlayerController>();

        if (_playerHealthText != null)
            _playerHealthText.text = _player != null ? $"Player Health: {_player.HealthBar}" : "Player Health: N/A";

        if (_playerMaxHealthText != null)
            _playerMaxHealthText.text = _player != null ? $"Player Max Health: {_player.MaxHealth}" : "Player Max Health: N/A";

        if (_playerSuccessText != null)
            _playerSuccessText.text = _player != null ? $"Success Meter: {_player.SuccessBar:F2}" : "Success Meter: N/A";

        if (_playerController != null)
        {
            _playerSpeedText.text = $"Player Speed: {_playerController.PlayerSpeed:F2}";
        }
        else if (_playerSpeedText != null)
        {
            _playerSpeedText.text = "Player Speed: N/A";
        }

        if (_player != null)
        {
            int currentHealth = _player.HealthBar;
            if (_lastKnownHealth >= 0 && currentHealth < _lastKnownHealth)
            {
                _hitRegisterCount += _lastKnownHealth - currentHealth;
            }

            _lastKnownHealth = currentHealth;
        }

        if (_playerHitText != null)
            _playerHitText.text = $"Hit Register: {_hitRegisterCount}";
    }

    private static void AddRowLayout(TextMeshProUGUI rowText)
    {
        var rowLe = rowText.gameObject.AddComponent<LayoutElement>();
        rowLe.minHeight = 28f;
        rowLe.preferredHeight = 36f;
    }

    private RectTransform CreateSidePanel(Transform parent, string name, bool left)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.pivot = left ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
        rt.anchorMin = left ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        rt.anchorMax = left ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
        rt.sizeDelta = new Vector2(_panelWidth, 0f);
        rt.anchoredPosition = left ? new Vector2(12f, 0f) : new Vector2(-12f, 0f);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.05f, 0.05f, 0.08f, 0.82f);

        return rt;
    }

    private void BuildFullDebugMenuControls(RectTransform parent)
    {
        var controlsRoot = new GameObject("FullMenuControls");
        controlsRoot.transform.SetParent(parent, false);
        var controlsRt = controlsRoot.AddComponent<RectTransform>();
        controlsRt.anchorMin = new Vector2(0f, 1f);
        controlsRt.anchorMax = new Vector2(0f, 1f);
        controlsRt.pivot = new Vector2(0f, 1f);
        controlsRt.anchoredPosition = new Vector2(36f, -120f);
        controlsRt.sizeDelta = new Vector2(760f, 520f);

        var layout = controlsRoot.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 12f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childAlignment = TextAnchor.UpperLeft;

        var sectionTitle = CreateTmpText(controlsRt, "PlayerSettingsTitle", "Player Settings", 28, FontStyles.Bold);
        sectionTitle.alignment = TextAlignmentOptions.Left;
        AddControlRowLayout(sectionTitle.rectTransform, 48f);

        _menuMaxHealthValueText = CreateAdjustControlRow(controlsRt, "MaxHealthControl", "Max Health", () => AdjustMaxHealth(-1), () => AdjustMaxHealth(1));
        _menuMaxSuccessValueText = CreateAdjustControlRow(controlsRt, "MaxSuccessControl", "Max Success Meter", () => AdjustMaxSuccess(-1f), () => AdjustMaxSuccess(1f));
        _menuSpeedValueText = CreateAdjustControlRow(controlsRt, "SpeedControl", "Speed", () => AdjustSpeed(-0.01f), () => AdjustSpeed(0.01f));

        var worldSettingsTitle = CreateTmpText(controlsRt, "WorldSettingsTitle", "World Settings", 28, FontStyles.Bold);
        worldSettingsTitle.alignment = TextAlignmentOptions.Left;
        AddControlRowLayout(worldSettingsTitle.rectTransform, 48f);

        _menuPlatformSpeedValueText = CreateAdjustControlRow(controlsRt, "PlatformSpeedControl", "Platform Speed", () => AdjustPlatformSpeed(-0.1f), () => AdjustPlatformSpeed(0.1f));
        _menuAttackSpeedValueText = CreateAdjustControlRow(controlsRt, "AttackSpeedControl", "Falling Attack Speed", () => AdjustAttackSpeed(-0.25f), () => AdjustAttackSpeed(0.25f));
    }

    private TextMeshProUGUI CreateAdjustControlRow(RectTransform parent, string rowName, string labelText, System.Action onMinus, System.Action onPlus)
    {
        var rowGo = new GameObject(rowName);
        rowGo.transform.SetParent(parent, false);
        var rowRt = rowGo.AddComponent<RectTransform>();
        AddControlRowLayout(rowRt, 58f);

        var rowLayout = rowGo.AddComponent<HorizontalLayoutGroup>();
        rowLayout.padding = new RectOffset(10, 10, 8, 8);
        rowLayout.spacing = 10f;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlHeight = true;
        rowLayout.childControlWidth = false;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = true;

        var rowBg = rowGo.AddComponent<Image>();
        rowBg.color = new Color(0.12f, 0.12f, 0.16f, 0.95f);

        var label = CreateTmpText(rowRt, $"{rowName}_Label", labelText, 20, FontStyles.Normal);
        label.alignment = TextAlignmentOptions.Left;
        AddFixedWidth(label.rectTransform, 280f);

        var minusButton = CreateMenuButton(rowRt, $"{rowName}_Minus", "-", onMinus);
        AddFixedWidth(minusButton.GetComponent<RectTransform>(), 56f);

        var valueText = CreateTmpText(rowRt, $"{rowName}_Value", "—", 20, FontStyles.Bold);
        valueText.alignment = TextAlignmentOptions.Center;
        AddFixedWidth(valueText.rectTransform, 220f);

        var plusButton = CreateMenuButton(rowRt, $"{rowName}_Plus", "+", onPlus);
        AddFixedWidth(plusButton.GetComponent<RectTransform>(), 56f);

        return valueText;
    }

    private GameObject CreateMenuButton(RectTransform parent, string name, string text, System.Action onClick)
    {
        var buttonGo = new GameObject(name);
        buttonGo.transform.SetParent(parent, false);
        var buttonRt = buttonGo.AddComponent<RectTransform>();
        buttonRt.sizeDelta = new Vector2(56f, 42f);

        var image = buttonGo.AddComponent<Image>();
        image.color = new Color(0.20f, 0.20f, 0.27f, 1f);

        var button = buttonGo.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick?.Invoke());

        var textTmp = CreateTmpText(buttonRt, $"{name}_Text", text, 28, FontStyles.Bold);
        textTmp.alignment = TextAlignmentOptions.Center;
        textTmp.rectTransform.anchorMin = Vector2.zero;
        textTmp.rectTransform.anchorMax = Vector2.one;
        textTmp.rectTransform.offsetMin = Vector2.zero;
        textTmp.rectTransform.offsetMax = Vector2.zero;

        return buttonGo;
    }

    private static void AddControlRowLayout(RectTransform rectTransform, float preferredHeight)
    {
        var le = rectTransform.gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;
        le.minHeight = preferredHeight;
    }

    private static void AddFixedWidth(RectTransform rectTransform, float width)
    {
        var le = rectTransform.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        le.minWidth = width;
    }

    private void AdjustMaxHealth(int delta)
    {
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_player == null) return;

        _player.SetMaxHealth(_player.MaxHealth + delta);
        UpdatePlayerStatsText();
        RefreshDebugMenuValues();
    }

    private void AdjustMaxSuccess(float delta)
    {
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_player == null) return;

        _player.SetMaxSuccessMeter(_player.MaxSuccessMeter + delta);
        UpdatePlayerStatsText();
        RefreshDebugMenuValues();
    }

    private void AdjustSpeed(float delta)
    {
        if (_playerController == null) _playerController = FindFirstObjectByType<PlayerController>();
        if (_playerController == null) return;

        _playerController.SetPlayerSpeed(_playerController.PlayerSpeed + delta);
        UpdatePlayerStatsText();
        RefreshDebugMenuValues();
    }

    private void RefreshDebugMenuValues()
    {
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_playerController == null) _playerController = FindFirstObjectByType<PlayerController>();
        if (_platformManager == null) _platformManager = FindFirstObjectByType<PlatformManager>();
        if (_attackSpawner == null) _attackSpawner = FindFirstObjectByType<FallingAttackSpawner>();

        if (_menuMaxHealthValueText != null)
            _menuMaxHealthValueText.text = _player != null ? _player.MaxHealth.ToString() : "N/A";

        if (_menuMaxSuccessValueText != null)
            _menuMaxSuccessValueText.text = _player != null ? _player.MaxSuccessMeter.ToString("F1") : "N/A";

        if (_menuSpeedValueText != null)
            _menuSpeedValueText.text = _playerController != null ? _playerController.PlayerSpeed.ToString("F2") : "N/A";

        if (_menuPlatformSpeedValueText != null)
            _menuPlatformSpeedValueText.text = _platformManager != null ? _platformManager.platformSpeed.ToString("F2") : "N/A";

        if (_menuAttackSpeedValueText != null)
            _menuAttackSpeedValueText.text = _attackSpawner != null ? _attackSpawner.AttackFallSpeed.ToString("F2") : "N/A";
    }

    private void AdjustPlatformSpeed(float delta)
    {
        if (_platformManager == null) _platformManager = FindFirstObjectByType<PlatformManager>();
        if (_platformManager == null) return;

        _platformManager.SetPlatformSpeed(_platformManager.platformSpeed + delta);
        RefreshDebugMenuValues();
    }

    private void AdjustAttackSpeed(float delta)
    {
        if (_attackSpawner == null) _attackSpawner = FindFirstObjectByType<FallingAttackSpawner>();
        if (_attackSpawner == null) return;

        _attackSpawner.SetAttackFallSpeed(_attackSpawner.AttackFallSpeed + delta);
        RefreshDebugMenuValues();
    }

    private static RectTransform CreateFullScreenRoot(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        return rt;
    }

    private static TextMeshProUGUI CreateTmpText(RectTransform parent, string name, string text, float fontSize,
        FontStyles style)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        go.AddComponent<RectTransform>();

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.raycastTarget = false;
        return tmp;
    }
}
