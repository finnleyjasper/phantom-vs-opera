using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
        private int currentLaneIndex = 0;
        private Vector3[] lanePositions;
        private Vector3 targetLanePosition;
        private Player _player;
        private int minLaneIndex;
        private int maxLaneIndex;
        private Rigidbody _player_RigidBody; // Variable to get Player's RigidBody Component

        private float lerpSpeed = 15f;
        private bool _isSlamming;
        private bool _isRidingPlatform;
        private Transform _currentPlatform;


    // Variables for Editable KeyCodes
    [Space(10)]
    [Header("Z Axis Movements")]
    [SerializeField] private KeyCode _zFrontKeyCode = KeyCode.UpArrow;
    [SerializeField] private KeyCode _zBackKeyCode = KeyCode.DownArrow;

    [Space(10)]
    [Header("Slamming")]
    [SerializeField] private float _slamSpeed = 25f;
    [SerializeField] private float _returnSpeed = 15f;

    // Variable for Audio Source 
    [Space(10)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource playerAudioSource;

    void Awake()
    {
        if (playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>(); // gets AudioSource comp.

            if (playerAudioSource == null)
            {
                Debug.LogWarning("AudioManager could not find an audio source");
                return;
            }
        }
    }

    void Start()
    {
        _player_RigidBody = GetComponent<Rigidbody>(); // Get Player's RigidBody Component
        _player = GetComponent<Player>();
        SearchLanePositions();
    }

    void Update()
    {
        PlayerInput();
    }

    void FixedUpdate()
    {
        LerpLaneMovement();
        HandleVerticalMovement();
    }


    public void StopSlam()
    {
        _isSlamming = false;
    }

    // Method to get Lane Positions
    public void SearchLanePositions()
    {
        GameObject[] lanes = GameObject.FindGameObjectsWithTag("Lane");

        List<Vector3> lanePositionList = new List<Vector3>(); // temporary list to store lane positions in

        // adds lane positions to Vector3 list
        foreach (GameObject lane in lanes)
        {
            if (lane != null)
            {
                lanePositionList.Add(lane.transform.position);
            }
        }

        // Sort list from lowest to highest Z axis value
        lanePositionList.Sort((a, b) => a.z.CompareTo(b.z));

        // Convert temporary list to Array
        lanePositions = lanePositionList.ToArray();

        // Assign min/max lane index
        minLaneIndex = 0;
        maxLaneIndex = lanePositions.Length - 1;

        FindInitialLaneIndex();
    }

    // Method to find initial lane index
    public void FindInitialLaneIndex()
    {
        Vector3 currentPlayerPosition = _player_RigidBody.position; // gets player's current position
        float closestLaneIndex = float.MaxValue; // sets max float distance for if condition below

        for (int i = 0; i < lanePositions.Length; i++)
        {
            float LaneDistance = Vector3.Distance(currentPlayerPosition, lanePositions[i]); // calculates distance between player's currernt position + lane's position

            // if current lane is closer than previous, updates the closestLaneIndex
            if (LaneDistance < closestLaneIndex)
            {
                closestLaneIndex = LaneDistance;
                currentLaneIndex = i;
                targetLanePosition = lanePositions[currentLaneIndex]; // Sets Player's current lane position
            }
        }
    }

    private void LaneIndexClamp()
    {
        currentLaneIndex = Mathf.Clamp(currentLaneIndex, minLaneIndex, maxLaneIndex);
    }

    // Method to Move Player via Keycodes
    private void PlayerInput()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play)
        {
            return;
        }

        if (Input.GetKeyDown(_zFrontKeyCode))
        {
            currentLaneIndex++;
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex];
        }

        if (Input.GetKeyDown(_zBackKeyCode))
        {
            currentLaneIndex--;
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex];
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isSlamming = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isSlamming = false;
        }
    }

    public void LerpLaneMovement()
    {
        Vector3 currentPlayerPosition = _player_RigidBody.position; //player's current position

        Vector3 lerpedPosition = Vector3.Lerp(currentPlayerPosition, targetLanePosition, lerpSpeed * Time.fixedDeltaTime);
        _player_RigidBody.MovePosition(lerpedPosition);
    }

    private void HandleVerticalMovement()
    {
        Vector3 pos = _player_RigidBody.position;

        // start riding
        if (_isSlamming && !_isRidingPlatform && _player.IsOnPlatform)
        {
            _isRidingPlatform = true;
            _currentPlatform = _player.CurrentPlatform;

            ParticleFactory.Instance.CreateParticleSystem("Riding", _player.PlayerGround.position);
            AudioManager.Instance.PlaySoundEffect("applause", playerAudioSource); // Play SFX
        }

        //  riding platform
        if (_isRidingPlatform)
        {
            // If no longer actually on a platform fall
            if (!_player.IsOnPlatform)
            {
                _isRidingPlatform = false;
                _currentPlatform = null;
            }
            else
            {
                // Stick to platform
                Vector3 platformPos = _currentPlatform.position;

                pos.y = platformPos.y + 2.6f;
                pos.x = _player_RigidBody.position.x; // keep X fixed
                pos.z = platformPos.z;

                // Let go → stop riding
                if (!_isSlamming)
                {
                    _isRidingPlatform = false;
                    _currentPlatform = null;
                }

                _player_RigidBody.MovePosition(pos);
                return;
            }
        }

        // slamming down
        if (_isSlamming)
        {
            pos.y -= _slamSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // return to start height when not slamming
            float targetY = _player.StartPosition.y;
            pos.y = Mathf.Lerp(pos.y, targetY, _returnSpeed * Time.fixedDeltaTime);
        }

        _player_RigidBody.MovePosition(pos);
    }

    // Properties 
    public bool IsSlamming => _isSlamming;
}
