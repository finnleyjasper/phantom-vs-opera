using UnityEngine;

public class Player : MonoBehaviour
{
    // Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private int _healthBar;
        private float _successBar;
        private bool _isOnPlatform = false;

    // Reference to PlayerBarUI Script 
    [Header("Player Health Bar UI")]
    [SerializeField] private PlayerBarUI playerHealthBarUI;

    [Header("Player Success Bar UI")]
    [SerializeField] private PlayerBarUI playerSuccessBarUI;

    // Set up Initial health/success levels in Start
    void Start()
    {
        _healthBar = 10;
        _successBar = 0;
        _isAlive = true;
        _hasWon = false;

        Debug.Log("Initial health: " + _healthBar);
        Debug.Log("Initial success: " + _successBar);

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
        get { return  IsOnPlatform; }
    }     

    // Method to Manage Health Bar - sets initial health bar level, sets results for losing all health (i.e. losing game)
    public void ManagePlayerLose()
    {
        if (_healthBar <= 0)
        {
            _isAlive = false;
            Debug.Log("isAlive status: " + _isAlive);
            Debug.Log("health bar: " + _healthBar + " Game over !");
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    // Method to Manage Success Bar - sets initial success bar level, sets results for reaching certain success level (i.e. winning game)
    public void ManagePlayerWin()
    {
        if (_successBar >= 10)
        {
            _hasWon = true;
            Debug.Log("hasWon status: " + _hasWon + " You won !");
            Debug.Log("success bar: " + _successBar);
            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }
    }

    // Method for when Falling Object hits Player = health decreases - method is called by falling objects

    public void IsHit(int damage)
    {
        _healthBar -= damage; 
        _healthBar = Mathf.Clamp(_healthBar, 0, 10); // Clamp - health bars cannot go below 0 or above 10
        Debug.Log("health bar: " + _healthBar);
     
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
    }

    void OnCollisionEnter(Collision platformCollision)
    {
        if (platformCollision.gameObject.tag == "Platform") // Change so it's just on top, just touching sides 
        {
            Debug.Log("Collision!"); // Change this - Delete
            _isOnPlatform = true;
        }


    }
}

/***
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