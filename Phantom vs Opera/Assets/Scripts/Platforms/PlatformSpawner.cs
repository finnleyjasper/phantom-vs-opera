using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject platformPrefab;

    [Header("Note Data")]
    public List<NoteData> notes = new List<NoteData>();

    [Header("Spawn Point")]
    public Transform spawnPoint;


    [Header("Lane Management")]
    public List<Transform> laneTransforms = new List<Transform>();
    public int minTone = 1;
    public int maxTone = 10;

    private List<MusicPlatform> activePlatforms = new List<MusicPlatform>();

    private void Start()
    {
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
        float previousTime = 0f;

        foreach (var note in notes)
        {
            float waitTime = note.time - previousTime;
            yield return new WaitForSeconds(waitTime);

            SpawnNote(note);
            previousTime = note.time;
        }
    }

    public void SpawnNote(NoteData note)
    {
        if (spawnPoint == null || laneTransforms.Count == 0 || platformPrefab == null)
        {
            Debug.LogWarning("SpawnPoint, LaneTransforms, or PlatformPrefab not assigned!");
            return;
        }

        // Map tone to lane index
        float t = (float)(note.tone - minTone) / (maxTone - minTone);
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
            mp.tone = note.tone;
            mp.strength = note.duration;
        }

        // Track active platform for despawning
        activePlatforms.Add(mp);
    }
}