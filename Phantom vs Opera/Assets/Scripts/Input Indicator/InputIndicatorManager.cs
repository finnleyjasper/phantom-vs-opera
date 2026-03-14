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

    // Creates a new indicator at runtime and parents it under this object.
    public InputIndicator AddIndicator(Key key, Sprite noteSprite = null)
    {
        var go = new GameObject($"Indicator_{key}");
        go.transform.SetParent(transform);
        var indicator = go.AddComponent<InputIndicator>();
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

    public void StartPlaying()
    {
        _isPlaying = true;
        _songTime = 0f;
    }

    public void StopPlaying()
    {
        _isPlaying = false;
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

    // Returns true for Perfect, false for Fail. Enable _debugLogHits in the Inspector to print results.
    private bool HandleResult(int indicatorIndex, HitResult result)
    {
        bool success = result == HitResult.Perfect;

        if (_debugLogHits)
        {
            string keyName = indicatorIndex < _indicators.Count
                ? _indicators[indicatorIndex].InputKey.ToString()
                : "Unknown";

            if (success)
                Debug.Log($"<color=green>PERFECT!</color> Indicator {indicatorIndex} [{keyName}] at {_songTime:F2}s");
            else
                Debug.Log($"<color=red>FAIL!</color> Indicator {indicatorIndex} [{keyName}] at {_songTime:F2}s");
        }

        OnHitResult?.Invoke(indicatorIndex, result);
        return success;
    }

    // Spawns D, F, J, K indicators if none are assigned in the Inspector.
    private void CreateDefaultIndicators()
    {
        Key[] defaultKeys = { Key.D, Key.F, Key.J, Key.K };

        foreach (var key in defaultKeys)
        {
            var go = new GameObject($"Indicator_{key}");
            go.transform.SetParent(transform);
            var indicator = go.AddComponent<InputIndicator>();
            indicator.Initialize(key);
            _indicators.Add(indicator);
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
