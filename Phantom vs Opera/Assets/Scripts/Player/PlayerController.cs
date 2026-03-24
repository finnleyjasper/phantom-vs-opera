using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Private variables for Keyboard Inputs 
    [Header("Left & Right Movement")]
    [SerializeField] private float _xMovement;
    [SerializeField] private float _zMovement;

    [Header("Up & Down Movement")]
    [SerializeField] private float _yMovement;

    //Player's speed 
    [Header("Speed")]
    [SerializeField] float _playerSpeed = 0.1f;

    void Update() // need this or? - Delete
    {
        //Receive position input from Player - is this correct ??? - Delete
        float _xMovement = Input.GetAxis("Horizontal");
        float _zMovement = Input.GetAxis("Vertical");
        float _yMovement = Input.GetAxis("Vertical"); //DONT KNWO WHAT TO NAME - delete

        Vector3 playerMove = new Vector3(_xMovement, _yMovement, _zMovement);

        transform.position += playerMove * _playerSpeed;
    }
}
