using UnityEngine;

public class Player : MonoBehaviour
{
    // Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private int _healthBar;
        private float _successBar;

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

    //Method called by Attack object - on fail = takes damage, on success = builds success
    //Can I delete this method ? - Delete
    public void HandleAttackResult(string outcome)
    {
        if (outcome == "success")
        {
            _successBar++;
            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        else
        {
            _healthBar--;

            // Clamps - succes + health bars cannot go below 0
            _successBar = Mathf.Clamp(_successBar, 0, 10);
            _healthBar = Mathf.Clamp(_healthBar, 0, 10);

            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        ManagePlayerLose();
        ManagePlayerWin();
        playerHealthBarUI.UpdatePlayerHealthUI();
        playerSuccessBarUI.UpdatePlayerSuccessUI();

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

    /* 
     
     * To Do : 
     * **Update Player to integrate changes made in Issues #16 + #17 - So, when they are finished w/ their issues, come back and make needed changes  :
     * Issue 17 (Falling Attacks) - On collision w/ Player = call isHit(int Damage) method within Player :
     * - IsHit
     * - Falling Attacks will likely detect the collision w/ player and then call IsHit - so make sure ur player is able to be detectable (idk if u need this)

     * Issue 16 (Platforms) -
     * Player can jump / walk / run / etc on platforms properly - make sure collider works 
     
     * Left + Right Movement - (Use basic keyboard input)
     * Jump + Land on Platforms - (Use basic keyboard input) 
     * Be able to jump down from platforms - (Use basic keyboard input) - might need to incorporate collision (so player knows 'can i jump down from this?')
     
     * Health = decrease when a falling object hits Player
     * Player should call GameManager's GameOver(GameState Lose) when this occurs (ie. game over - player loses)
     
     * //Success = increase over time
     * //Player should call GameManager's GameOver(GameState Win) when success bar is full (ie. game over - player wins)
     * //Perhaps the length of the game (how long the player needs to suvive) is a property in GameManager, changable in the editor
      
     * //Implement IsHit(int Damage) method - falling attacks will call if one collides w/ the player 
     * //Should cause Player health to decrease & check for Game Over condiiton (if Player is dead)
     
     * If time : 
     * Slide mechanic 
     * run option 
    */
}

