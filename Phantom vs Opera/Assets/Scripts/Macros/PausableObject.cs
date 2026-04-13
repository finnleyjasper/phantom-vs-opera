using UnityEngine;

// Gives functionality for pausing movement on objects with rigidbodies
// Used by Player and MusicPlatform

public class PausableObject : MonoBehaviour
{
    private bool _isPaused;

    // position and physics
    private Rigidbody _rigidbody;
    private bool _storedUseGravity;
    private RigidbodyConstraints _storedConstraints;

    virtual protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Pause(bool shouldPause)
    {
        if (_isPaused == shouldPause)
        {
            return;
        }

        _isPaused = shouldPause;

        if (shouldPause)
        {
            _storedUseGravity = _rigidbody.useGravity;
            _storedConstraints = _rigidbody.constraints;

            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }

        _rigidbody.useGravity = _storedUseGravity;
        _rigidbody.constraints = _storedConstraints;
    }

    public Rigidbody Rigidbody => _rigidbody;
    public bool IsPaused => _isPaused;

}
