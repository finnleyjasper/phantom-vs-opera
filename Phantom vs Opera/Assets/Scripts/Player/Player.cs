using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Private Variables 
        private bool _isAlive; 
        private bool _hasWon; 
        private int _healthBar;
        private int _successBar;
        private string _sceneGameOver = "Game Over";
        private string _sceneMainMenu = "Main Menu";

    //Set up Initial health/success levels in Start
    void Start()
    {
        _healthBar = 10;
        _successBar = 0;
        _isAlive = true;
        _hasWon = false;

        Debug.Log("Initial health: " + _healthBar);
        Debug.Log("Initial success: " + _successBar);
    }

    //Properties
    public bool IsAlive 
    {
        get { return _isAlive; }
    }

    public bool HasWon 
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
            _successBar --;
            _healthBar --;

            //Clamps - succes + health bars cannot go below 0
            _successBar = Mathf.Max(0, _successBar);
            _healthBar = Mathf.Max(0, _healthBar);

            Debug.Log("success bar: " + _successBar);
            Debug.Log("health bar: " + _healthBar);
        }

        ManageHealthBar();
        ManageSuccessBar();
    }
}

