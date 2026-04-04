using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class MIDIFacade : MonoBehaviour
{
    // DEBUG
    // show logs
    public bool debugMode = false;

    // MIDI stuff
    [HideInInspector] public static MIDIFacade Instance;
    private MidiFile _midiFile;

    private int _minPitch = 0;
    private int _maxPitch = 127; // MIDI standard range of pitches, from C-1

    // subsystems
    private MIDILoader _midiLoader; // loads in the MIDI file and extracts the relevant data for the parser to use
    private MIDIParser _midiParser; // the parser prepares data for the facade to use - ie. converts MIDI tempo --> seconds, etc.

    [Header("Settings")]
    [Tooltip("Name of the file within StreamingAssets")]public string fileName; // MidiFile.Read() requires a path (string), so we need the filename
    [SerializeField] private AudioClip _audioFile; // unity can't play MIDI, so we need a .wav/ .mp3 file to play
    public AudioSource audioSource;
     private void Awake()
    {
        // enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // get subsystems
        _midiLoader = GetComponent<MIDILoader>();
        _midiParser = GetComponent<MIDIParser>();

    }

    void Start()
    {
        // if there's no audio source assigned, try to find one in the scene
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
            if (audioSource == null && debugMode)
            {
                Debug.LogError("AudioSource not assigned in MIDIFacade and none found in scene.");
                return;
            }
            else
            {
                Debug.Log("AudioSource automatically assigned from scene.");
            }
        }

        _midiFile = _midiLoader.LoadMIDI(fileName);
        if (debugMode)
        {
            if (_midiFile != null)
            {
                Debug.Log("MIDI file loaded successfully: " + fileName);
            }
            else
            {
                Debug.LogError("Failed to load MIDI file: " + fileName);
            }
        }
    }

    public List<NoteData> GetNoteData(float spawnDelay) // parse the MIDI file & get note data
    {
       List<NoteData> _noteDataList = _midiParser.ConvertToNoteData(_midiFile, spawnDelay);

       if (debugMode)
        {
            if (_noteDataList != null && _noteDataList.Count > 0)
            {
                string notesInfo = "Following NoteData parsed successfully: ";
                foreach (var note in _noteDataList)
                {
                    notesInfo += note.noteName + " at pitch" + note.pitch + ", ";
                }
                Debug.Log(notesInfo.TrimEnd(',', ' '));
            }
            else
            {
                Debug.LogWarning("No notes parsed from MIDI file.");
                return _noteDataList;
            }
        }

        return _noteDataList;
    }

    public void StartSong()
    {
        audioSource.clip = _audioFile;
        audioSource.Play();

        if (debugMode)
        {
            Debug.Log("Audio source started playing: " + audioSource.clip.name);
        }
    }

    public double GetAudioSourceTime() // get the current time of the audio source in seconds - checked against NoteData.spawnTime
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    // Properties
    public int MinPitch => _minPitch;
    public int MaxPitch => _maxPitch;
}
