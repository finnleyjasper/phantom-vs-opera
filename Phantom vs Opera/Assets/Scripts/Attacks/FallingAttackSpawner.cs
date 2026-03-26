using UnityEngine;

public class FallingAttackSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject normalAttackPrefab;
    public GameObject strongAttackPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 1f;        // attacks per second
    public float spawnWidth = 10f;      // horizontal range
    public float spawnHeight = 10f;     // how high above

    [Header("Attack Chances")]
    [Range(0f, 1f)]
    public float strongAttackChance = 0.2f;

    [Header("Performance")]
    public int maxAttacks = 20;
    private int currentAttacks = 0;

    private float timer;

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
        
        fa.spawner = this; // Set reference to spawner for tracking
        currentAttacks++;
    }
    public void NotifyAttackDestroyed()
    {
        currentAttacks--;
    }
}