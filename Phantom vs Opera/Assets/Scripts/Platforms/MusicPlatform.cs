using UnityEngine;

public class MusicPlatform : MonoBehaviour
{
    [Header("Music Properties")]
    [Range(0, 127)]
    public int pitch = 1;          // Pitch
    [Min(0.1f)]
    public float length = 1f;   // Duration

    [HideInInspector] public int laneIndex = -1;
    [HideInInspector] public string noteName;

    private void Start()
    {
        ApplyLengthScale();
    }

    void Update()
    {
        // Move platform left
        float speed = PlatformManager.Instance.GetSpeed();
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void ApplyLengthScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = length * PlatformManager.Instance.platformLengthMultiplier; // base length from MIDI is a bit short, so multiply it
        transform.localScale = scale;
    }

    private void OnEnable()
    {
        PlatformManager.Instance.RegisterPlatform(this);
    }

    private void OnDestroy()
    {
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.UnregisterPlatform(this);
    }
}
