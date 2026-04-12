[System.Serializable]
public class NoteData
{
    public int pitch;        // pitch 0–127 (determines lane)
    public float noteOn;      // start time (x position)
    public float duration;  // how long (platform length)
    public float instrument; // based on MIDI channel

    // unused data for now - but added from the MIDI parser in case we want to use it later
    public float noteOff; // time + duration
    public int strength; // how hard the note is played
    public string noteName; // ie. C# etc.

    public float spawnTime; // when the platform should spawn, calculated from noteOn and lead from PlatformSpawner

    public NoteData(int pitch, float instrument, float noteOn, float duration, float noteOff, int strength, string noteName, float spawnLead)
    {
        this.pitch = pitch;
        this.instrument = instrument;
        this.noteOn = noteOn;
        this.duration = duration;
        this.noteOff = noteOff;
        this.strength = strength;
        this.noteName = noteName;
        if (this.noteOn - spawnLead > 0)
            this.spawnTime = this.noteOn - spawnLead;
        else
            this.spawnTime = 0;
    }
}

