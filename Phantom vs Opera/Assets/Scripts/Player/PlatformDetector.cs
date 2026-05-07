using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    public Animator decalAnimator;
    public float rayDistance = 2f;
    public LayerMask platformMask;

    [Header("Animation States")]
    [SerializeField] private string idleState = "Star";
    [SerializeField] private string activeState = "PlatformIndicator";

    private bool isOnPlatform = false;

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        bool hitPlatform = Physics.Raycast(ray, out hit, rayDistance, platformMask);

        if (hitPlatform)
        {
            if (!isOnPlatform)
            {
                decalAnimator.Play(activeState);
                isOnPlatform = true;
            }
        }
        else
        {
            if (isOnPlatform)
            {
                decalAnimator.Play(idleState);
                isOnPlatform = false;
            }
        }
    }
}