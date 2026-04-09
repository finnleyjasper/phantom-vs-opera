using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private bool _isOnPlatform; // Bool variable for if Player touches platform
        private bool _fellOnFloor; // Bool variable for if Player touches ground

    // References to Player Ground
        [Header("Player Ground")]
        [SerializeField] private Transform _playerGround;
        [SerializeField] private float _playerGroundRadius = 0.1f;

    // Initialize values
    void Start()
    {
        _isAlive = true;
        _hasWon = false;
        _isOnPlatform = false;
        _fellOnFloor = false;

    }

    private void FixedUpdate()
    {
        PlatformCollision();
    }

    // Method for detecting if Player is on a platform
    // Creates sphere below Player and detects for Platform tag
    void PlatformCollision()
    {
        _isOnPlatform = false;

        Collider[] gameColliders = Physics.OverlapSphere(_playerGround.position, _playerGroundRadius);

        foreach (var gameCollider in gameColliders)
        {
            if (gameCollider.gameObject.tag == "Platform")
            {
                _isOnPlatform = true;
                Debug.Log("Collision! " + _isOnPlatform);
                break;
            }
        }
    }

    // Method for detecting when Player falls on floor
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _fellOnFloor = true;
        }
    }

    // Method to detect if the Player is no longer on the floor
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _fellOnFloor = false;
        }
    }

    // Properties
    public bool IsAlive
    {
        get { return _isAlive; }
    }

    public bool HasWon
    {
        get { return _hasWon; }
    }

    public bool IsOnPlatform
    {
        get { return _isOnPlatform; }
    }

    public bool FellOnFloor
    {
        get { return _fellOnFloor; }
    }
}
