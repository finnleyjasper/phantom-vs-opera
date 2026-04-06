using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    // Add/remove rows in DrawWindow() to match DebugOptions.* you want exposed.
    [Header("Toggle")]
    [SerializeField] private KeyCode _menuKey = KeyCode.F3;
    [SerializeField] private bool _startOpen;

    [Header("Window")]
    [SerializeField] private Rect _windowRect = new Rect(20f, 20f, 520f, 380f);
    [SerializeField] private int _fontSize = 22;

    private bool _isOpen;
    private const int WindowId = 7581;
    private Vector2 _scroll;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureMenuExists()
    {
        if (FindObjectOfType<DebugMenu>() != null) return;

        var menuObject = new GameObject("DebugMenu");
        DontDestroyOnLoad(menuObject);
        menuObject.AddComponent<DebugMenu>();
    }

    private void Start()
    {
        _isOpen = _startOpen;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_menuKey))
            _isOpen = !_isOpen;
    }

    private void OnGUI()
    {
        if (!_isOpen) return;
        _windowRect = GUI.Window(WindowId, _windowRect, DrawWindow, "Debug Menu");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

        float rowHeight = Mathf.Max(24f, _fontSize * 1.5f);

        var toggleStyle = new GUIStyle(GUI.skin.toggle)
        {
            fontSize = _fontSize,
            wordWrap = false,
            fixedHeight = 0f
        };

        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = _fontSize,
            wordWrap = false,
            fixedHeight = 0f
        };

        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

        bool debugLogHits = GUILayout.Toggle(DebugOptions.DebugLogHits, "Debug Log Hits", toggleStyle,
            GUILayout.MinHeight(rowHeight));
        if (debugLogHits != DebugOptions.DebugLogHits)
            DebugOptions.SetDebugLogHits(debugLogHits);

        bool enableManualSpawn = GUILayout.Toggle(DebugOptions.EnableManualSpawn, "Enable Manual Spawn", toggleStyle,
            GUILayout.MinHeight(rowHeight));
        if (enableManualSpawn != DebugOptions.EnableManualSpawn)
            DebugOptions.SetManualSpawnEnabled(enableManualSpawn);

        GUILayout.Space(10f);
        GUILayout.Label($"Menu Key: {_menuKey}", labelStyle, GUILayout.MinHeight(rowHeight));

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}
