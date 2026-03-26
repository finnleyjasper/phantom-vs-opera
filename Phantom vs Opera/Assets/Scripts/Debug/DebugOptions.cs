using System;
using UnityEngine;

public static class DebugOptions
{
    private static bool _initialized;
    private static bool _timingDefaultsInitialized;

    public static bool DebugLogHits { get; private set; }
    public static bool EnableManualSpawn { get; private set; }
    public static float PerfectWindow { get; private set; } = 0.15f;
    public static float NoteSpeed { get; private set; } = 0.5f;

    public static event Action OnOptionsChanged;

    public static void Initialize(bool debugLogHitsDefault, bool manualSpawnDefault)
    {
        if (_initialized) return;

        DebugLogHits = debugLogHitsDefault;
        EnableManualSpawn = manualSpawnDefault;
        _initialized = true;
    }

    public static void InitializeTimingDefaults(float perfectWindowDefault, float noteSpeedDefault)
    {
        if (_timingDefaultsInitialized) return;

        PerfectWindow = Mathf.Clamp(perfectWindowDefault, 0.01f, 1f);
        NoteSpeed = Mathf.Clamp(noteSpeedDefault, 0.01f, 5f);
        _timingDefaultsInitialized = true;
    }

    public static void SetDebugLogHits(bool enabled)
    {
        if (DebugLogHits == enabled) return;

        DebugLogHits = enabled;
        OnOptionsChanged?.Invoke();
    }

    public static void SetManualSpawnEnabled(bool enabled)
    {
        if (EnableManualSpawn == enabled) return;

        EnableManualSpawn = enabled;
        OnOptionsChanged?.Invoke();
    }

    public static void SetPerfectWindow(float value)
    {
        value = Mathf.Clamp(value, 0.01f, 1f);
        if (Mathf.Approximately(PerfectWindow, value)) return;

        PerfectWindow = value;
        OnOptionsChanged?.Invoke();
    }

    public static void SetNoteSpeed(float value)
    {
        value = Mathf.Clamp(value, 0.01f, 5f);
        if (Mathf.Approximately(NoteSpeed, value)) return;

        NoteSpeed = value;
        OnOptionsChanged?.Invoke();
    }
}
