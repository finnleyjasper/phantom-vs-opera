using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Whether the player hit the input on time or missed.
public enum HitResult
{
    Perfect,
    Fail
}

// Data for one input — index ties it to an indicator, HitTime is in seconds.
[Serializable]
public class BeatNote
{
    public int IndicatorIndex;
    public float HitTime;
    public bool IsSpecial;

    [HideInInspector] public bool IsConsumed;
}

// Core manager
public class InputIndicatorManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject _player;

    [Header("Indicators")]
    [SerializeField] private List<InputIndicator> _indicators = new();
    [SerializeField] private InputIndicator _indicatorPrefab;
    [SerializeField] private Transform _indicatorParent;

    [Header("Timing")]
    [SerializeField] private float _perfectWindow = 0.15f;

    [Header("Beat Map")]
    [SerializeField] private List<BeatNote> _beatMap = new();

    [Header("Debug / Test")]
    [SerializeField] private bool _generateTestBeats = true;
    [SerializeField] private int _testBeatCount = 20;
    [SerializeField] private float _testBeatInterval = 1f;
    [SerializeField] private bool _debugLogHits = false;
    [SerializeField] private bool _enableManualSpawn = true;
    [SerializeField] private float _manualSpawnLeadTime = 2f;

    private float _songTime;
    private bool _isPlaying;

    public GameObject Player
    {
        get => _player;
        set => _player = value;
    }

    public float SongTime => _songTime;
    public bool IsPlaying => _isPlaying;
    public IReadOnlyList<BeatNote> BeatMap => _beatMap;
    public IReadOnlyList<InputIndicator> Indicators => _indicators;

    // Subscribe to these to react to hits/misses in other scripts.
    public event Action<int, HitResult> OnHitResult;
    public event Action<BeatNote, HitResult> OnBeatConsumed;
    public event Action<BeatNote> OnBeatAdded;

    private void Awake()
    {
        DebugOptions.Initialize(_debugLogHits, _enableManualSpawn);
        DebugOptions.InitializeTimingDefaults(_perfectWindow, DebugOptions.NoteSpeed);
        SyncDebugFlagsFromGlobal();
        DebugOptions.OnOptionsChanged += HandleDebugOptionsChanged;

        if (_generateTestBeats && _indicators.Count == 0)
            CreateDefaultIndicators();

        if (_generateTestBeats)
            GenerateTestBeats();
    }

    private void Start()
    {
        _isPlaying = true;
        _songTime = 0f;

        Debug.Log($"[Rhythm] Started with {_indicators.Count} indicators and {_beatMap.Count} beats. " +
                  $"Perfect window: +/- {_perfectWindow}s");
    }

    private void Update()
    {
        if (!_isPlaying) return;

        _songTime += Time.deltaTime;

        CheckForMissedBeats();

        if (_enableManualSpawn && (IsCtrlHeld() || IsShiftHeld()))
            CheckManualSpawn(IsShiftHeld());
        else
            CheckPlayerInput();
    }

    private void OnDestroy()
    {
        DebugOptions.OnOptionsChanged -= HandleDebugOptionsChanged;
    }

    // Creates a new indicator at runtime and parents it under this object.
    public InputIndicator AddIndicator(Key key, Sprite noteSprite = null)
    {
        Transform parent = _indicatorParent != null ? _indicatorParent : transform;

        InputIndicator indicator;
        if (_indicatorPrefab != null)
        {
            indicator = Instantiate(_indicatorPrefab, parent);
            indicator.name = $"Indicator_{key}";
        }
        else
        {
            var go = new GameObject($"Indicator_{key}");
            go.transform.SetParent(parent);
            indicator = go.AddComponent<InputIndicator>();
        }

        indicator.Initialize(key, noteSprite);
        _indicators.Add(indicator);
        return indicator;
    }

    // Adds a beat to the map at runtime — the UI will auto-spawn a note for it.
    public BeatNote AddBeatNote(int indicatorIndex, float hitTime, bool isSpecial = false)
    {
        var beat = new BeatNote
        {
            IndicatorIndex = indicatorIndex,
            HitTime = hitTime,
            IsSpecial = isSpecial
        };

        _beatMap.Add(beat);
        OnBeatAdded?.Invoke(beat);
        return beat;
    }

    // Call this from any external script to spawn an attack.
    // indicatorIndex: which lane (0-based), -1 = random lane.
    // leadTime: seconds until the note reaches the hit zone.
    public BeatNote SpawnAttack(int indicatorIndex = -1, float leadTime = 2f, bool isSpecial = false)
    {
        if (_indicators.Count == 0)
        {
            Debug.LogWarning("[Attack] No indicators set up — cannot spawn attack");
            return null;
        }

        if (indicatorIndex < 0 || indicatorIndex >= _indicators.Count)
            indicatorIndex = UnityEngine.Random.Range(0, _indicators.Count);

        float hitTime = _songTime + leadTime;
        BeatNote beat = AddBeatNote(indicatorIndex, hitTime, isSpecial);

        if (_debugLogHits)
            Debug.Log($"[Attack] Spawned attack on lane {indicatorIndex} [{_indicators[indicatorIndex].InputKey}], " +
                      $"hits at {hitTime:F2}s (lead {leadTime:F1}s)");

        return beat;
    }

    public void StartPlaying()
    {
        _isPlaying = true;
        _songTime = 0f;
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }

    // Public API for any script that wants to change runtime debug settings.
    public void SetDebugLogHits(bool enabled)
    {
        DebugOptions.SetDebugLogHits(enabled);
    }

    public void SetManualSpawnEnabled(bool enabled)
    {
        DebugOptions.SetManualSpawnEnabled(enabled);
    }

    public void SetPerfectWindow(float value)
    {
        DebugOptions.SetPerfectWindow(value);
    }

    private static bool IsCtrlHeld()
    {
        return Keyboard.current != null &&
               (Keyboard.current[Key.LeftCtrl].isPressed ||
                Keyboard.current[Key.RightCtrl].isPressed);
    }

    private static bool IsShiftHeld()
    {
        return Keyboard.current != null &&
               (Keyboard.current[Key.LeftShift].isPressed ||
                Keyboard.current[Key.RightShift].isPressed);
    }

    private void HandleDebugOptionsChanged()
    {
        SyncDebugFlagsFromGlobal();
    }

    private void SyncDebugFlagsFromGlobal()
    {
        _debugLogHits = DebugOptions.DebugLogHits;
        _enableManualSpawn = DebugOptions.EnableManualSpawn;
        _perfectWindow = DebugOptions.PerfectWindow;
    }

    // Hold Ctrl/Shift + press an indicator key to place beats during play (for testing).
    private void CheckManualSpawn(bool isSpecial)
    {
        for (int i = 0; i < _indicators.Count; i++)
        {
            if (!_indicators[i].WasPressed) continue;

            float hitTime = _songTime + _manualSpawnLeadTime;
            AddBeatNote(i, hitTime, isSpecial);

            string type = isSpecial ? "Special" : "Normal";
            Debug.Log($"[Rhythm] Manual spawn ({type}): Indicator {i} [{_indicators[i].InputKey}] at {hitTime:F2}s");
        }
    }

    // Matches each key press to the closest upcoming input within the perfect window.
    private void CheckPlayerInput()
    {
        for (int i = 0; i < _indicators.Count; i++)
        {
            if (!_indicators[i].WasPressed) continue;

            BeatNote closestBeat = FindClosestActiveBeat(i);

            if (closestBeat != null && IsWithinPerfectWindow(closestBeat))
            {
                closestBeat.IsConsumed = true;
                HandleResult(i, HitResult.Perfect);
                OnBeatConsumed?.Invoke(closestBeat, HitResult.Perfect);
            }
            else
            {
                HandleResult(i, HitResult.Fail);
            }
        }
    }

    // Marks beats as failed once they pass outside the hit window.
    private void CheckForMissedBeats()
    {
        foreach (var beat in _beatMap)
        {
            if (beat.IsConsumed) continue;
            if (_songTime <= beat.HitTime + _perfectWindow) continue;

            beat.IsConsumed = true;
            HandleResult(beat.IndicatorIndex, HitResult.Fail);
            OnBeatConsumed?.Invoke(beat, HitResult.Fail);
        }
    }

    private BeatNote FindClosestActiveBeat(int indicatorIndex)
    {
        BeatNote closest = null;
        float closestDistance = float.MaxValue;

        foreach (var beat in _beatMap)
        {
            if (beat.IsConsumed || beat.IndicatorIndex != indicatorIndex) continue;

            float distance = Mathf.Abs(_songTime - beat.HitTime);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = beat;
            }
        }

        return closest;
    }

    private bool IsWithinPerfectWindow(BeatNote beat)
    {
        return Mathf.Abs(_songTime - beat.HitTime) <= _perfectWindow;
    }

    // Sends "success" or "fail" to the Player's HandleAttackResult method, then destroys the Attack object.
    private bool HandleResult(int indicatorIndex, HitResult result)
    {
        bool success = result == HitResult.Perfect;
        string outcome = success ? "success" : "fail";

        if (_debugLogHits)
        {
            string keyName = indicatorIndex < _indicators.Count
                ? _indicators[indicatorIndex].InputKey.ToString()
                : "Unknown";

            Debug.Log($"[Attack] Indicator {indicatorIndex} [{keyName}] outcome: {outcome} at {_songTime:F2}s");
        }

        if (_player != null)
        {
            _player.SendMessage("HandleAttackResult", outcome, SendMessageOptions.DontRequireReceiver);

            if (_debugLogHits)
                Debug.Log($"[Attack] Sent \"{outcome}\" to Player ({_player.name})");
        }
        else if (_debugLogHits)
        {
            Debug.LogWarning("[Attack] No Player assigned — result not delivered");
        }

        OnHitResult?.Invoke(indicatorIndex, result);
        return success;
    }

    // Spawns arrow key indicators if none are assigned in the Inspector.
    private void CreateDefaultIndicators()
    {
        Key[] defaultKeys = { Key.UpArrow, Key.DownArrow, Key.LeftArrow, Key.RightArrow };

        foreach (var key in defaultKeys)
        {
            AddIndicator(key);
        }
    }

    // Fills the beat map with random test beats for quick debugging.
    private void GenerateTestBeats()
    {
        _beatMap.Clear();
        float startDelay = 2f;

        for (int i = 0; i < _testBeatCount; i++)
        {
            _beatMap.Add(new BeatNote
            {
                IndicatorIndex = UnityEngine.Random.Range(0, Mathf.Max(1, _indicators.Count)),
                HitTime = startDelay + i * _testBeatInterval
            });
        }
    }
}
