using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// F1 toggles left/right debug panels. Left: last despawn per lane. Right: latest spawn per lane.
/// </summary>
public class LaneDespawnDebugUI : MonoBehaviour
{
    [SerializeField] private KeyCode _toggleKey = KeyCode.F1;
    [SerializeField] private int _fallbackLaneCount = 5;
    [SerializeField] private float _panelWidth = 340f;
    [SerializeField] private int _canvasSortOrder = 125;

    private Canvas _rootCanvas;
    private TextMeshProUGUI[] _laneTexts;
    private TextMeshProUGUI[] _spawnLaneTexts;
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
            var rowLe = row.gameObject.AddComponent<LayoutElement>();
            rowLe.minHeight = 28f;
            rowLe.preferredHeight = 36f;
            _laneTexts[i] = row;
        }

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
            var rowLe = row.gameObject.AddComponent<LayoutElement>();
            rowLe.minHeight = 28f;
            rowLe.preferredHeight = 36f;
            _spawnLaneTexts[i] = row;
        }
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
