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
    [Space(10)]
    [SerializeField] [Tooltip("Delay before the level starts after loading")]private float _levelStartDelay = 2f;
    [SerializeField] private GameState _currentGameState = GameState.Play;
    [Space(10)]
    [Header("Audience Support Settings")]
    public float MaxAudienceSupport = 10f; // win condition
    public float IncreasePerSecond = 1.0f; // how much audience support increases per second when player is on platform
    public float FallenPunishment = 5f; // how much audience support decreases when player falls on floor

    [HideInInspector] public static GameManager Instance;

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

    public void StartGame() // should get called from menu buttons
    {
        StartCoroutine(StartLevelAfterDelay());
    }

    private IEnumerator StartLevelAfterDelay() // give some time between loading the scene and starting the level
    {
        yield return new WaitForSeconds(_levelStartDelay);

        SetGameState(GameState.Play);
        AudioManager.Instance.StartSong();
        FindFirstObjectByType<PlatformSpawner>().StartSpawning();
        // should reset audience support
        // reset player position, etc.
    }

    public void SetGameState(GameState newState)
    {
        _currentGameState = newState;
        Debug.Log("Game State changed to: " + _currentGameState);
    }

    // Switches scene based on result - called from player - CHANGE TO CALLED FROM
    public void GameOver(GameState result)
    {
        AudioManager.Instance.AudioSource.Stop();
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

    // Properties
    public GameState CurrentGameState => _currentGameState;

}
