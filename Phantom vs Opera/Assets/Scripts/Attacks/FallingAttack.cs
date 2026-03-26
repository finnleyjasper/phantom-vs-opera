using UnityEngine;

public class FallingAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float fallSpeed = 10f;
    public int damage = 1;

    [HideInInspector]
    public FallingAttackSpawner spawner; // reference to spawner

    private void Update()
    {
        // Move downward constantly
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Hit player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("IsHit", damage, SendMessageOptions.DontRequireReceiver);
        }
        DestroySelf();
    }
    
    void DestroySelf()
    {
        // Notify spawner BEFORE destroying
        if (spawner != null)
        {
            spawner.NotifyAttackDestroyed();
        }

        Destroy(gameObject);
    }

}