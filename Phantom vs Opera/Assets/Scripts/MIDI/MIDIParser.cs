using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MIDIParser : MonoBehaviour
{
    public void GetData(MidiFile midiFile)
    {
        GetNoteArray(midiFile);
    }

    public Note[] GetNoteArray(MidiFile midiFile) // converts MIDI notes to an array of Note objects
    {
        var notes = midiFile.GetNotes();
        var notesArray = new Note[notes.Count];
        notes.CopyTo(notesArray, 0);

        return notesArray;
    }
}
