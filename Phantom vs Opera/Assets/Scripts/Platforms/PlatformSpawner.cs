using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject platformPrefab;

    [Header("Note Data")]
    public List<NoteData> notes = new List<NoteData>();

    private void Start()
    {
        SpawnAllNotes();
    }

    public void SpawnAllNotes()
    {
        foreach (var note in notes)
        {
            SpawnNote(note);
        }
    }

    public void SpawnNote(NoteData note)
    {
        float x = note.time;
        float y = ToneMapper.GetYPosition(note.tone);

        GameObject platform = Instantiate(
            platformPrefab,
            new Vector3(x, y, 0),
            Quaternion.identity
        );

        MusicPlatform mp = platform.GetComponent<MusicPlatform>();
        mp.tone = note.tone;
        mp.strength = note.duration;
    }
}