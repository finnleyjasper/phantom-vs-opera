using UnityEngine;

public class AudienceBean : MonoBehaviour
{
    private Vector3 _basePosition;
    private float _phaseOffset;
    private AudienceJumpSettings _settings;

    public void Initialize(Vector3 basePosition, float phaseOffset, AudienceJumpSettings settings)
    {
        _basePosition = basePosition;
        _phaseOffset = phaseOffset;
        _settings = settings;
    }

    private void Update()
    {
        float intensity = GetComboIntensity();
        float height = Mathf.Min(
            _settings.MaxJumpHeight,
            _settings.IdleJumpHeight + (intensity * _settings.JumpHeightPerComboLevel));
        float speed = Mathf.Min(
            _settings.MaxBounceSpeed,
            _settings.BaseBounceSpeed + (intensity * _settings.BounceSpeedPerComboLevel));

        float bounce = (1f + Mathf.Sin((Time.time + _phaseOffset) * speed)) * 0.5f;
        transform.position = _basePosition + (Vector3.up * (bounce * height));
    }

    private static float GetComboIntensity()
    {
        if (GameObserver.Instance == null || GameManager.Instance == null)
            return 0f;

        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play)
            return 0f;

        int landings = GameObserver.Instance.ConsecutivePlatformLandings;
        int startsAt = GameManager.Instance.ComboStartsAtConsecutiveLandings;
        if (landings < startsAt)
            return 0f;

        return landings - startsAt + 1;
    }
}

public struct AudienceJumpSettings
{
    public float IdleJumpHeight;
    public float JumpHeightPerComboLevel;
    public float MaxJumpHeight;
    public float BaseBounceSpeed;
    public float BounceSpeedPerComboLevel;
    public float MaxBounceSpeed;
}
