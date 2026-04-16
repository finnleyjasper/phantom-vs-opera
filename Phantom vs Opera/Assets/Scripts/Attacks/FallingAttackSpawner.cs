using UnityEngine;

public class FallingAttackSpawner : MonoBehaviour
{
    public static FallingAttackSpawner Instance;

    [Header("Prefabs")]
    public GameObject normalAttackPrefab;
    public GameObject strongAttackPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 1f;        // attacks per second
    public float spawnWidth = 10f;      // horizontal range
    public float spawnHeight = 10f;     // how high above
    [SerializeField] private float _attackFallSpeed = 10f;

    [Header("Attack Chances")]
    [Range(0f, 1f)]
    public float strongAttackChance = 0.2f;

    [Header("Performance")]
    public int maxAttacks = 20;
    private int currentAttacks = 0;

    private float timer;

    public float AttackFallSpeed => _attackFallSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / spawnRate)
        {
            SpawnAttack();
            timer = 0f;
        }
    }

    void SpawnAttack()
    {
        float randomX = Random.Range(-spawnWidth, spawnWidth);
        Vector3 spawnPos = new Vector3(randomX, spawnHeight, 0);

        GameObject prefabToSpawn;

        // Decide attack type
        if (Random.value < strongAttackChance && strongAttackPrefab != null)
        {
            prefabToSpawn = strongAttackPrefab;
        }
        else
        {
            prefabToSpawn = normalAttackPrefab;
        }

        GameObject attack = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Get the script from the spawned object
        FallingAttack fa = attack.GetComponent<FallingAttack>();

        if (fa != null)
        {
            fa.spawner = this; // Set reference to spawner for tracking
            fa.fallSpeed = _attackFallSpeed;
        }

        currentAttacks++;
    }
    public void NotifyAttackDestroyed()
    {
        currentAttacks--;
    }

    public void SetAttackFallSpeed(float newSpeed)
    {
        _attackFallSpeed = Mathf.Max(0.01f, newSpeed);

        FallingAttack[] activeAttacks = FindObjectsByType<FallingAttack>(FindObjectsSortMode.None);
        foreach (FallingAttack attack in activeAttacks)
        {
            if (attack != null)
                attack.fallSpeed = _attackFallSpeed;
        }
    }
}