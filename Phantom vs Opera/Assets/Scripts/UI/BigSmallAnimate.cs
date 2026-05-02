using UnityEngine;

public class BigSmallAnimate : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 0.1f;

    private Vector3 startScale;

    private void OnEnable()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * speed) * scaleAmount;
        transform.localScale = startScale * pulse;
    }
}
