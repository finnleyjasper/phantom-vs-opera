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
        Vector3 scale = transform.localScale;
        scale.x = length * PlatformManager.Instance.platformLengthMultiplier; // base length from MIDI is a bit short, so multiply it
        transform.localScale = scale;
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
