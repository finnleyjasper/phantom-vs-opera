using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    // Private Variables 
        [Header("Audience Support Max)")]
        [SerializeField] private float _audienceSupportMax = 10; // Stores Maximum Audience Support Value 
        private float _audienceSupportValue; // Stores Audience Support value 

        [Header("Rate Audience Support Decreases (seconds)")]
        [SerializeField] private float _rateBySecond = 1.0f;

    // Reference to Objects 
        [SerializeField] private Player player; 
        [SerializeField] private PlayerBarUI playerBarUI;

    // Initialize audience support value with max audience support value
    void Start()
    {
        _audienceSupportValue = _audienceSupportMax;
    }

    void Update()
    {
        ManageAudienceSupport();
        playerBarUI.UpdateAudienceSupportUI();
    }

    public void ManageAudienceSupport() 
    {
        if (player.IsOnPlatform == true)
        {
            _audienceSupportValue++;
            _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, _audienceSupportMax); // Clamp - audienceSupport bar cannot go below 0 or above 10
            playerBarUI.UpdateAudienceSupportUI();
        }

        else
        {
            _audienceSupportValue -= Time.fixedDeltaTime * _rateBySecond;
            _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, _audienceSupportMax); // Clamp - audienceSupport bar cannot go below 0 or above 10
        }

        if (player.FellOnFloor == true)
        {
            _audienceSupportValue = 0;
        }
    }

    // Properties 
    public float AudienceSupportValue
    {
        get { return _audienceSupportValue; }
    }

    public float AudienceSupportMax
    {
        get { return _audienceSupportMax; }
    }
}
