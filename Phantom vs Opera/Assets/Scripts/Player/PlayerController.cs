using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Private variables for Keyboard Inputs 
    [Header("Left & Right Movement")]
    [SerializeField] private float _xMovement;
    [SerializeField] private float _zMovement;


    //Player's speed 
    [Header("Speed")]
    [SerializeField] float _playerSpeed = 0.1f;

    //Varibale to get Player's RigidBody Component
    private Rigidbody _player_RigidBody;

    void Start()
    {
        _player_RigidBody = GetComponent<Rigidbody>();
    }

    void Update() // need this or? - Delete
    {
        //Receive position input from Player - is this correct ??? - Delete
        float _xMovement = Input.GetAxis("Horizontal"); //Assigns ad key or left/right arrow key
        float _zMovement = Input.GetAxis("Vertical"); //Assigns ws or up/down arrow key 

        Vector3 playerMove = new Vector3(_xMovement, 0.0f, _zMovement);

        transform.Translate(playerMove * _playerSpeed);
    }
}
