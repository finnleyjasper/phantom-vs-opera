using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObserver : MonoBehaviour
{
    [HideInInspector] public static GameObserver Instance;

    private bool _wasOnPlatform;
    private float _leftPlatformAt = -1f;
    private bool _fallOffPenaltyApplied;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Play)
        {
            CheckForGameOver();

            ProcessAudienceSupport();
            CheckFallenPlayer();
        }
    }

    /// <summary>Call when a level starts so landing/fall tracking matches the reset player.</summary>
    public void ResetAudiencePlatformState()
    {
        _wasOnPlatform = false;
        _leftPlatformAt = -1f;
        _fallOffPenaltyApplied = false;
    }

    /// <summary>Floor teleport: applies LandingBonus × 1.5 once if fall-off was not already applied this air segment.</summary>
    public void ApplyFloorFallPenalty()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.AudienceSupport == null) return;
        if (_fallOffPenaltyApplied) return;
        gm.AudienceSupport.ManageAudienceSupport(-(gm.LandingBonus * 1.5f));
        _fallOffPenaltyApplied = true;
        _leftPlatformAt = -1f;
    }

    private void ProcessAudienceSupport()
    {
        GameManager gm = GameManager.Instance;
        Player player = gm.Player;
        if (player == null) return;

        bool on = player.IsOnPlatform;

        if (on && !_wasOnPlatform)
        {
            gm.AudienceSupport.ManageAudienceSupport(gm.LandingBonus);
            _leftPlatformAt = -1f;
            _fallOffPenaltyApplied = false;
        }

        if (!on && _wasOnPlatform)
        {
            _leftPlatformAt = Time.time;
            _fallOffPenaltyApplied = false;
        }

        if (!on && _leftPlatformAt >= 0f && !_fallOffPenaltyApplied)
        {
            if (Time.time - _leftPlatformAt >= gm.PlatformLeaveGraceSeconds)
            {
                gm.AudienceSupport.ManageAudienceSupport(-(gm.LandingBonus * 1.5f));
                _fallOffPenaltyApplied = true;
            }
        }

        if (on)
            gm.AudienceSupport.ManageAudienceSupport(gm.IncreasePerSecond * Time.deltaTime);
        else
            gm.AudienceSupport.ManageAudienceSupport(-(gm.DecreasePerSecond * Time.deltaTime));

        _wasOnPlatform = on;
    }


    // GAME STATE -------------------------
    private void CheckForGameOver()
    {
        // max audience support = win
        if (GameManager.Instance.AudienceSupport.AudienceSupportValue >= GameManager.Instance.MaxAudienceSupport)
        {
            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }
        // song finishes = win ??
        else if (AudioManager.Instance.AudioSource.isPlaying && AudioManager.Instance.AudioSource.time >= AudioManager.Instance.AudioSource.clip.length)
        {
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
        //lose
        else if (GameManager.Instance.AudienceSupport.AudienceSupportValue <= 0)
        {
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    // PLAYER -----------------------------
    private void CheckFallenPlayer()
    {
        if (GameManager.Instance.Player.FellOnFloor)
        {
            GameManager.Instance.HandlePlayerFall();
        }
    }

    // SCENE MANAGEMENT -------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameManager.Instance.PlaySceneName)
        {
            GameManager.Instance.StartGame();
        }
    }
}
