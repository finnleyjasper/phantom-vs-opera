[System.Serializable]
public class NoteData
{
    public int pitch;        // pitch (1–10)
    public float time;      // when it happens (x position)
    public float duration;  // how long (platform length)

    // things added from finn
    public double timeInstantiaed;

    public NoteData(int pitch, float time, float duration)
    {
        this.pitch = pitch;
        this.time = time;
        this.duration = duration;
    }
}
