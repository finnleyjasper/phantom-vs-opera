using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

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
    [SerializeField] [Tooltip("Padding between note reaching player. Lower number = note heard earlier.")] private float _noteTimingPadding = -0.3f;

    private bool _isTeleporting;
    [SerializeField] private GameState _currentGameState = GameState.Pause;
    [SerializeField] private float _currentTrack = 0; // refers to the current track - 0 is all

    [Space(10)]
    [Header("Audience Support Settings")]
    public float StartingAudienceSupport = 5f; // starting value for audience support
    public float MaxAudienceSupport = 100f; // win condition
    [Tooltip("Audience support gained once each time the player lands on a platform (air → platform).")]
    public float LandingBonus = 3f;
    [Tooltip("Audience support gained per second while the player stays on a platform.")]
    public float IncreasePerSecond = 5.0f;
    [Tooltip("Legacy: no longer used. Audience is reduced by LandingBonus × 1.5 on each floor touch, not per second in the air.")]
    public float DecreasePerSecond = 0.5f;
    [Tooltip("Legacy: no longer used (leave-platform instant penalty removed). Kept for existing scenes / prefab data.")]
    public float PlatformLeaveGraceSeconds = 0.25f;

    private Player _player;
    private AudienceSupport _audienceSupport;
    [HideInInspector] public static GameManager Instance;

    private float _gameTime; // when StartGame() was called - used to time platform spawning

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

    private void Update()
    {
        TryTogglePauseWithEscape();

        if (_currentGameState == GameState.Play)
        {
            // used to time platform spawning - in GameManager so Pause() and Play() still work
            _gameTime += Time.deltaTime;
        }
    }

    private void TryTogglePauseWithEscape()
    {
        if (_isTeleporting || _player == null) return;
        if (_currentGameState != GameState.Play && _currentGameState != GameState.Pause) return;

        Keyboard kb = Keyboard.current;
        if (kb == null || !kb.escapeKey.wasPressedThisFrame) return;

        if (_currentGameState == GameState.Play)
            Pause();
        else
            Play();
    }


    public void StartGame() // should get called from menu buttons
    {
        StartCoroutine(StartLevelAfterDelay());
    }

    private IEnumerator StartLevelAfterDelay() // give some time between loading the scene and starting the level
    {
        yield return new WaitForSeconds(_levelStartDelay);

        _gameTime = Time.deltaTime - PlatformManager.Instance.TravelTime + _noteTimingPadding;

        Debug.Log("Game Time: " + _gameTime);

        _player = FindFirstObjectByType<Player>();
        _audienceSupport = FindFirstObjectByType<AudienceSupport>();

        SetGameState(GameState.Play);

        _player.Reset();
        _audienceSupport.ManageAudienceSupport(StartingAudienceSupport); // reset audience support value
        if (GameObserver.Instance != null)
            GameObserver.Instance.ResetAudiencePlatformState();

        StartCoroutine(StartMusic());
        FindFirstObjectByType<PlatformSpawner>().StartSpawning();
    }

    private IEnumerator StartMusic() // start music when first platform reaches the player
    {
        yield return new WaitForSeconds(PlatformManager.Instance.TravelTime);
        AudioManager.Instance.StartSong();
    }

    private IEnumerator TeleportRoutine()
    {
        _isTeleporting = true;

        // Pause game systems
        SetGameState(GameState.Pause);

        _player.Pause(true);
        FindFirstObjectByType<PlatformManager>().Pause(true);
        AudioManager.Instance.AudioSource.Pause();

        // Floor audience penalty: applied on <see cref="Player"/> floor collision, not here.

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
    public float GameTime => _gameTime;

}
