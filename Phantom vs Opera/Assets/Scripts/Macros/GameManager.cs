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

<<<<<<< Updated upstream
    // Current state of the game - further functionality within GameManager will be based on this state
    // Should be changed within this script via methods called by other objects
    [SerializeField] private GameState _currentGameState = GameState.Play; // As we don't have a main menu yet, I temporarily changed starting state to Play

=======
    private bool _isTeleporting;
    private Player _player;
    private AudienceSupport _audienceSupport;
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    public void SetGameState(GameState newState)
=======
    private IEnumerator TeleportRoutine()
    {
        _isTeleporting = true;

        // Pause game systems
        SetGameState(GameState.Pause);

        _player.Pause(true);
        FindFirstObjectByType<PlatformManager>().Pause(true);
        AudioManager.Instance.AudioSource.Pause();

        // Apply fall punishment
        _audienceSupport.ManageAudienceSupport(-FallenPunishment);
        Debug.Log($"[Audience Support] Player fell! -" + $"{FallenPunishment}. New value: {_audienceSupport.AudienceSupportValue}");

        // Find safe platform
        MusicPlatform safePlatform = FindSafePlatform();

        if (safePlatform != null)
        {
            Vector3 safePos = safePlatform.transform.position;
            safePos.y += 2.5f; // height above platform
            _player.transform.position = safePos;
        }
        else
        {
            Debug.LogWarning("No safe platform found!");
        }

        // Wait so player can react
        yield return new WaitForSeconds(1.5f);

        // Resume game
        _player.Pause(false);
        FindFirstObjectByType<PlatformManager>().Pause(false);
        AudioManager.Instance.AudioSource.Play();

        SetGameState(GameState.Play);

        _isTeleporting = false;
    }

    private MusicPlatform FindSafePlatform()
    {
        MusicPlatform[] platforms = FindObjectsByType<MusicPlatform>(FindObjectsSortMode.None);

        MusicPlatform best = null;
        float bestDistance = float.MaxValue;

        foreach (var p in platforms)
        {
            float dist = Mathf.Abs(p.transform.position.x - _player.transform.position.x);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                best = p;
            }
        }

        return best;
    }

    public void HandlePlayerFall()
    {
        if (_isTeleporting) return;

        StartCoroutine(TeleportRoutine());
    }

    public void Pause()
>>>>>>> Stashed changes
    {
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
<<<<<<< Updated upstream
    void Update()
=======


    private void SetGameState(GameState newState)
>>>>>>> Stashed changes
    {
        if (_currentGameState != GameState.Play)
        {
            _gameTimer = 0; // Restart timer when game state is in lose/win/menu
            return;
        }

        _gameTimer += Time.deltaTime;
    }

    // Properties
    public GameState CurrentGameState => _currentGameState;

    public float GameLength => _gameLength;

    public float GameTimer => _gameTimer;
}
