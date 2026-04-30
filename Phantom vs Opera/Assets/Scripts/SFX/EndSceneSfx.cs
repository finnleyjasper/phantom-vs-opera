using UnityEngine;

public class EndSceneSfx : MonoBehaviour
{
    // Variables 
    private GameManager.GameState gameState;

    [Space(10)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource endAudioSource;

    void Awake()
    {
        GetAudioSource();
    }

    void Start()
    {
        gameState = GameManager.Instance.CurrentGameState;

        // Depending on result, win/lose sfx plays
        if (gameState == GameManager.GameState.Lose)
        {
            AudioManager.Instance.PlaySoundEffect("boo", endAudioSource); // Play SFX - Lose condition
        }
        else if (gameState == GameManager.GameState.Win)
        {
            AudioManager.Instance.PlaySoundEffect("applause", endAudioSource); // Play SFX - Win condition
        }
    }

    // Method to get Audio Component 
    public void GetAudioSource()
    {
        if (endAudioSource == null)
        {
            endAudioSource = GetComponent<AudioSource>(); // gets AudioSource comp.

            if (endAudioSource == null)
            {
                Debug.LogWarning("AudioManager could not find an audio source");
                return;
            }
        }
    }
}
