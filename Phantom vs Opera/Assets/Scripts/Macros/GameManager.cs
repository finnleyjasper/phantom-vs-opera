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
        Pause,
        Win,
        Lose
    }

    [Header("Scenes")]
    public string MainMenuSceneName;
    public string PlaySceneName;
    public string EndSceneName;

    [Space(10)]
    [Header("Game Settings")]
    [SerializeField] [Tooltip("Delay before the level starts after loading")]private float _levelStartDelay = 2f;

    private bool _isTeleporting;
    [SerializeField] private GameState _currentGameState = GameState.Pause;
    [SerializeField] private float _currentTrack = 1; // refers to the currnet instrument track

    [Space(10)]
    [Header("Audience Support Settings")]
    public float StartingAudienceSupport = 5f; // starting value for audience support
    public float MaxAudienceSupport = 100f; // win condition
    public float IncreasePerSecond = 5.0f; // how much audience support increases per second when player is on platform
    public float DecreasePerSecond = 0.5f; // how much audience support decreases per second when player is not on platform
    public float FallenPunishment = 10f; // how much audience support decreases when player falls on floor

    private Player _player;
    private AudienceSupport _audienceSupport;
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

        _player = FindFirstObjectByType<Player>();
        _audienceSupport = FindFirstObjectByType<AudienceSupport>();

        SetGameState(GameState.Play);

        _player.Reset();
        _audienceSupport.ManageAudienceSupport(StartingAudienceSupport); // reset audience support value

        AudioManager.Instance.StartSong();
        FindFirstObjectByType<PlatformSpawner>().StartSpawning();
    }

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
    {
        SetGameState(GameState.Pause);
        _player.Pause(true);
        FindFirstObjectByType<PlatformManager>().Pause(true);
        AudioManager.Instance.AudioSource.Pause();
    }

    public void Play()
    {
        SetGameState(GameState.Play);
        _player.Pause(false);
        FindFirstObjectByType<PlatformManager>().Pause(false);
       AudioManager.Instance.AudioSource.Play();

    }

    public void GameOver(GameState result)
    {
        // should pause the game momentarity so player can realise what happened

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

    public void SwitchTrack(float track)
    {
        _currentTrack = track;
        AudioManager.Instance.SwitchTrack(track);
    }

    private void SetGameState(GameState newState)
    {
        _currentGameState = newState;
        Debug.Log("Game State changed to: " + _currentGameState);
    }

    // Properties
    public GameState CurrentGameState => _currentGameState;
    public Player Player => _player;
    public AudienceSupport AudienceSupport => _audienceSupport;
    public float CurrentTrack => _currentTrack;

}
