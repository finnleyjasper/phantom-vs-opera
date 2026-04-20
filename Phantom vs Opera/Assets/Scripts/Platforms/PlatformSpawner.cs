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

    [Header("Lane Management")]
    [Tooltip("Number of MIDI pitches at the low and high ends that will be clamped to the first and last lanes")] public int deadZone = 30;
    public List<Transform> laneTransforms = new List<Transform>();

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
                Debug.Log("Note spawned at: " + note.spawnTime + " and GameTime: " + GameManager.Instance.GameTime);
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
            0f,
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
            OnPlatformSpawned?.Invoke(mp);

        if (_debugMode){ Debug.Log($"Spawned platform for note {note.noteName} (Pitch: {note.pitch}) in lane {laneIndex+1}"); }
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
