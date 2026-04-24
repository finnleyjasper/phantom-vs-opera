using UnityEngine;
using UnityEngine.UI;

public class DebugTempoSlider : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateTempo()
    {
        GameManager.Instance.SwitchTempo(slider.value);
    }

}
