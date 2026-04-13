using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    private float _audienceSupportValue;

    [SerializeField] private bool _debugMode = false;

    void Start()
    {
        _audienceSupportValue = GameManager.Instance.StartingAudienceSupport;
    }


    public void ManageAudienceSupport(float supportChange)
    {
        _audienceSupportValue += supportChange;
        ClampAudienceSupport();
        if (_debugMode) { Debug.Log($"[Audience Support] New value: {_audienceSupportValue}"); }
    }

    private void ClampAudienceSupport()
    {
        _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, GameManager.Instance.MaxAudienceSupport);
    }

    public float AudienceSupportValue =>_audienceSupportValue;
}
