using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

public class Demo : MonoBehaviour
{
    public TextAsset midiAsset;

    private void Start()
    {
        if (midiAsset != null)
        {
            MidiFile midiFile = MidiFile.Read(new MemoryStream(midiAsset.bytes));
            var notes = midiFile.GetNotes();
            foreach (var note in notes)
            {
                Debug.Log(note.NoteName);
            }
        }
        else
        {
            Debug.LogWarning("No MIDI asset assigned.");
        }
    }
}
