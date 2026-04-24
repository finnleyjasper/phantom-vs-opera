using UnityEngine;

[DefaultExecutionOrder(-20)]
public class Player : PausableObject
{
    // Private Variables
        private bool _isOnPlatform;
        private bool _fellOnFloor;
        private Vector3 _startPosition;
        private Transform _currentPlatform;


        private PlayerController _playerController;
        public Transform CurrentPlatform => _currentPlatform;

    // References to Player Ground
        [Header("Player Ground")]
        [SerializeField] private Transform _playerGround;
        [SerializeField] private float _playerGroundRadius = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        _startPosition = transform.position;
        Reset();
    }

    void Update()
    {
        if (_fellOnFloor)
        {
            GameManager.Instance.HandlePlayerFall();
            _fellOnFloor = false; // prevent spam
        }
    }

    private void FixedUpdate()
    {
        PlatformCollision();
    }

    // Creates sphere below Player and detects for Platform tag
    void PlatformCollision()
    {
        _isOnPlatform = false;
        _currentPlatform = null;

        Collider[] gameColliders = Physics.OverlapSphere(_playerGround.position, _playerGroundRadius);

        foreach (var gameCollider in gameColliders)
        {
            if (gameCollider.gameObject.tag == "Platform")
            {
                _isOnPlatform = true;
                _currentPlatform = gameCollider.transform;
//                Debug.Log("Collision! " + _isOnPlatform); // Debug
                break;
            }
        }
    }

    public void Reset()
    {
        _isOnPlatform = false;
        _fellOnFloor = false;
        Pause(false);

        if (this.Rigidbody != null)
        {
            this.Rigidbody.linearVelocity = Vector3.zero;
            this.Rigidbody.angularVelocity = Vector3.zero;
        }

        transform.SetPositionAndRotation(_startPosition, Quaternion.Euler(0f, 0f, 0f)); // reset player position and rotation
    }

    public override void Pause(bool shouldPause)
    {
        base.Pause(shouldPause);

        if (_playerController != null)
        {
            _playerController.enabled = !shouldPause; // disable when paused, enable when unpaused
            
            if (shouldPause)
            {
                _playerController.StopSlam();
            }
        }
    }

    // Player falls on floor
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Debug.Log("HIT FLOOR");
            _fellOnFloor = true;
            if (GameObserver.Instance != null)
                GameObserver.Instance.OnPlayerTouchedFloorForAudience();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _fellOnFloor = false;
        }
    }

    public bool IsOnPlatform => _isOnPlatform;

    public bool FellOnFloor => _fellOnFloor;

    public Vector3 StartPosition => _startPosition;
}
