
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Tooltip("Optional")][SerializeField] private AudioSource _audioSource;

    [HideInInspector] public static AudioManager Instance;

    [SerializeField] private AudioClip _song; // unity can't play MIDI, so we need a .wav/ .mp3 file to play - should match the MIDI

    [SerializeField] private AudioClip[] _songTracks; // all the different isolated tracks

    [SerializeField] private AudioMixer _pitchShifter; // an audio mixer that normalises the pitch after a tempo change
    [SerializeField] private string _pitchShifterParameter = "AudioPitch";

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
        if (_pitchShifter == null)
        {
            Debug.LogWarning("No pitch shifter assigned!");
            return;
        }
         _audioSource.clip = _song;
    }

    public void StartSong()
    {
        _audioSource.Play();
    }

    public void SwitchTrack(float track)
    {
        double currrentTime = _audioSource.time;
        _audioSource.Stop();

        if (track == 0) // play default song (all tracks)
        {
            _audioSource.clip = _song;
        }
        else // select individual track to make active
        {
            _audioSource.clip = _songTracks[(int)track-1]; // account for 0 index thing
        }

        if (_audioSource.clip == null)
        {
            Debug.LogError("AudioManager could not find audio clip when changing tracks!");
            return;
        }

        _audioSource.time = (float)currrentTime; // same position in the song when switching tracks
        _audioSource.Play();

    }

    public void SwitchTempo(float multiplier)
    {
        multiplier = Mathf.Max(0.01f, multiplier);

        float compensatedPitch = 1f / multiplier;
        compensatedPitch = Mathf.Clamp(compensatedPitch, 0.5f, 2f);

        bool pitchWasSet = _pitchShifter.SetFloat(_pitchShifterParameter, compensatedPitch);
        if (!pitchWasSet)
        {
            Debug.LogWarning($"AudioManager could not set mixer parameter '{_pitchShifterParameter}'. Make sure the Pitch Shifter Pitch value is exposed with this exact name.");
        }

        _audioSource.pitch = multiplier;
        Debug.Log("Audio changed tempo");
    }

    public AudioSource AudioSource => _audioSource;

}
