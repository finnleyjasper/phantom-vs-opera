using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // # DELETE ALL - "DELETE"
    // # Also refine comments - DELETE

        private bool _isAlive; //idk what type - DELETE
        private bool _hasWon; //idk what type - DELETE
        private int _healthBar;
        private int _successBar;
        private string _sceneGameOver = "Game Over";
        private string _sceneMainMenu = "Main Menu";

    //Set up Initial health/success levels
    void Start()
    {
        _healthBar = 10;
        _successBar = 0;
        _isAlive = true;
        _hasWon = false;

        Debug.Log("Initial health: " + _healthBar);
        Debug.Log("Initial success: " + _successBar);
    }

    public bool IsAlive //dk what type - DELETE
    {
        get { return _isAlive; }
    }

    public bool HasWon //dk what type - DELETE
    {
        get { return _hasWon; }
    }

    //Method to Load GameOver Scene 
    public void LoadSceneGameOver()
    {
        if (string.IsNullOrEmpty(_sceneGameOver))
        {
            Debug.LogWarning("Scene name is null or empty");
        }
        else
        {
            SceneManager.LoadScene(_sceneGameOver);
        }
    }

    //Method to Load MainMenu Scene 
    public void LoadSceneMainMenu()
    {
        if (string.IsNullOrEmpty(_sceneMainMenu))
        {
            Debug.LogWarning("Scene name is null or empty");
        }
        else
        {
            SceneManager.LoadScene(_sceneMainMenu);
        }
    }

    //Method to Manage Health Bar - sets initial health bar level, sets results for losing all health (i.e. losing game)
    public void ManageHealthBar()
    {
        if (_healthBar <= 0)
        {
            _isAlive = false;
            Debug.Log("isAlive status: " + _isAlive);
            Debug.Log("health bar: " + _healthBar + " Game over !");
            LoadSceneGameOver();
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
            LoadSceneMainMenu();
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
            _successBar = Mathf.Max(0, _successBar); //Success bar cannot go below 0

            _successBar --;
            _healthBar --;
            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        ManageHealthBar();
        ManageSuccessBar();
    }
}

///Player should have isAlive & hasWon properties GameManager can check based on number of success/losses
///Player should include a method called by the Attack object to TAKE damage on a fail, or BUILD success master on success
///When this method is called, Player should check if it meets conditions to change isAlive or hasWon
///Will take in (string success/fail) as a perm and react accordingly
///Console or onscreen result should be included when method is called so user can see outcome (or a bar if you're feeling fancy)