using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Optional prefab view for an individual scrolling note.
// Assign this prefab in InputIndicatorTrackUI to make note visuals prefab-oriented.
public class InputIndicatorNoteView : MonoBehaviour
{
    [Header("Required")]
    public Image NoteImage;

    [Header("Optional")]
    public TextMeshProUGUI KeyLabel;
    public Image SpecialIcon;
}
