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

    // Variables for changing platform colour after passing player 
    [Tooltip("Change the colour of untouched platforms once they pass player")]
    [SerializeField] private Color _passedPlatformColour = Color.grey;

    [Tooltip("Change the colour of touched platforms once they pass player")]
    [SerializeField] private Color _wasOnPlatformColour = Color.darkGray;

    private float _platformPositionX;
    private float _playerPostiionX;
    private bool _hasBeenRidden;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
        _platformPositionX = this.transform.position.x;
        _playerPostiionX = PlatformManager.Instance.PlayerX;
        _hasBeenRidden = false;
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

        _platformPositionX = this.transform.position.x;
        PlatformGetsPassedColour();
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

    // Method changes platform's colour if it has passed the player 
    public void PlatformGetsPassedColour() // DELETE THIS Delete
    {
        if (_platformPositionX >= _playerPostiionX)
        {
            return; // is this necessary? - delete 
        }

        if (_hasBeenRidden)
        {
            ApplyWasOnColor(_wasOnPlatformColour);
        }

        else
        {
            ApplyPassedColor(_passedPlatformColour);
        }
    }

    // Method applies 'passed colour' to untouched platform without instancing materials (URP Lit)
    public void ApplyPassedColor(Color _passedPlatformColour)
    {
        if (_meshRenderer == null) return;
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, _passedPlatformColour);
        _propertyBlock.SetColor(ColorId, _passedPlatformColour);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    // Method applies 'passed colour' to touched platform without instancing materials (URP Lit)
    public void ApplyWasOnColor(Color _wasOnPlatformColour)
    {
        if (_meshRenderer == null) return;
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, _wasOnPlatformColour);
        _propertyBlock.SetColor(ColorId, _wasOnPlatformColour);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            WasRidden();
        }
    }

    private void WasRidden()
    {
        _hasBeenRidden = true;
        ApplyWasOnColor(_wasOnPlatformColour);
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
