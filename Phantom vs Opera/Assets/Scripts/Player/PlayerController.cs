using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for Player Right + Left Movement 
    //    private float _xMovement = 0; // dont know if still need these two - Delete 
    //    private float _zMovement = 0; // dont know if still need these two - Delete 

    [Header("Lanes")] // is this necessary? - delete 
    [SerializeField] private int currentLaneIndex = 3; // change so it automatically finds the lane index - delete
        private Vector3[] lanePositions; // is this correct - also do i add .normalized to the end ? - also can i just add this to the list above instead of having two separate lists? - delete 
        private Vector3 targetLanePosition; // is this correct - delete
        private int minLaneIndex;
        private int maxLaneIndex;  //try to find better names for all these private variables - delete     

        private float lerpTimer = 0;
        private float lerpSpeed = 15f;

    // Variables for Editable KeyCodes 
    [Space(10)]
    [Header("Z Axis Movements")]
    // [SerializeField] private KeyCode _xLeftKeyCode = KeyCode.LeftArrow; // dont know if still need this - Delete 
    // [SerializeField] private KeyCode _xRightKeyCode = KeyCode.RightArrow; // dont know if still need this - Delete 
    [SerializeField] private KeyCode _zFrontKeyCode = KeyCode.UpArrow;
    [SerializeField] private KeyCode _zBackKeyCode = KeyCode.DownArrow;

    [Space(10)]
    [Header("Jump Keycode")]
    [SerializeField] private KeyCode _jumpKeyCode = KeyCode.Space;

    // Player's speed + force
    [Space(10)]
    [Header("Speed")]
    [SerializeField] private float _playerSpeed = 5f;

    [Space(10)]
    [Header("Speed")]
    [SerializeField] private float _playerForce = 300f;

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
                Debug.Log(lane.transform.position); // debug - delete 
            }
        }

            // Sort list along Z axis
            lanePositionList.Sort((a, b) => a.z.CompareTo(b.z)); // double check if any other way to do this - delete 

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
        if (Input.GetKeyDown(_zFrontKeyCode)) // double check this is for when key = pressed NOT held down - delete
        {
            currentLaneIndex++; // dont know if this is right - delete 
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex]; // ist his right? - delete
            Debug.Log("Lane index: " + currentLaneIndex); // delete
        }

        if (Input.GetKeyDown(_zBackKeyCode))
        {
            currentLaneIndex--; // dont know what to put here - delete
            LaneIndexClamp();
            targetLanePosition = lanePositions[currentLaneIndex]; // ist his right? - delete
            Debug.Log("Lane index: " + currentLaneIndex); // delete
        }
    }

    public void LerpLaneMovement()
    {
        Vector3 currentPlayerPosition = _player_RigidBody.position; //player's current position 

        // Check if another way to do this, while loop - delete 

            Vector3 lerpedPosition = Vector3.Lerp(currentPlayerPosition, targetLanePosition, lerpSpeed * Time.fixedDeltaTime); 

        _player_RigidBody.MovePosition(lerpedPosition); // is this ok - delete
    }

    public void PlayerJump()
    {
        if (Input.GetKeyDown(_jumpKeyCode))
        {
            _player_RigidBody.AddForce(Vector3.up  * _playerForce);
        }
    }

    void Start()
    {
        _player_RigidBody = GetComponent<Rigidbody>(); // Get Player's RigidBody Component
        FindLanePositions();
        targetLanePosition = lanePositions[currentLaneIndex]; // Get Player's current lane position


    }

    void Update()
    {
        PlayerInput();
        PlayerJump();
    }

    void FixedUpdate()
    {
        LerpLaneMovement();
        // PlayerMove(); // Remove eventually - delete
    }
}