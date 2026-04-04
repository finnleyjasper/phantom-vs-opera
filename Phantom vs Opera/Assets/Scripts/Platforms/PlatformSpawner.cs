using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject platformPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;


    [Header("Lane Management")]
    public List<Transform> laneTransforms = new List<Transform>();
    public int minpitch = 0;
    public int maxpitch = 127;

    private List<MusicPlatform> activePlatforms = new List<MusicPlatform>();
    private List<NoteData> notes = new List<NoteData>(); // retrieved from MIDIFacade

    public void StartSpawning()
    {
        notes = MIDIFacade.Instance.GetNoteData(PlatformManager.Instance.spawnDelay);
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
            // wait until the audio time reaches this note's spawn time
            while (MIDIFacade.Instance.GetAudioSourceTime() < note.spawnTime)
            {
                yield return null; // Wait one frame
            }

            Debug.Log($"Spawning note {note.noteName} at audio time {MIDIFacade.Instance.GetAudioSourceTime()} (noteOn: {note.noteOn})");
            SpawnNote(note);
        }
    }

    public void SpawnNote(NoteData note)
    {
        if (spawnPoint == null || laneTransforms.Count == 0 || platformPrefab == null)
        {
            Debug.LogWarning("SpawnPoint, LaneTransforms, or PlatformPrefab not assigned!");
            return;
        }

        // Map pitch to lane index
        float t = (float)(note.pitch - minpitch) / (maxpitch - minpitch);
        int index = Mathf.RoundToInt(t * (laneTransforms.Count - 1));
        index = Mathf.Clamp(index, 0, laneTransforms.Count - 1);

        // Determine spawn position
        Vector3 spawnPos = new Vector3(
            spawnPoint.position.x,
            0f,
            laneTransforms[index].position.z
        );

        // Instantiate platform
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        // Set platform properties
        MusicPlatform mp = platform.GetComponent<MusicPlatform>();
        if (mp != null)
        {
            mp.pitch = note.pitch;
            mp.length = note.duration;
        }

        // Track active platform for despawning
        activePlatforms.Add(mp);

        Debug.Log($"Spawned platform for note {note.noteName} (Pitch: {note.pitch}) in lane {index}");
    }
}
