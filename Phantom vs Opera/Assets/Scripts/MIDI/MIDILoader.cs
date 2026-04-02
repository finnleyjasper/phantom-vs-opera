using UnityEngine;
using Melanchall.DryWetMidi.Core;

public class MIDILoader : MonoBehaviour
{
    public MidiFile LoadMIDI(string fileName)
    {
        MidiFile midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileName);

        return midiFile;
    }
}
