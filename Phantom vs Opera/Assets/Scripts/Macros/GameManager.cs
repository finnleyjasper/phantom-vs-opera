using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Over engineered atm but will be useful later...?
    public enum GameState
    {
        Menu,
        Play,
        Win,
        Lose
    }

    [Header("Game Settings")]
    public string EndSceneName;
    public string MainMenuSceneName;

    // Current state of the game - further functionality within GameManager will be based on this state
    // Should be changed within this script via methods called by other objects
    [SerializeField] private GameState _currentGameState = GameState.Menu; // starts in main menu i assume

    [HideInInspector] public static GameManager Instance;

    // Variables for Game Length 
    [SerializeField] private float _gameLength = 10f; // Stores overall game length 
    private float _gameTimer = 0f; // Stores time that passes

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

    }
    public void SetGameState(GameState newState)
    {
        _currentGameState = newState;
        Debug.Log("Game State changed to: " + _currentGameState);
    }

    // Switches scene based on result - called from player
    public void GameOver(GameState result)
    {
        SetGameState(result);

        if (string.IsNullOrEmpty(EndSceneName))
        {
            Debug.LogWarning("Scene name is null or empty");
        }
        else
        {
            SceneManager.LoadScene(EndSceneName);
        }
    }
    void Update()
    {
        if (_currentGameState != GameState.Play)
        {
            _gameTimer = 0; // Restart timer when game state is in lose/win/menu
            return;
        }
        
        _gameTimer += Time.deltaTime;
    }

    // Properties
    public GameState CurrentGameState
    {
        get { return _currentGameState;}
    }

    public float GameLength
    {
        get {  return _gameLength;}
    }

    public float GameTimer
    {
        get { return _gameTimer; }
    }
}
