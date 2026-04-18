using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentTrackUIText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _text.text = "Current Track: " + GameManager.Instance.CurrentTrack;
    }
}
