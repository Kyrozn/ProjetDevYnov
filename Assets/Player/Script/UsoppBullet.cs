using Mirror;
using UnityEngine;

public class UsoppBullet : NetworkBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private GameObject owner;

    public LayerMask targetLayer;

    public void Initialize(Vector2 direction, float speed, int damage, GameObject owner)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.owner = owner;

        Destroy(gameObject, 5f); // sécurité : se détruit après 5s
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return; // ne touche pas le tireur

        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent<BossController>(out var boss))
            {
                boss.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
