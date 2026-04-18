[System.Serializable]
public class NoteData
{
    public int pitch;        // pitch 0–127 (determines lane)
    public float noteOn;      // start time (x position)
    public float duration;  // how long (platform length)
    public int track; // based on MIDI track

    // unused data for now - but added from the MIDI parser in case we want to use it later
    public float noteOff; // time + duration
    public int strength; // how hard the note is played
    public string noteName; // ie. C# etc.

    public float spawnTime; // when the platform should spawn, calculated from noteOn and lead from PlatformSpawner

    public NoteData(int pitch, int track, float noteOn, float duration, float noteOff, int strength, string noteName, float spawnLead)
    {
        this.pitch = pitch;
        this.track = track;
        this.noteOn = noteOn;
        this.duration = duration;
        this.noteOff = noteOff;
        this.strength = strength;
        this.noteName = noteName;
        this.spawnTime = this.noteOn - spawnLead;
    }
}
