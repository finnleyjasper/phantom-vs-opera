## Spawning Attacks (External Scripts)
Get a reference to `InputIndicatorManager` and call `SpawnAttack`:
```csharp
manager.SpawnAttack();                    // random lane, 2s lead time
manager.SpawnAttack(1);                   // lane 1, 2s lead time
manager.SpawnAttack(0, 3f);              // lane 0, 3s lead time
manager.SpawnAttack(-1, 2f, true);       // random lane, special attack
```
- `indicatorIndex`: which lane (0-based). Pass -1 for a random lane.
- `leadTime`: seconds until the note reaches the hit zone.
- `isSpecial`: marks the note as a special variant.

Returns the `BeatNote` if you need to track it.

## Manual Spawn (Debug Only)
- **Ctrl + indicator key** → spawns a normal input.
- **Shift + indicator key** → spawns a special input.

## Player Integration
On success or fail, the manager calls `HandleAttackResult("success")` or `HandleAttackResult("fail")` on the assigned Player GameObject via `SendMessage`. The Attack object (note visual) is destroyed automatically after the result is sent.

Your Player script just needs a public method:
```csharp
public void HandleAttackResult(string outcome) { }
```

## Console Logs (enable Debug Log Hits)
- `[Attack] Indicator X [Key] outcome: success/fail` — confirms each result.
- `[Attack] Sent "success"/"fail" to Player` — confirms delivery to Player.
- `[Attack] No Player assigned` — warning if Player field is empty.
