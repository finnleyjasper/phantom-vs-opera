using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// F1 toggles left/right debug panels. Left: last despawn per lane, audio track + current track. Right: latest spawn per lane, audience tuning, and last fixed-step audience Δ.
/// </summary>
public class LaneDespawnDebugUI : MonoBehaviour
{
    [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
    [SerializeField] private int _fallbackLaneCount = 5;
    [SerializeField] private float _panelWidth = 340f;
    [SerializeField] private int _canvasSortOrder = 125;

    private const float TrackSwitchUiScale = 1.25f;

    private Canvas _rootCanvas;
    private TextMeshProUGUI[] _laneTexts;
    private TextMeshProUGUI[] _spawnLaneTexts;
    private TextMeshProUGUI _landingBonusLine;
    private TextMeshProUGUI _ridingGainPerSecondLine;
    private TextMeshProUGUI _lastAudienceDeltaLine;
    private TextMeshProUGUI _currentTrackLine;
    private int _laneCount;
    private bool _visible;

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

        BuildUi();
        SetPanelsVisible(false);
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
        if (Input.GetKeyDown(_toggleKey))
        {
            _visible = !_visible;
            SetPanelsVisible(_visible);
        }

        if (_visible)
        {
            if (_landingBonusLine != null)
                RefreshAudienceSupportParams();
            if (_currentTrackLine != null)
                RefreshCurrentTrackLine();
        }
    }

    private void SetPanelsVisible(bool visible)
    {
        if (_rootCanvas != null)
            _rootCanvas.gameObject.SetActive(visible);
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

        var leftPanel = CreateSidePanel(canvasGo.transform, "LeftPanel", true);
        var rightPanel = CreateSidePanel(canvasGo.transform, "RightPanel", false);

        var leftPanelVlg = leftPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        ApplyPanelContentVerticalLayout(leftPanelVlg);

        var title = CreateTmpText(leftPanel, "Title", "Last despawn per lane", 22, FontStyles.Bold);
        var le = title.gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = 36f;

        _laneTexts = new TextMeshProUGUI[_laneCount];
        for (int i = 0; i < _laneCount; i++)
        {
            var row = CreateTmpText(leftPanel, $"Lane{i + 1}", $"Lane {i + 1}: —", 18, FontStyles.Normal);
            row.alignment = TextAlignmentOptions.TopLeft;
            row.enableWordWrapping = true;
            var rowLe = row.gameObject.AddComponent<LayoutElement>();
            rowLe.minHeight = 28f;
            rowLe.preferredHeight = 36f;
            _laneTexts[i] = row;
        }

        var trackSectionTitle = CreateTmpText(leftPanel, "TrackSectionTitle", "Audio track", 22, FontStyles.Bold);
        var trackSectionTitleLe = trackSectionTitle.gameObject.AddComponent<LayoutElement>();
        trackSectionTitleLe.preferredHeight = 36f;

        Color trackMetaMuted = new Color(0.85f, 0.9f, 1f);
        const float trackMetaFont = 17f;
        _currentTrackLine = CreateTmpText(leftPanel, "CurrentTrackLine", "Current track: —", trackMetaFont, FontStyles.Normal);
        _currentTrackLine.color = trackMetaMuted;
        _currentTrackLine.enableWordWrapping = true;
        var currentTrackLe = _currentTrackLine.gameObject.AddComponent<LayoutElement>();
        currentTrackLe.minHeight = 22f;
        currentTrackLe.preferredHeight = 26f;

        BuildTrackSwitchRow(leftPanel);

        var leftBottomSpacer = new GameObject("LeftPanelBottomSpacer");
        leftBottomSpacer.transform.SetParent(leftPanel, false);
        var leftBottomSpacerLe = leftBottomSpacer.AddComponent<LayoutElement>();
        leftBottomSpacerLe.flexibleHeight = 1f;
        leftBottomSpacerLe.minHeight = 0f;

        var rightPanelVlg = rightPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        ApplyPanelContentVerticalLayout(rightPanelVlg);

        var rightTitle = CreateTmpText(rightPanel, "RightTitle", "Latest spawn per lane", 22, FontStyles.Bold);
        var rightTitleLe = rightTitle.gameObject.AddComponent<LayoutElement>();
        rightTitleLe.preferredHeight = 36f;

        _spawnLaneTexts = new TextMeshProUGUI[_laneCount];
        for (int i = 0; i < _laneCount; i++)
        {
            var row = CreateTmpText(rightPanel, $"SpawnLane{i + 1}", $"Lane {i + 1}: —", 18, FontStyles.Normal);
            row.alignment = TextAlignmentOptions.TopLeft;
            row.enableWordWrapping = true;
            var rowLe = row.gameObject.AddComponent<LayoutElement>();
            rowLe.minHeight = 28f;
            rowLe.preferredHeight = 36f;
            _spawnLaneTexts[i] = row;
        }

        Color audienceMuted = new Color(0.85f, 0.9f, 1f);
        const float audienceFont = 17f;

        _landingBonusLine = CreateTmpText(rightPanel, "LandingBonusLine",
            "Landing bonus (on touch): —", audienceFont, FontStyles.Normal);
        _landingBonusLine.color = audienceMuted;
        _landingBonusLine.enableWordWrapping = true;
        var landingLe = _landingBonusLine.gameObject.AddComponent<LayoutElement>();
        landingLe.minHeight = 24f;
        landingLe.preferredHeight = 28f;

        _ridingGainPerSecondLine = CreateTmpText(rightPanel, "RidingGainPerSecondLine",
            "Riding gain per second: —", audienceFont, FontStyles.Normal);
        _ridingGainPerSecondLine.color = audienceMuted;
        _ridingGainPerSecondLine.enableWordWrapping = true;
        var ridingLe = _ridingGainPerSecondLine.gameObject.AddComponent<LayoutElement>();
        ridingLe.minHeight = 24f;
        ridingLe.preferredHeight = 28f;

        _lastAudienceDeltaLine = CreateTmpText(rightPanel, "LastAudienceDeltaLine",
            "Last fixed-step audience Δ: —", audienceFont, FontStyles.Normal);
        _lastAudienceDeltaLine.color = audienceMuted;
        _lastAudienceDeltaLine.enableWordWrapping = true;
        var lastDeltaLe = _lastAudienceDeltaLine.gameObject.AddComponent<LayoutElement>();
        lastDeltaLe.minHeight = 24f;
        lastDeltaLe.preferredHeight = 40f;
    }

    /// <summary>Matches <see cref="AudioManager.SwitchTrack"/>: 0 = full mix, 1–4 = isolated stems.</summary>
    private void BuildTrackSwitchRow(RectTransform leftPanel)
    {
        var root = new GameObject("TrackSwitchRow");
        root.transform.SetParent(leftPanel, false);

        var vlg = root.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.spacing = 6f * TrackSwitchUiScale;
        vlg.padding = new RectOffset(0, 0, 0, 0);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var rootLe = root.AddComponent<LayoutElement>();
        rootLe.minHeight = 104f * TrackSwitchUiScale;
        rootLe.preferredHeight = 112f * TrackSwitchUiScale;

        Transform row12 = CreateTrackSwitchHRow(root.transform, "TrackRow12");
        CreateTrackSwitchButton(row12, "TrackSwitch_1", "1", 1f);
        CreateTrackSwitchButton(row12, "TrackSwitch_2", "2", 2f);

        Transform row34 = CreateTrackSwitchHRow(root.transform, "TrackRow34");
        CreateTrackSwitchButton(row34, "TrackSwitch_3", "3", 3f);
        CreateTrackSwitchButton(row34, "TrackSwitch_4", "4", 4f);

        Transform rowAll = CreateTrackSwitchHRow(root.transform, "TrackRowAll");
        CreateTrackSwitchButton(rowAll, "TrackSwitch_0", "All", 0f);
    }

    private static Transform CreateTrackSwitchHRow(Transform parent, string name)
    {
        var rowGo = new GameObject(name);
        rowGo.transform.SetParent(parent, false);

        var hlg = rowGo.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 4f * TrackSwitchUiScale;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = false;

        var rowLe = rowGo.AddComponent<LayoutElement>();
        rowLe.minHeight = 30f * TrackSwitchUiScale;
        rowLe.preferredHeight = 32f * TrackSwitchUiScale;

        return rowGo.transform;
    }

    private void CreateTrackSwitchButton(Transform parent, string name, string label, float trackIndex)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
        img.color = new Color(0.22f, 0.24f, 0.32f, 0.95f);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.38f, 0.42f, 0.52f, 1f);
        colors.pressedColor = new Color(0.14f, 0.16f, 0.22f, 1f);
        btn.colors = colors;

        float captured = trackIndex;
        btn.onClick.AddListener(() => OnTrackSwitchClicked(captured));

        var le = go.AddComponent<LayoutElement>();
        le.flexibleWidth = 1f;
        le.minHeight = 28f * TrackSwitchUiScale;
        le.preferredHeight = 32f * TrackSwitchUiScale;

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 14f * TrackSwitchUiScale;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
    }

    private static void OnTrackSwitchClicked(float track)
    {
        if (GameManager.Instance == null)
            return;
        GameManager.Instance.SwitchTrack(track);
    }

    private static void ApplyPanelContentVerticalLayout(VerticalLayoutGroup vlg)
    {
        vlg.padding = new RectOffset(14, 14, 14, 14);
        vlg.spacing = 10f;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
    }

    private void RefreshAudienceSupportParams()
    {
        if (_landingBonusLine == null || _ridingGainPerSecondLine == null)
            return;

        if (GameManager.Instance == null)
        {
            _landingBonusLine.text = "Landing bonus (on touch): —";
            _ridingGainPerSecondLine.text = "Riding gain per second: —";
            if (_lastAudienceDeltaLine != null)
                _lastAudienceDeltaLine.text = "Last fixed-step audience Δ: —";
            return;
        }

        GameManager gm = GameManager.Instance;
        _landingBonusLine.text = $"Landing bonus (on touch): {gm.LandingBonus:F2}";
        _ridingGainPerSecondLine.text = $"Riding gain per second: {gm.IncreasePerSecond:F2}";

        if (_lastAudienceDeltaLine == null)
            return;

        if (gm.CurrentGameState != GameManager.GameState.Play || GameObserver.Instance == null)
        {
            _lastAudienceDeltaLine.text = "Last fixed-step audience Δ: —";
            return;
        }

        float d = GameObserver.Instance.LastFixedStepAudienceDelta;
        float fdt = Time.fixedDeltaTime;
        float perSec = fdt > 1e-5f ? d / fdt : 0f;
        _lastAudienceDeltaLine.text = $"Last fixed-step audience Δ: {d:+#0.000;-#0.000;0.000} " +
            $"(~{perSec:+#0.0;-#0.0;0.0} /s if sustained)";
    }

    private void RefreshCurrentTrackLine()
    {
        if (_currentTrackLine == null)
            return;

        if (GameManager.Instance == null)
        {
            _currentTrackLine.text = "Current track: —";
            return;
        }

        _currentTrackLine.text = "Current track: " + GameManager.Instance.CurrentTrack;
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
