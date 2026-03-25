using UnityEngine;

public class MusicPlatform : MonoBehaviour
{
    [Header("Music Properties")]
    [Range(1, 10)]
    public int tone = 1;          // Pitch (1 = low, 10 = high)

    [Min(0.1f)]
    public float strength = 1f;   // Duration (affects platform length)

    private void Start()
    {
        ApplyToneHeight();
        ApplyStrengthScale();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayTone("Player");
        }

        // Falling attack also triggers tone
        if (collision.gameObject.CompareTag("FallingAttack"))
        {
            PlayTone("Falling Attack");
        }
    }

    void PlayTone( string source)
    {
        Debug.Log($"Platform {gameObject.name} played a {tone} tone triggered by {source}");
    }

    void ApplyToneHeight()
    {
        float y = ToneMapper.GetYPosition(tone);

        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    void ApplyStrengthScale()
    {
        Vector3 scale = transform.localScale;
        scale.x = strength; // change axis if needed
        transform.localScale = scale;
    }
}