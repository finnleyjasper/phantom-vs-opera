using System;

// Central place for runtime debug flags.
//
// How to add a new option:
// 1) Add a public read-only property:
//      public static bool GodMode { get; private set; }
//
// 2) Add a setter method that raises OnOptionsChanged when the value changes:
//      public static void SetGodMode(bool enabled)
//      {
//          if (GodMode == enabled) return;
//          GodMode = enabled;
//          OnOptionsChanged?.Invoke();
//      }
//
// 3) (Optional) Initialize defaults in Initialize(...), OR just rely on the default 'false'.
//
// 4) Add a toggle row in DebugMenu.DrawWindow():
//      bool godMode = GUILayout.Toggle(DebugOptions.GodMode, "God Mode", ...);
//      if (godMode != DebugOptions.GodMode) DebugOptions.SetGodMode(godMode);
//
// 5) In your gameplay scripts, read the value:
//      if (DebugOptions.GodMode) { ... }
public static class DebugOptions
{
    private static bool _initialized;

    public static bool DebugLogHits { get; private set; }
    public static bool EnableManualSpawn { get; private set; }

    public static event Action OnOptionsChanged;

    public static void Initialize(bool debugLogHitsDefault, bool manualSpawnDefault)
    {
        if (_initialized) return;

        DebugLogHits = debugLogHitsDefault;
        EnableManualSpawn = manualSpawnDefault;
        _initialized = true;
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
}
