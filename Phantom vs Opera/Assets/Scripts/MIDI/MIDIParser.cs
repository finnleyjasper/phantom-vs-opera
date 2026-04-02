using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MIDIParser : MonoBehaviour
{
    public void GetData(MidiFile midiFile)
    {
        GetNoteArray(midiFile);
    }

    public Note[] GetNoteArray(MidiFile midiFile)
    {
        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
    }
}
