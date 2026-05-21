using UnityEngine;

/// <summary>
/// Legacy uGUI audience bar — replaced by <see cref="AudienceBarUI"/> + audienceBar.uxml.
/// </summary>
public class AudienceSupportBarUI : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
