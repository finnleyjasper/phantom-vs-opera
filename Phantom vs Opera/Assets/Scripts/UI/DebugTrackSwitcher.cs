using UnityEngine;

public class DebugTrackSwitcher : MonoBehaviour
{
    [SerializeField] private float  _targetTrack;

    public void SwitchTrack()
    {
        GameManager.Instance.SwitchTrack(_targetTrack);
    }
}
