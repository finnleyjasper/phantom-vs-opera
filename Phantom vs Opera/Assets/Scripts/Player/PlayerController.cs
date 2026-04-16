using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for Player Right + Left Movement 
    //    private float _xMovement = 0; // dont know if still need these two - Delete 
    //    private float _zMovement = 0; // dont know if still need these two - Delete 

    [Header("Lanes")] // is this necessary? - delete 
    private int currentLane;
    private int minLaneIndex;
    private int maxLaneIndex;
    private int[] lanePositions; //try to find better names for all these private variables - delete
     

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

    public void PlayerMove()
    {
        Vector3 playerMove = new Vector3(_xMovement, 0.0f, _zMovement).normalized;

        //transform.Translate(playerMove * _playerSpeed); replced it with rb.MovePosition to avoid physics problems
        _player_RigidBody.MovePosition(_player_RigidBody.position + playerMove * _playerSpeed * Time.fixedDeltaTime);
    }

    public void Lane() // find better name - delete
    {
        
    }
    
    public void LaneChange()
    {
        // lane change logic 
    }

    private void PlayerInput()
    {
       // _xMovement = 0; - delete ?
       // _zMovement = 0; - delete ?

        /*** - delete ?
        if (Input.GetKey(_xLeftKeyCode))
        {
            _xMovement--;
        }

        if (Input.GetKey(_xRightKeyCode))
        {
            _xMovement++;
        }

        ***/

        if (Input.GetKeyDown(_zFrontKeyCode)) // double check this is for when key = pressed NOT held down - delete
        {
            // lane change++;  // dont know what to put here - delete
        }

        if (Input.GetKeyDown(_zBackKeyCode))
        {
            // lane change--; // dont know what to put here - delete
        }
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
    }

    void Update()
    {
        PlayerInput();
        PlayerJump();
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
}