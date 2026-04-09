using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    private float _audienceSupportValue;

    void Start()
    {
        _audienceSupportValue = GameManager.Instance.StartingAudienceSupport;
    }


    public void ManageAudienceSupport(float supportChange)
    {
        _audienceSupportValue += supportChange;
        ClampAudienceSupport();
    }

    private void ClampAudienceSupport()
    {
        _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, GameManager.Instance.MaxAudienceSupport);
    }

    public float AudienceSupportValue =>_audienceSupportValue;
}
