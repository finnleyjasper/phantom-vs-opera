using UnityEngine;

public class PlayerSprite : MonoBehaviour
{

    // Private Variables 
    [Header("Sprites")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite slamSprite;
    private Sprite currentSprite;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprite = defaultSprite;
        spriteRenderer.sprite = currentSprite;
    }

    void Update()
    {
        SetSlamSprite();
    }

    // Method to switch sprite when spacebar pressed 
    public void SetSlamSprite()
    {
        if (playerController.IsSlamming == true)
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