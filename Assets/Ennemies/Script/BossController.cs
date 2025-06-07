using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
public class BossController : NetworkBehaviour
{
    [SyncVar]
    public int maxHealth = 500;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;

    private Animator animator;

    [Header("Combat Settings")]
    public float meleeRange = 2f;
    public float rangedRange = 6f;
    public float attackCooldown = 2f;
    public GameObject magicProjectilePrefab;
    public Transform firePoint;

    private Transform target;
    private float lastAttackTime;
    [SyncVar] public bool isAttacking = false;


    void Start()
    {
        animator = GetComponent<Animator>();

        if (isServer)
        {
            currentHealth = maxHealth;
            target = GameObject.FindWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (!isServer || target == null || currentHealth <= 0) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (distance <= meleeRange)
            {
                MeleeAttack();
            }
            else if (distance <= rangedRange)
            {
                RangedAttack();
            }
        }
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [Server]
    void Die()
    {
        RpcPlayDeathAnimation();
        Destroy(gameObject, 2f);
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"Boss HP: {newHealth}/{maxHealth}");
    }

    [ClientRpc]
    void RpcPlayDeathAnimation()
    {
        animator.SetTrigger("Die");
    }

    [Server]
    void MeleeAttack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Claw");

        if (target != null)
        {
            target.GetComponent<PlayerStats>()?.TakeDamage(20);
        }
        Invoke(nameof(StopAttacking), 0.5f);
    }

    [Server]
    void RangedAttack()
{
    lastAttackTime = Time.time;
    isAttacking = true;

    animator.SetTrigger("Cast");

    if (magicProjectilePrefab && firePoint)
    {
        GameObject proj = Instantiate(magicProjectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (target.position - firePoint.position).normalized;
        proj.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f;

        NetworkServer.Spawn(proj);
    }

    Invoke(nameof(StopAttacking), 0.5f);
}

[Server]
void StopAttacking()
{
    isAttacking = false;
}
    // public override void OnStartServer()
    // {
    //     base.OnStartServer();
    //     currentHealth = maxHealth;
    // }

    // public override void OnStopServer()
    // {
    //     base.OnStopServer();
    //     Destroy(gameObject);
    // }
}
