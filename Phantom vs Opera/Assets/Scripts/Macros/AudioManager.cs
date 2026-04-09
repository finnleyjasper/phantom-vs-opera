using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Optional")][SerializeField] private AudioSource _audioSource;

    [HideInInspector] public static AudioManager Instance;

    [SerializeField] private AudioClip _song; // unity can't play MIDI, so we need a .wav/ .mp3 file to play - should match the MIDI

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // if there's no audio source assigned, try to find one in the scene
        if (_audioSource == null)
        {
            _audioSource = FindFirstObjectByType<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogWarning("AudioManager could not find an Audio Source.");
                return;
            }
        }

    }

    public void StartSong()
    {
        _audioSource.clip = _song;
        _audioSource.Play();
    }

    public double GetAudioSourceTime() // get the current time of the audio source in seconds - checked against NoteData.spawnTime
    {
        return (double)Instance._audioSource.timeSamples / Instance._audioSource.clip.frequency;
    }

    public AudioSource AudioSource => _audioSource;

}
