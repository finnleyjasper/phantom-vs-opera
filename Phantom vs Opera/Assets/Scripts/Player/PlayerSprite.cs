using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    private const string ColourSwapShaderName = "Custom/ExactColorSwap";

    private static readonly int OriginalColorId = Shader.PropertyToID("_OriginalColor");
    private static readonly int SecondaryOriginalColorId = Shader.PropertyToID("_SecondaryOriginalColor");
    private static readonly int ReplacementColorId = Shader.PropertyToID("_ReplacementColor");
    private static readonly int ToleranceId = Shader.PropertyToID("_Tolerance");
    private static readonly int SecondaryToleranceId = Shader.PropertyToID("_SecondaryTolerance");

    // Private Variables
    [Header("Sprites")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite slamSprite;

    [Header("Accent Colour Swap")]
    [SerializeField] private Material colourSwapMaterial;
    [SerializeField] private Color originalAccentColor = new Color(0.52f, 0.65f, 0.35f, 1f);
    [SerializeField, Range(0f, 0.5f)] private float colourMatchTolerance = 0.18f;
    [SerializeField] private Color secondaryOriginalAccentColor = new Color(0.17f, 0.31f, 0.08f, 1f);
    [SerializeField, Range(0f, 0.5f)] private float secondaryColourMatchTolerance = 0.12f;
    private PlatformSpawner platformSpawner;

    private Sprite currentSprite;
    private bool lastIsSlamming;
    private int lastLaneIndex = -1;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Material runtimeColourSwapMaterial;
    private Material activeColourSwapMaterial;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
        platformSpawner = FindFirstObjectByType<PlatformSpawner>();

        ApplyColourSwapMaterial();

        if (platformSpawner == null)
        {
            platformSpawner = FindFirstObjectByType<PlatformSpawner>();
        }

        if (spriteRenderer.sharedMaterial == null || !spriteRenderer.sharedMaterial.HasProperty(ReplacementColorId))
        {
            Debug.LogWarning("PlayerSprite needs a material using ColourSwap.shader on the SpriteRenderer or in the Colour Swap Material field.");
        }

        ApplyAccentColour();
    }

    private void OnDestroy()
    {
        if (activeColourSwapMaterial != null)
        {
            Destroy(activeColourSwapMaterial);
        }
    }

    void Update()
    {
        SetSlamSprite();
        ApplyAccentColour();
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

    private void ApplyAccentColour()
    {
        if (playerController == null || spriteRenderer == null || platformSpawner == null)
        {
            return;
        }

        int laneIndex = playerController.CurrentLaneIndex;
        lastLaneIndex = laneIndex;
        Color replacementColor = platformSpawner.GetLaneColor(laneIndex);

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(OriginalColorId, originalAccentColor);
        propertyBlock.SetColor(SecondaryOriginalColorId, secondaryOriginalAccentColor);
        propertyBlock.SetColor(ReplacementColorId, replacementColor);
        propertyBlock.SetFloat(ToleranceId, colourMatchTolerance);
        propertyBlock.SetFloat(SecondaryToleranceId, secondaryColourMatchTolerance);
        spriteRenderer.SetPropertyBlock(propertyBlock);

        if (activeColourSwapMaterial != null)
        {
            activeColourSwapMaterial.SetColor(OriginalColorId, originalAccentColor);
            activeColourSwapMaterial.SetColor(SecondaryOriginalColorId, secondaryOriginalAccentColor);
            activeColourSwapMaterial.SetColor(ReplacementColorId, replacementColor);
            activeColourSwapMaterial.SetFloat(ToleranceId, colourMatchTolerance);
            activeColourSwapMaterial.SetFloat(SecondaryToleranceId, secondaryColourMatchTolerance);
        }
    }

    private void ApplyColourSwapMaterial()
    {
        if (colourSwapMaterial != null)
        {
            activeColourSwapMaterial = new Material(colourSwapMaterial)
            {
                name = $"{colourSwapMaterial.name} (Player Instance)"
            };
            spriteRenderer.material = activeColourSwapMaterial;
            return;
        }

        if (spriteRenderer.sharedMaterial != null && spriteRenderer.sharedMaterial.HasProperty(ReplacementColorId))
        {
            activeColourSwapMaterial = spriteRenderer.material;
            return;
        }

        Shader colourSwapShader = Shader.Find(ColourSwapShaderName);
        if (colourSwapShader == null)
        {
            Debug.LogWarning($"Could not find shader '{ColourSwapShaderName}'. Make sure ColourSwap.shader is in the project.");
            return;
        }

        runtimeColourSwapMaterial = new Material(colourSwapShader)
        {
            name = "Runtime Player Colour Swap"
        };
        activeColourSwapMaterial = runtimeColourSwapMaterial;
        spriteRenderer.material = activeColourSwapMaterial;
    }
}
