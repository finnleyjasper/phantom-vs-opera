using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
        public bool debugMode = false; // show logs

    // Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private int _healthBar;
        private float _successBar;
        private bool _isOnPlatform;
        private bool _fellOnFloor; // dont know if we need this ? Or if I should make this a property ? - Delete

    // Reference to PlayerBarUI Script
    [Header("Player Health Bar UI")]
    [SerializeField] private PlayerBarUI playerHealthBarUI;

    [Header("Player Success Bar UI")]
    [SerializeField] private PlayerBarUI playerSuccessBarUI;

    // References to Player Ground 
    [Header("Player Ground")]
    [SerializeField] private Transform _playerGround; // Should this be Transform type ? - Delete
    [SerializeField] private float _playerGroundRadius = 0.1f;

    //Reference to Platform 
    [Header("Platform")]
    [SerializeField] private Transform _playerPlatform; // Delete if not using the platform collision parent logic 

    // Set up Initial health/success levels in Start
    void Start()
    {
        _healthBar = 10;
        _successBar = 0;
        _isAlive = true;
        _hasWon = false;
        _isOnPlatform = false;
        _fellOnFloor = false;

        if (debugMode)
        {
            Debug.Log("Initial health: " + _healthBar);
            Debug.Log("Initial success: " + _successBar);
        }

        playerHealthBarUI.UpdatePlayerHealthUI();
        playerSuccessBarUI.UpdatePlayerSuccessUI();
    }

    // Properties

    public int HealthBar
    {
        get { return _healthBar; }
    }

    public float SuccessBar
    {
        get { return _successBar; }
    }

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
        get { return  _isOnPlatform; }
    }

    public bool FellOnFloor
    {
        get { return _fellOnFloor; }
    }

    // Method to Manage Health Bar - sets initial health bar level, sets results for losing all health (i.e. losing game)
    public void ManagePlayerLose()
    {
        if (_healthBar <= 0)
        {
            _isAlive = false;

            if (debugMode)
            {
                Debug.Log("isAlive status: " + _isAlive);
                Debug.Log("health bar: " + _healthBar + " Game over !");
            }

            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    // Method to Manage Success Bar - sets initial success bar level, sets results for reaching certain success level (i.e. winning game)
    public void ManagePlayerWin()
    {
        if (_successBar >= 10)
        {
            _hasWon = true;

            if (debugMode)
            {
                Debug.Log("hasWon status: " + _hasWon + " You won !");
                Debug.Log("success bar: " + _successBar);
            }

            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }
    }

    // Method for when Falling Object hits Player = health decreases - method is called by falling objects

    public void IsHit(int damage)
    {
        _healthBar -= damage;
        _healthBar = Mathf.Clamp(_healthBar, 0, 10); // Clamp - health bars cannot go below 0 or above 10

        if (debugMode)
        {
            Debug.Log("health bar: " + _healthBar);
        }

        if (_healthBar <= 0)
        {
            ManagePlayerLose();
        }

        playerHealthBarUI.UpdatePlayerHealthUI();
    }

    // Method for when Player wins if game time ends
    public void PlayerSuccessTimer()
    {
        _successBar = GameManager.Instance.GameTimer;
        _successBar = Mathf.Clamp(_successBar, 0, 10); // Clamps - succes + health bars cannot go below 0
        playerSuccessBarUI.UpdatePlayerSuccessUI();

        // Calls win condition if game length is reached
        if (_successBar >= GameManager.Instance.GameLength)
        {
            ManagePlayerWin();
        }
    }

    void Update()
    {
        PlayerSuccessTimer();
        PlatformCollision(); // Does this go here ?
    }

    // Method for detecting if Player is on a platform
    // Creates sphere below Player and detects for Platform tag
    void PlatformCollision()
    {
        _isOnPlatform = false;

        Collider[] platformColliders = Physics.OverlapSphere(_playerGround.position, _playerGroundRadius);  // Do I need to delete this ... ? or alter ? to ray cast ?

        foreach (var platformCollider in platformColliders) // Not sure what else to call this besides 'platformCollider' - cause they're technically not P.Colliders ? - Delete
        {
            if (platformCollider.gameObject.tag == "Platform") // Is this too Nested ?? - Delete
            {
                _isOnPlatform = true;
                Debug.Log("Collision! " + _isOnPlatform); // Change this after - Delete
                break; // Dont know if i need this or why - Delete
            }
        }
    }

    // Method for detecting when Player falls on floor 
    void OnCollisionEnter(Collision collision) // Fix so that when game first plays, player doesnt trigger "fell"  
    {   
        // Player collision detection for floor 
        if (collision.gameObject.tag == "Floor") // Dont know if 'else if' works here? - Delete
        {
            _fellOnFloor = true;
            Debug.Log("Player fell! " + _fellOnFloor); // Change this - Delete
        }

        // Do we still want this to make player stay?  
        /***
        // Moves Player while platform is moving  
        else if (collision.gameObject.tag == "Platform") // Dont know if 'else if' should be uesd, or else
        {
            Debug.Log("Player is moving w/ Platform !"); // Change this - Delete
            collision.gameObject.transform.parent = _playerPlatform;
        }
        ***/
    }


    // LOOK THRU CODE AND DELETE ANYTHING UNECESSARY EHRE - Delete 
    // Also maybe not put things in 'start' for no reason idk?? SHould i have put them there? Also anythig unecessary in update ? - Delete
}

/***
 Delete
Audience Support Plan : 

- See Research Current - 15 mins
- Research how to do collision detection - 20 min

**Collision Implementation ; 1 hour 
- In player: Create collision detection for when player is ON TOP of platform - make bool, can use this for when player is/isnt on platform - needs to konw the # of time the player isn't on a platform somehow
- In player: Create collision detection for when player touches ground floor - use this for flat decrease in AS
- AS uses these 2 collision detects (turn them into properties) 

- Delete Success + Health related methods here 
- Delete success + health variables 
- Keep IsAlive + HasWon

- Create new script for AudienceSupport - let GameObserver watch it for 0/max stats, then calls GM's methods for win/lose
- Observer then uses the audience support property to call GM for win/lose conditions

- Change UI accordingly = 1 hour :
    - Create AS bar in Unity 
    - Copy UI format here to AS
    - Fix + test 

***/