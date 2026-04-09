using UnityEngine;

public class AudienceSupport : MonoBehaviour
{
    private float _audienceSupportValue;
    [SerializeField] private Player player; // REFACTOR why should audiencesupport have a player? feels yucky

    [HideInInspector] public static AudienceSupport Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

    }

    // Initialize with half max value??
    void Start()
    {
        _audienceSupportValue = GameManager.Instance.MaxAudienceSupport / 2;
    }

    void Update()
    {
        ManageAudienceSupport();
    }

    public void ManageAudienceSupport()
    {
        if (player.IsOnPlatform == true)
        {
            _audienceSupportValue++;
            ClampAudienceSupport();
        }

        else
        {
            _audienceSupportValue -= Time.fixedDeltaTime * GameManager.Instance.IncreasePerSecond; // Decrease audience support value by 1 every second when player is not on platform
            ClampAudienceSupport();
        }

        if (player.FellOnFloor == true)
        {
            _audienceSupportValue -= GameManager.Instance.FallenPunishment;
            ClampAudienceSupport();
        }
    }

    private void ClampAudienceSupport()
    {
        _audienceSupportValue = Mathf.Clamp(_audienceSupportValue, 0, GameManager.Instance.MaxAudienceSupport);
    }

    public float AudienceSupportValue =>_audienceSupportValue;
}
