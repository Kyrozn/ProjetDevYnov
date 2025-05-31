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

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isServer) // initialiser uniquement sur le serveur
        {
            currentHealth = maxHealth;
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
        // Désactiver IA, collisions, etc.
        RpcPlayDeathAnimation();
        Destroy(gameObject, 2f); // délai pour laisser l’anim jouer
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        // Affichage d'une barre de vie par exemple ?
        Debug.Log($"Boss HP: {newHealth}/{maxHealth}");
    }

    [ClientRpc]
    void RpcPlayDeathAnimation()
    {
        animator.SetTrigger("Die"); // à définir dans Animator
    }
}
