using UnityEngine;
using System.Globalization;

public class DebugMenu : MonoBehaviour
{
    // Add/remove rows in DrawWindow() to match DebugOptions.* you want exposed.
    [Header("Toggle")]
    [SerializeField] private KeyCode _menuKey = KeyCode.F1;
    [SerializeField] private bool _startOpen;

    [Header("Window")]
    [SerializeField] private Rect _windowRect = new Rect(20f, 20f, 520f, 380f);
    [SerializeField] private int _fontSize = 22;

    private bool _isOpen;
    private const int WindowId = 7581;
    private Vector2 _scroll;
    private string _perfectWindowInput;
    private string _noteSpeedInput;

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
        _perfectWindowInput = DebugOptions.PerfectWindow.ToString("0.00", CultureInfo.InvariantCulture);
        _noteSpeedInput = DebugOptions.NoteSpeed.ToString("0.00", CultureInfo.InvariantCulture);
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

        var textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = _fontSize
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
        GUILayout.Label($"Perfect Window: {DebugOptions.PerfectWindow:F2}s", labelStyle, GUILayout.MinHeight(rowHeight));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("- 0.01", GUILayout.MinHeight(rowHeight)))
            DebugOptions.SetPerfectWindow(DebugOptions.PerfectWindow - 0.01f);
        if (GUILayout.Button("+ 0.01", GUILayout.MinHeight(rowHeight)))
            DebugOptions.SetPerfectWindow(DebugOptions.PerfectWindow + 0.01f);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        _perfectWindowInput = GUILayout.TextField(_perfectWindowInput, textFieldStyle, GUILayout.MinHeight(rowHeight));
        if (GUILayout.Button("Apply", GUILayout.MinHeight(rowHeight)))
        {
            if (TryParseFloat(_perfectWindowInput, out float value))
            {
                DebugOptions.SetPerfectWindow(value);
                _perfectWindowInput = DebugOptions.PerfectWindow.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.Label($"Note Speed: {DebugOptions.NoteSpeed:F2}", labelStyle, GUILayout.MinHeight(rowHeight));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("- 0.05", GUILayout.MinHeight(rowHeight)))
            DebugOptions.SetNoteSpeed(DebugOptions.NoteSpeed - 0.05f);
        if (GUILayout.Button("+ 0.05", GUILayout.MinHeight(rowHeight)))
            DebugOptions.SetNoteSpeed(DebugOptions.NoteSpeed + 0.05f);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        _noteSpeedInput = GUILayout.TextField(_noteSpeedInput, textFieldStyle, GUILayout.MinHeight(rowHeight));
        if (GUILayout.Button("Apply", GUILayout.MinHeight(rowHeight)))
        {
            if (TryParseFloat(_noteSpeedInput, out float value))
            {
                DebugOptions.SetNoteSpeed(value);
                _noteSpeedInput = DebugOptions.NoteSpeed.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.Label($"Menu Key: {_menuKey}", labelStyle, GUILayout.MinHeight(rowHeight));

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private static bool TryParseFloat(string value, out float parsed)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed) ||
               float.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out parsed);
    }
}
