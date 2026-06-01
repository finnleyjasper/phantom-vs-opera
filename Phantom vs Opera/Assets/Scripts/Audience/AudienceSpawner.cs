using UnityEngine;

public class AudienceSpawner : MonoBehaviour
{
    [Header("Spawn Timing")]
    [SerializeField] private bool _spawnOnStart = true;

    [Header("Crowd Layout")]
    [SerializeField] private Vector3 _crowdOrigin = new(1f, -1.985f, -10.75f);
    [SerializeField] private int _rows = 3;
    [SerializeField] private int _columns = 20;
    [SerializeField] private float _columnSpacing = 0f;
    [SerializeField] private float _rowSpacing = 0f;
    [SerializeField] private float _spacingPadding = 0f;
    [SerializeField] private float _positionJitter = 0f;
    [SerializeField] private float _depthJitter = 0f;

    [Header("Bean Shape")]
    [SerializeField] private Vector3 _beanScale = new(1.2f, 1.7f, 1.2f);
    [SerializeField] private float _rotationJitter = 18f;

    [Header("Combo Jump")]
    [SerializeField] private float _idleJumpHeight = 0.04f;
    [SerializeField] private float _jumpHeightPerComboLevel = 0.12f;
    [SerializeField] private float _maxJumpHeight = 1.4f;
    [SerializeField] private float _baseBounceSpeed = 2.5f;
    [SerializeField] private float _bounceSpeedPerComboLevel = 0.6f;
    [SerializeField] private float _maxBounceSpeed = 9f;

    [SerializeField] private Color[] _beanColors =
    {
        new(0.92f, 0.45f, 0.38f),
        new(0.95f, 0.62f, 0.48f),
        new(0.78f, 0.36f, 0.42f),
        new(0.88f, 0.52f, 0.55f),
        new(0.72f, 0.48f, 0.78f),
        new(0.55f, 0.62f, 0.82f),
    };

    private Transform _audienceRoot;
    private Material _beanMaterialTemplate;

    private void Start()
    {
        if (_spawnOnStart)
            SpawnAudience();
    }

    public void SpawnAudience()
    {
        if (_audienceRoot != null)
        {
            Destroy(_audienceRoot.gameObject);
            _audienceRoot = null;
        }

        EnsureAudienceRoot();
        SpawnCrowd();
    }

    public void ClearAudience()
    {
        if (_audienceRoot == null)
            return;

        for (int i = _audienceRoot.childCount - 1; i >= 0; i--)
            Destroy(_audienceRoot.GetChild(i).gameObject);
    }

    private void EnsureAudienceRoot()
    {
        if (_audienceRoot != null)
            return;

        var rootObject = new GameObject("Bean Audience");
        _audienceRoot = rootObject.transform;
        _audienceRoot.SetParent(transform, false);
    }

    private void SpawnCrowd()
    {
        float columnSpacing = GetColumnSpacing();
        float rowSpacing = GetRowSpacing();
        float beanHalfHeight = _beanScale.y;
        float startX = -((_columns - 1) * columnSpacing) * 0.5f;
        float startZ = -((_rows - 1) * rowSpacing) * 0.5f;

        for (int row = 0; row < _rows; row++)
        {
            for (int column = 0; column < _columns; column++)
            {
                float x = startX + (column * columnSpacing);
                float z = startZ + (row * rowSpacing);

                x += Random.Range(-_positionJitter, _positionJitter);
                z += Random.Range(-_depthJitter, _depthJitter);

                Vector3 offset = new(x, beanHalfHeight, z);
                SpawnBean(_crowdOrigin + offset);
            }
        }
    }

    private float GetColumnSpacing()
    {
        float beanDiameter = _beanScale.x;
        if (_columnSpacing > 0f)
            return Mathf.Max(_columnSpacing, beanDiameter + _spacingPadding);

        return beanDiameter * 0.95f + _spacingPadding;
    }

    private float GetRowSpacing()
    {
        float beanDiameter = _beanScale.z;
        if (_rowSpacing > 0f)
            return Mathf.Max(_rowSpacing, beanDiameter + _spacingPadding);

        return beanDiameter * 0.95f + _spacingPadding;
    }

    private void SpawnBean(Vector3 worldPosition)
    {
        GameObject bean = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        bean.name = "Audience Bean";

        Transform beanTransform = bean.transform;
        beanTransform.SetParent(_audienceRoot, false);
        beanTransform.localScale = _beanScale;
        beanTransform.position = worldPosition;
        beanTransform.rotation = Quaternion.Euler(
            0f,
            Random.Range(180f - _rotationJitter, 180f + _rotationJitter),
            0f);

        Collider collider = bean.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);

        ApplyBeanColor(bean.GetComponent<Renderer>());

        AudienceBean audienceBean = bean.AddComponent<AudienceBean>();
        audienceBean.Initialize(worldPosition, Random.Range(0f, Mathf.PI * 2f), BuildJumpSettings());
    }

    private AudienceJumpSettings BuildJumpSettings()
    {
        return new AudienceJumpSettings
        {
            IdleJumpHeight = _idleJumpHeight,
            JumpHeightPerComboLevel = _jumpHeightPerComboLevel,
            MaxJumpHeight = _maxJumpHeight,
            BaseBounceSpeed = _baseBounceSpeed,
            BounceSpeedPerComboLevel = _bounceSpeedPerComboLevel,
            MaxBounceSpeed = _maxBounceSpeed,
        };
    }

    private void ApplyBeanColor(Renderer renderer)
    {
        if (renderer == null)
            return;

        EnsureBeanMaterialTemplate();

        Material beanMaterial = new(_beanMaterialTemplate);
        Color color = PickRandomColor();
        beanMaterial.SetColor("_BaseColor", color);
        beanMaterial.SetColor("_Color", color);
        renderer.sharedMaterial = beanMaterial;
    }

    private void EnsureBeanMaterialTemplate()
    {
        if (_beanMaterialTemplate != null)
            return;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        _beanMaterialTemplate = new Material(shader);
    }

    private Color PickRandomColor()
    {
        if (_beanColors == null || _beanColors.Length == 0)
            return new Color(0.9f, 0.5f, 0.45f);

        return _beanColors[Random.Range(0, _beanColors.Length)];
    }

    private void OnDestroy()
    {
        if (_beanMaterialTemplate != null)
            Destroy(_beanMaterialTemplate);
    }
}
