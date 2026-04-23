using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Runs after <see cref="Player"/> so <see cref="Player.IsOnPlatform"/> matches the latest fixed physics step.</summary>
[DefaultExecutionOrder(20)]
public class GameObserver : MonoBehaviour
{
    [HideInInspector] public static GameObserver Instance;

    [Tooltip("Consecutive fixed frames with physics \"not on platform\" before we stop the riding (IncreasePerSecond) rate. " +
             "Handles overlap gaps: a few false frames in a row while you are still on the platform no longer cost half your gain.")]
    [SerializeField, Min(1)]
    private int _fixedStepsOffBeforeStopRidingRate = 4;

    /// <summary>Sum of all <see cref="AudienceSupport.ManageAudienceSupport"/> changes from the most recent <see cref="ProcessAudienceSupport"/> (one physics step). For F1 debug UI.</summary>
    public float LastFixedStepAudienceDelta { get; private set; }

    private bool _wasOnPlatform;
    private float _lastTimeSeenOnPlatform = -1f;
    private int _consecutivePhysicsNotOn;

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
            CheckFallenPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameManager.GameState.Play)
            return;

        ProcessAudienceSupport();
    }

    /// <summary>Call when a level starts so landing/fall tracking matches the reset player.</summary>
    public void ResetAudiencePlatformState()
    {
        _wasOnPlatform = false;
        _lastTimeSeenOnPlatform = -1f;
        _consecutivePhysicsNotOn = 0;
        LastFixedStepAudienceDelta = 0f;
    }

    /// <summary>Called from <see cref="Player"/> when a floor collider is touched. One immediate −(LandingBonus×1.5) per event.</summary>
    public void OnPlayerTouchedFloorForAudience()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.AudienceSupport == null) return;
        if (gm.CurrentGameState != GameManager.GameState.Play) return;
        gm.AudienceSupport.ManageAudienceSupport(-(gm.LandingBonus * 1.5f));
    }

    private void ProcessAudienceSupport()
    {
        GameManager gm = GameManager.Instance;
        Player player = gm.Player;
        if (player == null) return;

        float stepSum = 0f;
        void Apply(float d)
        {
            stepSum += d;
            gm.AudienceSupport.ManageAudienceSupport(d);
        }

        bool on = player.IsOnPlatform;

        if (on && !_wasOnPlatform)
        {
            Apply(gm.LandingBonus);
        }

        float fdt = Time.fixedDeltaTime;

        if (on)
        {
            _consecutivePhysicsNotOn = 0;
        }
        else if (_lastTimeSeenOnPlatform >= 0f)
        {
            _consecutivePhysicsNotOn++;
        }

        bool shouldApplyRidingPerSecond = on
            || (_lastTimeSeenOnPlatform >= 0f && _consecutivePhysicsNotOn < _fixedStepsOffBeforeStopRidingRate);

        if (shouldApplyRidingPerSecond)
        {
            Apply(gm.IncreasePerSecond * fdt);
            if (on)
                _lastTimeSeenOnPlatform = Time.time;
        }

        LastFixedStepAudienceDelta = stepSum;
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
