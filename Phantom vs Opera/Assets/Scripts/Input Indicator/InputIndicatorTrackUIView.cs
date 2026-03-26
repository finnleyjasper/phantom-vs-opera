using UnityEngine;
using UnityEngine.UI;

// Optional prefab "view" for InputIndicatorTrackUI.
// If you assign a prefab with this component, the track UI will use its references
// instead of building the canvas/bar/track/hitline in code.
public class InputIndicatorTrackUIView : MonoBehaviour
{
    [Header("Required References")]
    public RectTransform TrackRect;
    public Image HitZoneImage;
}
