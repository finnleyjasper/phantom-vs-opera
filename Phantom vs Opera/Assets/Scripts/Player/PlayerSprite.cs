using UnityEngine;

public class PlayerSprite : MonoBehaviour
{

    // Private Variables 
    [Header("Sprites")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite slamSprite;
    private Sprite currentSprite;
    private bool lastIsSlamming;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    void Update()
    {
        SetSlamSprite();
    }

    // Method to switch sprite when spacebar pressed 
    public void SetSlamSprite()
    {
        // Checks if null
        if (playerController == null || spriteRenderer == null)
        {
            Debug.LogWarning("playerController or spriteRenderer empty!");
            return;
        }

        // Only continues running method if 'isSlamming' condition has changed
        if (lastIsSlamming == playerController.IsSlamming)
        {
            return;
        }

        lastIsSlamming = playerController.IsSlamming;

        //Switches Sprite
        if (playerController.IsSlamming)
        {
            currentSprite = slamSprite;
        }
        else
        {
            currentSprite = defaultSprite;
        }

        spriteRenderer.sprite = currentSprite;
    }
}