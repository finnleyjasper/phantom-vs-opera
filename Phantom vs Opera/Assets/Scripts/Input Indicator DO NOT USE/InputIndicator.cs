using UnityEngine;
using UnityEngine.InputSystem;

// Represents a single keyboard key used as a input.
// Attach to its own GameObject — the manager will reference it.
public class InputIndicator : MonoBehaviour
{
    [SerializeField] private Key _inputKey;
    [SerializeField] private Sprite _noteSprite;

    public Key InputKey => _inputKey;
    public Sprite NoteSprite => _noteSprite;

    // True only on the exact frame the key is pressed.
    public bool WasPressed =>
        Keyboard.current != null && Keyboard.current[_inputKey].wasPressedThisFrame;

    // Call this to set up the indicator at runtime instead of using the Inspector.
    public void Initialize(Key key, Sprite noteSprite = null)
    {
        _inputKey = key;
        _noteSprite = noteSprite;
    }
}
