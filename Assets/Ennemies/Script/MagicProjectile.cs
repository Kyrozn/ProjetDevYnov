using UnityEngine;
using Mirror;

public class MagicProjectile : NetworkBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 15;
    public float lifetime = 5f;

    [Header("Optional FX")]
    public GameObject impactEffect;

    void Start()
    {
        // Auto-destruction après un délai
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }

            // SpawnImpact();
            NetworkServer.Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // SpawnImpact();
            NetworkServer.Destroy(gameObject);
        }
    }

}
