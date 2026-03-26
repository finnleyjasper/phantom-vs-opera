using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    [SerializeField] private float _playerSpeed = 0.05f;

    [Header("Speed")]
    [SerializeField] private float _playerForce = 300f;

    // Varibale to get Player's RigidBody Component
        private Rigidbody _player_RigidBody;

    public void PlayerMove()
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

        Vector3 playerMove = new Vector3(_xMovement, 0.0f, _zMovement);

        transform.Translate(playerMove * _playerSpeed);
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
        PlayerMove();
        PlayerJump();
    }
}