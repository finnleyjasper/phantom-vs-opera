using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;

public class DebugTempoText : MonoBehaviour
{
    private Slider slider;

    [SerializeField] private TextMeshProUGUI _text;

    private void Awake() {
        slider = GetComponent<Slider>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
        _text.text = "Current Tempo Multiplier: " + GameManager.Instance.CurrentTempoMultiplier;
    }

}
