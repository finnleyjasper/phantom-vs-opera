using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MIDIParser : MonoBehaviour
{
    public List<NoteData> ConvertToNoteData(MidiFile midiFile, float lead) // converts MIDI Notes to our NoteData
    {
        var notes = midiFile.GetNotes();
        var tempoMap = midiFile.GetTempoMap(); // converts MIDI ticks to seconds, taking into account tempo changes etc.

        List<NoteData> noteDataList = new List<NoteData>();

        foreach (var note in notes)
        {

            NoteData data = new NoteData(
                pitch: note.NoteNumber, // 0-127 MIDI pitch
                instrument: note.Channel,
                noteOn: (float)note.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds,
                duration: (float)note.LengthAs<MetricTimeSpan>(tempoMap).TotalSeconds,
                noteOff: (float)note.EndTimeAs<MetricTimeSpan>(tempoMap).TotalSeconds,
                strength: note.Velocity,
                noteName: note.NoteName.ToString(),
                spawnLead: lead
            );

            noteDataList.Add(data);
        }

        return noteDataList;
    }
}
