using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class MIDIFacade : MonoBehaviour
{
    // DEBUG
    // show logs
    public bool debugMode = false;

    // MIDI stuff
    [HideInInspector] public static MIDIFacade Instance;
    private MidiFile _midiFile;
    private Note[] _noteArray; // all notes in the MIDI file

    // subsystems
    private MIDILoader _midiLoader; // loads in the MIDI file and extracts the relevant data for the parser to use
    private MIDIParser _midiParser; // the parser prepares data for the facade to use - ie. converts MIDI tempo --> seconds, etc.

    [Header("Settings")]
    public AudioSource audioSource; // the audio source that will play the MIDI file's audio
    [Tooltip("Name of the file within StreamingAssets")]public string fileName; // MidiFile.Read() requires a path (string), so we need the filename

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

        // load the MIDI file
        _midiFile = _midiLoader.LoadMIDI(fileName);
        if (_midiFile == null && debugMode)
        {
            Debug.LogError("Failed to load MIDI file: " + fileName);
            return;
        }

    }

    public void StartLevel() // start music + platform spawning
    {
        // parse the MIDI file & get note data
       _noteArray = _midiParser.GetNoteArray(_midiFile);

       foreach (var note in _noteArray)
        {
            Debug.Log(note.NoteName + " at " + note.Time);
        }

        // more stuff later
        StartSong();
    }

    public double GetAudioSourceTime() // get the current time of the audio source in seconds
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    private void StartSong()
    {
        // fix this
        audioSource.Play();
    }
}
