using System.Collections.Generic;
using UnityEngine;

public class OLDPlayerController : MonoBehaviour
{
    // Variables for Player Right + Left Movement
        private float _xMovement = 0;
        private float _zMovement = 0;

    // Variables for Editable KeyCodes
    [Header("Left & Right Movement")]
    [SerializeField] private KeyCode _xLeftKeyCode = KeyCode.LeftArrow;
    [SerializeField] private KeyCode _xRightKeyCode = KeyCode.RightArrow;
    [SerializeField] private KeyCode _zFrontKeyCode = KeyCode.UpArrow;
    [SerializeField] private KeyCode _zBackKeyCode = KeyCode.DownArrow;

    [Header("Jump Keycode")]
    [SerializeField] private KeyCode _jumpKeyCode = KeyCode.Space;


    // Player's speed + force
    [Header("Speed")]
    [SerializeField] private float _playerSpeed = 5f;

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

    private void PlayerInput()
    {
        _xMovement = 0;
        _zMovement = 0;

        if (Input.GetKey(_xLeftKeyCode))
        {
            _xMovement--;
        }

        if (Input.GetKey(_xRightKeyCode))
        {
            _xMovement++;
        }

        if (Input.GetKey(_zFrontKeyCode))
        {
            _zMovement++;
        }

        if (Input.GetKey(_zBackKeyCode))
        {
            _zMovement--;
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
