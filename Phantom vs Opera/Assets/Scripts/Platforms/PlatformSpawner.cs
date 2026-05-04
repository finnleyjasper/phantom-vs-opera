using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    /// <summary>Fired after each music platform is instantiated and configured (lane, pitch, etc.).</summary>
    public static event System.Action<MusicPlatform> OnPlatformSpawned;

    public bool _debugMode = false;

    [Header("References")]
    public GameObject platformPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;
    [SerializeField] private float _platformYPosition = 0f;

    [Header("Lane Management")]
    [Tooltip("Number of MIDI pitches at the low and high ends that will be clamped to the first and last lanes")] public int deadZone = 30;
    public List<Transform> laneTransforms = new List<Transform>();

    [Header("Lane appearance")]
    [Tooltip("If any entry is set, platforms use these materials by lane index (cycles if there are fewer materials than lanes).")]
    [SerializeField] private Material[] _laneMaterials;
    [Tooltip("Per-lane tint when not using materials, or if a material slot is null. Cycles if shorter than lane count.")]
    [SerializeField] private Color[] _laneColors;

    private List<MusicPlatform> activePlatforms = new List<MusicPlatform>();
    private List<NoteData> notes = new List<NoteData>(); // retrieved from MIDIFacade

    void Awake()
    {
        // find lanes
        GameObject laneContainer = GameObject.Find("Lanes");
        if (laneContainer == null)
        {
            Debug.LogError("Lane container not found! Please create a GameObject named 'Lanes'");
        }
        else
        {
            Transform[] lanes = laneContainer.GetComponentsInChildren<Transform>();

            foreach (Transform lane in lanes)
            {
                if (lane.gameObject.name != "Lanes") // skip the parent container
                laneTransforms.Add(lane.transform);
            }
        }
    }
    public void StartSpawning()
    {
        notes = MIDIFacade.Instance.GetNoteData(PlatformManager.Instance.TravelTime); // with travel time between spawn & player to account for spawn lead
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        // Check active platforms for despawn
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            if (activePlatforms[i] == null) continue;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        foreach (var note in notes)
        {
            while (GameManager.Instance.GameTime < note.spawnTime) // wait for note to play
            {
                yield return null;
            }

            // spwawn only for active track OR if track == 0, spawn all
            if (note.track == GameManager.Instance.CurrentTrack || GameManager.Instance.CurrentTrack == 0) // spwawn only for active track
            {
                SpawnNote(note);
            }
        }
    }

    public void SpawnNote(NoteData note)
    {
        if (spawnPoint == null || laneTransforms.Count == 0 || platformPrefab == null)
        {
            Debug.LogWarning("SpawnPoint, LaneTransforms, or PlatformPrefab not assigned!");
            return;
        }

        int laneIndex = Mathf.RoundToInt(GetLaneIndex(note.pitch));

        // Determine spawn position w/ GetLaneIndex() based on note pitch
        Vector3 spawnPos = new Vector3(
            spawnPoint.position.x,
            _platformYPosition,
            laneTransforms[laneIndex].position.z
        );

        // Instantiate platform
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        // Set platform properties
        MusicPlatform mp = platform.GetComponent<MusicPlatform>();
        if (mp != null)
        {
            mp.pitch = note.pitch;
            mp.length = note.duration;
            mp.laneIndex = laneIndex;
            mp.noteName = note.noteName;
        }

        // Track active platform for despawning
        activePlatforms.Add(mp);

        if (mp != null)
        {
            ApplyLaneAppearance(mp);
            OnPlatformSpawned?.Invoke(mp);
        }

        if (_debugMode){ Debug.Log($"Spawned platform for note {note.noteName} (Pitch: {note.pitch}) in lane {laneIndex+1}"); }
    }

    private void ApplyLaneAppearance(MusicPlatform mp)
    {
        if (laneTransforms.Count == 0) return;

        int lane = Mathf.Clamp(mp.laneIndex, 0, laneTransforms.Count - 1);

        if (_laneMaterials != null && _laneMaterials.Length > 0)
        {
            Material mat = _laneMaterials[lane % _laneMaterials.Length];
            if (mat != null)
            {
                mp.ApplyLaneMaterial(mat);
                return;
            }
        }

        mp.ApplyLaneColor(ResolveLaneColor(lane, laneTransforms.Count));
    }

    public Color GetLaneColor(int laneIndex)
    {
        int laneCount = laneTransforms.Count;
        if (laneCount == 0)
            return Color.white;

        int lane = Mathf.Clamp(laneIndex, 0, laneCount - 1);

        if (_laneMaterials != null && _laneMaterials.Length > 0)
        {
            Material mat = _laneMaterials[lane % _laneMaterials.Length];
            if (TryGetMaterialColor(mat, out Color materialColor))
                return materialColor;
        }

        return ResolveLaneColor(lane, laneCount);
    }

    private static bool TryGetMaterialColor(Material material, out Color color)
    {
        color = Color.white;

        if (material == null)
            return false;

        if (material.HasProperty("_BaseColor"))
        {
            color = material.GetColor("_BaseColor");
            return true;
        }

        if (material.HasProperty("_Color"))
        {
            color = material.GetColor("_Color");
            return true;
        }

        return false;
    }

    private Color ResolveLaneColor(int laneIndex, int laneCount)
    {
        if (_laneColors != null && _laneColors.Length > 0)
            return _laneColors[laneIndex % _laneColors.Length];

        float h = laneCount <= 1 ? 0f : laneIndex / (float)(laneCount - 1);
        return Color.HSVToRGB(h, 0.55f, 0.92f);
    }

    private float GetLaneIndex(int pitch)
    {
        int effectiveMin = MIDIFacade.Instance.MinPitch + deadZone;
        int effectiveMax = MIDIFacade.Instance.MaxPitch - deadZone;

        // clamp pitch to effective range
        int clampedPitch = Mathf.Clamp(pitch, effectiveMin, effectiveMax);

        // normalize and map to lane index
        float t = (float)(clampedPitch - effectiveMin) / (effectiveMax - effectiveMin);
        float laneIndex = t * (laneTransforms.Count - 1);

        return laneIndex;
    }
}
