using UnityEngine;

public class MusicPlatform : PausableObject
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    [Header("Music Properties")]
    [Range(0, 127)]
    public int pitch = 1;          // Pitch
    [Min(0.1f)]
    public float length = 1f;   // Duration

    [HideInInspector] public int laneIndex = -1;
    [HideInInspector] public string noteName;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        ApplyLengthScale();
    }

    void Update()
    {
        if (!IsPaused) // move platform left
        {
            float speed = PlatformManager.Instance.GetSpeed();
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }

    void ApplyLengthScale()
    {
        // Store the original position before scaling
        Vector3 originalPos = transform.position;

        // Apply the scale
        Vector3 scale = transform.localScale;
        float scaledLength = length * PlatformManager.Instance.platformLengthMultiplier;
        scale.x = scaledLength;
        transform.localScale = scale;

        // Adjust position so the right edge stays at the original spawn point
        // The platform mesh has a default width (when localScale.x == 1), we need to account for that
        // Shift left by: (scaledLength - 1) / 2, assuming default mesh width is 1 unit
        Vector3 newPos = originalPos;
        newPos.x -= (scaledLength - 1f) / 2f;
        transform.position = newPos;
    }

    /// <summary>Swap to a full material for this lane (optional; set on PlatformSpawner).</summary>
    public void ApplyLaneMaterial(Material material)
    {
        if (_meshRenderer == null || material == null) return;
        _meshRenderer.sharedMaterial = material;
    }

    /// <summary>Tint the platform for this lane without instancing materials (URP Lit).</summary>
    public void ApplyLaneColor(Color color)
    {
        if (_meshRenderer == null) return;
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, color);
        _propertyBlock.SetColor(ColorId, color);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void OnEnable()
    {
        PlatformManager.Instance.RegisterPlatform(this);
    }

    private void OnDestroy()
    {
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.UnregisterPlatform(this);
    }
}
