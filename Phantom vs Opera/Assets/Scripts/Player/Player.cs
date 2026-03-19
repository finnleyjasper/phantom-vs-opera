using UnityEngine;

public class Player : MonoBehaviour
{
    //Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private int _healthBar;
        private int _successBar;

    //Reference to PlayerBarUI Script 
    [Header("Player Health Bar UI")]
    [SerializeField] private PlayerBarUI playerHealthBarUI;

    [Header("Player Success Bar UI")]
    [SerializeField] private PlayerBarUI playerSuccessBarUI;


    //Set up Initial health/success levels in Start
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

    //Properties

    public int HealthBar
    {
        get { return _healthBar; }
    }

    public int SuccessBar
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

    //Method to Manage Health Bar - sets initial health bar level, sets results for losing all health (i.e. losing game)
    public void ManageHealthBar()
    {
        if (_healthBar <= 0)
        {
            _isAlive = false;
            Debug.Log("isAlive status: " + _isAlive);
            Debug.Log("health bar: " + _healthBar + " Game over !");
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    //Method to Manage Success Bar - sets initial success bar level, sets results for reaching certain success level (i.e. winning game)
    public void ManageSuccessBar()
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
    public void HandleAttackResult(string outcome)
    {
        if (outcome == "success")
        {
            _successBar ++;
            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        else
        {
            _healthBar --;

            //Clamps - succes + health bars cannot go below 0
            _successBar = Mathf.Clamp(_successBar, 0, 10);
            _healthBar = Mathf.Clamp(_healthBar, 0, 10);

            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        ManageHealthBar();
        ManageSuccessBar();
        playerHealthBarUI.UpdatePlayerHealthUI();
        playerSuccessBarUI.UpdatePlayerSuccessUI();

    }
}

