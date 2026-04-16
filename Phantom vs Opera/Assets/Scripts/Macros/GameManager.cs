using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [SerializeField] [Tooltip("Delay before the level starts after loading")]private float _levelStartDelay = 2f;
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

    // Current state of the game - further functionality within GameManager will be based on this state
    // Should be changed within this script via methods called by other objects
    [SerializeField] private GameState _currentGameState = GameState.Play; // As we don't have a main menu yet, I temporarily changed starting state to Play

    [HideInInspector] public static GameManager Instance;

    // Variables for Game Length
    [SerializeField] private float _gameLength = 10f; // Stores overall game length
    private float _gameTimer = 0f; // Stores time that passes
    private bool _isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;

    }

    public void StartGame() // should get called from menu buttons
    {
        StartCoroutine(StartLevelAfterDelay());
    }

    private IEnumerator StartLevelAfterDelay() // give some time between loading the scene and starting the level
    {
        yield return new WaitForSeconds(_levelStartDelay);

        SetGameState(GameState.Play);
        // ######################################################
        MIDIFacade.Instance.StartSong(); // AKLHFLKJHDKFJHSLDHFLKSJDFKLSJDF CHANGE TO AUDIO MANAGER LATER
       // ######################################################
        FindFirstObjectByType<PlatformSpawner>().StartSpawning();
        // should reset audience support
        // reset player position, etc.
    }

    public void SetGameState(GameState newState)
    {
        if (_isPaused && newState != GameState.Play)
        {
            ResumeGame();
        }

        _currentGameState = newState;
        Debug.Log("Game State changed to: " + _currentGameState);
    }

    // Switches scene based on result - called from player - CHANGE TO CALLED FROM
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
        if (_currentGameState == GameState.Play && !LaneDespawnDebugUI.IsFullMenuOpen && Input.GetKeyDown(_pauseKey))
        {
            TogglePause();
        }

        if (_currentGameState != GameState.Play)
        {
            _gameTimer = 0; // Restart timer when game state is in lose/win/menu
            return;
        }

        if (_isPaused) return;

        _gameTimer += Time.deltaTime;
    }

    public void TogglePause()
    {
        if (_isPaused)
        {
            ResumeGame();
            return;
        }

        PauseGame();
    }

    public void PauseGame()
    {
        if (_isPaused || _currentGameState != GameState.Play) return;

        _isPaused = true;
        Time.timeScale = 0f;

        if (MIDIFacade.Instance != null && MIDIFacade.Instance.audioSource != null && MIDIFacade.Instance.audioSource.isPlaying)
            MIDIFacade.Instance.audioSource.Pause();

        if (PlatformManager.Instance != null)
            PlatformManager.Instance.isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        _isPaused = false;
        Time.timeScale = 1f;

        if (MIDIFacade.Instance != null && MIDIFacade.Instance.audioSource != null)
            MIDIFacade.Instance.audioSource.UnPause();

        if (PlatformManager.Instance != null)
            PlatformManager.Instance.isPaused = false;
    }

    // Properties
    public GameState CurrentGameState => _currentGameState;

    public float GameLength => _gameLength;

    public float GameTimer => _gameTimer;

    public bool IsPaused => _isPaused;
}
