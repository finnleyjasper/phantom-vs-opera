using UnityEngine;

public class MusicPlatform : MonoBehaviour
{
    [Header("Music Properties")]
    [Range(1, 10)]
    public int tone = 1;          // Pitch
    [Min(0.1f)]
    public float strength = 1f;   // Duration

    private void Start()
    {
        ApplyStrengthScale();
    }

    void Update()
    {
        // Move platform left
        float speed = PlatformManager.Instance.GetSpeed();
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void ApplyStrengthScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = strength;
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