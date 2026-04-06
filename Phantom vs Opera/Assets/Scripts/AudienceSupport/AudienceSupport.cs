using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    // Private Variables 
        [Header("Audience Support Max)")]
        [SerializeField] private float _audienceSupportMax = 10; // this and audience suppor value are the same - dk if i can make this more efficient - Delete 
        private float _audienceSupportValue; //don't know if this should be the value - Delete

        [Header("Rate Audience Support Decreases (seconds)")]
        [SerializeField] private float _rateBySecond = 1.0f;

    // Reference to Objects 
        [SerializeField] private Player player; // or should this be connect thru GameObserver ? - Delete
        [SerializeField] private PlayerBarUI playerBarUI; 

    public void ManageAudienceSupport() // Does it need to be void? Delete
    {
        if (player.IsOnPlatform == true)
        {
            _audienceSupportValue++;
            _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, _audienceSupportMax); // Clamp - audienceSupport bar cannot go below 0 or above 10
            playerBarUI.UpdateAudienceSupportUI();
            Debug.Log("Audience support (+) : " + _audienceSupportValue + player.IsOnPlatform + player.FellOnFloor); // Added debug - delete later ? - Delete
        }

        else
        {
            _audienceSupportValue -= Time.fixedDeltaTime * _rateBySecond;
            _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, _audienceSupportMax); // Clamp - audienceSupport bar cannot go below 0 or above 10
            Debug.Log("Audience support (-) : " + _audienceSupportValue + player.IsOnPlatform + player.FellOnFloor); // Added debug - delete later ? - Delete
        }

        if (player.FellOnFloor == true)
        {
            _audienceSupportValue = 0;
            Debug.Log("PlayerFall() active"); // Delete Debug log 
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

    void Start() //Idk if i need this - Delete
    {
        _audienceSupportValue = _audienceSupportMax;
    }

    void Update() //Idk if i need this - Delete
    {
        ManageAudienceSupport();
        playerBarUI.UpdateAudienceSupportUI();
    }

    private void FixedUpdate()
    {
      //  ManageAudienceSupport(); // idk if should keep this here ... ? Delete
       // playerBarUI.UpdateAudienceSupportUI();

    }
}
