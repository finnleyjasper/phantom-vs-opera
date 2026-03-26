[System.Serializable]
public class NoteData
{
    public int tone;        // pitch (1–10)
    public float time;      // when it happens (x position)
    public float duration;  // how long (platform length)

    public NoteData(int tone, float time, float duration)
    {
        this.tone = tone;
        this.time = time;
        this.duration = duration;
    }
}