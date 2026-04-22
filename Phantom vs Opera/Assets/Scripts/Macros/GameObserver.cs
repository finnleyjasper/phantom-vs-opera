using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObserver : MonoBehaviour
{
    [HideInInspector] public static GameObserver Instance;

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
            CheckForSwitchAct();

            CheckFallenPlayer();
            CheckPlayerOnPlatform();
        }
    }


    // GAME STATE -------------------------
    private void CheckForGameOver()
    {
        // max audience support = win
        if (GameManager.Instance.AudienceSupport.AudienceSupportValue >= GameManager.Instance.MaxAudienceSupport)
        {
            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }
        else if (GameManager.Instance.AudienceSupport.AudienceSupportValue <= 0)
        {
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    private void CheckForSwitchAct()
    {
        if (AudioManager.Instance.AudioSource.clip.length == AudioManager.Instance.AudioSource.time)
        {
            GameManager.Instance.SwitchAct(GameManager.Instance.CurrentAct + 1);
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

    private void CheckPlayerOnPlatform()
    {
        if (GameManager.Instance.Player.IsOnPlatform == true) // increase audience support
        {
            GameManager.Instance.AudienceSupport.ManageAudienceSupport(GameManager.Instance.IncreasePerSecond * Time.fixedDeltaTime);
        }
        else // decrease audience support
        {
            GameManager.Instance.AudienceSupport.ManageAudienceSupport(-(GameManager.Instance.DecreasePerSecond * Time.fixedDeltaTime));
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
