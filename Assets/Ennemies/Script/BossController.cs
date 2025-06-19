using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class BossController : NetworkBehaviour
{
    [SyncVar]
    public int maxHealth = 500;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth = 500;

    private Animator animator;

    [Header("Combat Settings")]
    public float meleeRange = 2f;
    public float rangedRange = 6f;
    public float attackCooldown = 2f;
    public GameObject magicProjectilePrefab;
    public Transform firePoint;

    [Header("Targeting")]
    public LayerMask playerLayer;

    private Transform target;
    private float lastAttackTime;

    [SyncVar]
    public bool isAttacking = false;
    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
        StartCoroutine(TargetingAndCombatRoutine());
    }

    private IEnumerator TargetingAndCombatRoutine()
    {
        while (currentHealth > 0)
        {
            FindClosestPlayer();

            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.position);

                // Déplacement vers le joueur si hors de portée
                if (distance > meleeRange && distance <= 10f) // 10 = distance d'aggro
                {
                    Vector2 dir = (target.position - transform.position).normalized;
                    transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    animator.SetBool("isMoving", false);
                }

                // Attaques
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    if (distance <= meleeRange)
                        MeleeAttack();
                    else if (distance <= rangedRange)
                        RangedAttack();
                }
            }

            yield return null;
        }
    }


    [Server]
    void FindClosestPlayer()
    {
        if (!isServer || currentHealth <= 0) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Transform nearestTarget = GetNearestPlayer();
            if (nearestTarget == null) return;

            float distance = Vector2.Distance(transform.position, nearestTarget.position);

            if (distance <= meleeRange)
            {
                MeleeAttack();
            }
            else if (distance <= rangedRange)
            {
                target = nearestTarget;
                RangedAttack();
            }
        }
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Boss took damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    [Server]
    void Die()
    {
        RpcPlayDeathAnimation();
        RpcChangeToWinScreen();
        StartCoroutine(DelayedDestroy(2f));
    }

    [ClientRpc]
    void RpcPlayDeathAnimation()
    {
        animator.SetTrigger("Die");
    }

    [ClientRpc]
    void RpcChangeToWinScreen()
    {
        SceneManager.LoadScene("Win");
    }

    [Server]
    IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkServer.Destroy(gameObject);
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"[Sync] Boss HP: {newHealth}/{maxHealth}");
    }

    [Server]
    void MeleeAttack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        animator.SetTrigger("Claw");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerStats>(out var player))
            {
                player.TakeDamage(20);
            }
        }

        Invoke(nameof(StopAttacking), 0.5f);
    }

    [Server]
    void RangedAttack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        animator.SetTrigger("Cast");

        if (magicProjectilePrefab && firePoint && target != null)
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

    [Server]
    Transform GetNearestPlayer()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, rangedRange, playerLayer);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var col in players)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }
}
