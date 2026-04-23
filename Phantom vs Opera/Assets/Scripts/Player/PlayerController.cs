using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for Player Right + Left Movement 
    //    private float _xMovement = 0; // dont know if still need these two - Delete 
    //    private float _zMovement = 0; // dont know if still need these two - Delete 

    [Header("Lanes")] // is this necessary? - delete 
    [SerializeField] private int currentLaneIndex = 3; // change so it automatically finds the lane index - Can just get the position of player from start? - delete 
        private Vector3[] lanePositions; // is this correct - also do i add .normalized to the end ? - also can i just add this to the list above instead of having two separate lists? - delete 
        private Vector3 targetLanePosition; // is this correct - delete
        private Player _player;
        private int minLaneIndex;
        private int maxLaneIndex;  //try to think of better names for all these private variables - delete     

        private float lerpTimer = 0;
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

    // Player's speed + force
    [Space(10)]
    [Header("Speed")]
    [SerializeField] private float _playerSpeed = 5f; // can we delete ? - delete


    // Varibale to get Player's RigidBody Component
        private Rigidbody _player_RigidBody;
    
    /*** Delete eventually
    public void PlayerMove() // do I delete this ??  wbt the _player_RigidBody part ? - delete
    {
        Vector3 playerMove = new Vector3(_xMovement, 0.0f, _zMovement).normalized;

        //transform.Translate(playerMove * _playerSpeed); replced it with rb.MovePosition to avoid physics problems
        _player_RigidBody.MovePosition(_player_RigidBody.position + playerMove * _playerSpeed * Time.fixedDeltaTime);
    }
    ***/

    public void StopSlam()
    {
        _isSlamming = false;
    }
    
    // Method to get Lane Positions 
    public void FindLanePositions() // find better name - delete
    {
        // finds + stores all GameObjects with tag "lane"
        GameObject[] lanes = GameObject.FindGameObjectsWithTag("Lane");

        List<Vector3> lanePositionList = new List<Vector3>(); // temporary list to store lane positions in

        // adds lane positions to Vector3 list 
        foreach (GameObject lane in lanes)
        {
            if (lane != null) // necessary ? - delete 
            {
                lanePositionList.Add(lane.transform.position); // wanna check that it's ading the ones closest to left to right ?? double check it's in that progressive order - make sure it's sorted correctly - delete
                Debug.Log($"Adding lane at: {lane.transform.position}"); // debug - delete 
            }
        }

            // Sort list along Z axis
            lanePositionList.Sort((a, b) => a.z.CompareTo(b.z)); // double check if any better way to do this - delete 

            // Convert temporary list to Array 
            lanePositions = lanePositionList.ToArray();
            
            // Assign min/max lane index 
            minLaneIndex = 0;
            maxLaneIndex = lanePositions.Length - 1;
    }
    
    private void LaneIndexClamp()
    {
        currentLaneIndex = Mathf.Clamp(currentLaneIndex, minLaneIndex, maxLaneIndex);
    }

    private void PlayerInput()
    {        
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Play)
        {
            return;
        }

        if (Input.GetKeyDown(_zFrontKeyCode)) // double check this is for when key = pressed NOT held down - delete
        {
            currentLaneIndex++; // double check if this worked - delete 
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex]; // ist his right? - delete
            Debug.Log($"Current Lane: {currentLaneIndex}, {lanePositions[currentLaneIndex]}, Target Position: {targetLanePosition}"); // delete
        }

        if (Input.GetKeyDown(_zBackKeyCode))
        {
            currentLaneIndex--; // dont know what to put here - delete
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex]; // ist his right? - delete
            Debug.Log($"Current Lane: {currentLaneIndex}, {lanePositions[currentLaneIndex]}, Target Position: {targetLanePosition}"); // delete
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

        // Check if another way to do this, while loop - delete 

            Vector3 lerpedPosition = Vector3.Lerp(currentPlayerPosition, targetLanePosition, lerpSpeed * Time.fixedDeltaTime); 

        _player_RigidBody.MovePosition(lerpedPosition); // is this ok - delete
    }

    private void HandleVerticalMovement()
    {
        Vector3 pos = _player_RigidBody.position;

        // start riding
        if (_isSlamming && !_isRidingPlatform && _player.IsOnPlatform)
        {
            _isRidingPlatform = true;
            _currentPlatform = _player.CurrentPlatform;
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


    void Start()
    {
        _player_RigidBody = GetComponent<Rigidbody>(); // Get Player's RigidBody Component
        _player = GetComponent<Player>();
        FindLanePositions();
        targetLanePosition = lanePositions[currentLaneIndex]; // Get Player's current lane position


    }

    void Update()
    {
        PlayerInput(); // should I move to FixedUP - delete
    }

    void FixedUpdate()
    {
        LerpLaneMovement();
        HandleVerticalMovement();
    }
}