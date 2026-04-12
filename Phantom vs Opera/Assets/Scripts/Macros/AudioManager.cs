using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Optional")][SerializeField] private AudioSource _audioSource;

    [HideInInspector] public static AudioManager Instance;

    [SerializeField] private AudioClip _song; // unity can't play MIDI, so we need a .wav/ .mp3 file to play - should match the MIDI

    [SerializeField] private AudioClip[] _songTracks; // all the different isolated tracks
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

         _audioSource.clip = _song;

    }

    public void StartSong()
    {
        _audioSource.Play();
    }

    public void SwitchTrack(float track)
    {
        double currrentTime = GetAudioSourceTime();
        _audioSource.Stop();

        _audioSource.clip = _songTracks[(int)track-1]; // account for 0 index thing

        if (_audioSource.clip == null)
        {
            Debug.LogError("AudioManager could not find audio clip when changing tracks!");
            return;
        }

        _audioSource.time = (float)currrentTime; // same position in the song when switching tracks
        _audioSource.Play();
    }

    public double GetAudioSourceTime() // get the current time of the audio source in seconds - checked against NoteData.spawnTime
    {
        return (double)Instance._audioSource.timeSamples / Instance._audioSource.clip.frequency;
    }

    public AudioSource AudioSource => _audioSource;

}
