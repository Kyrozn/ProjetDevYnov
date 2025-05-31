using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
public class PlayerSpellManager : NetworkBehaviour
{
    [Header("Spells")]
    public Spell[] equippedSpells = new Spell[3];

    private Animator animator;
    private double[] lastCastTimes;

    void Start()
    {
        animator = GetComponent<Animator>();
        InitCooldowns();
    }

    void InitCooldowns()
    {
        lastCastTimes = new double[equippedSpells.Length];
        for (int i = 0; i < lastCastTimes.Length; i++)
        {
            lastCastTimes[i] = -999; // cooldown initial
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Gestion des entrées (à adapter selon ton mapping)
        if (Input.GetButtonDown("Fire1")) TryRequestCast(0);
        if (Input.GetButtonDown("Fire2")) TryRequestCast(1);
        if (Input.GetButtonDown("Fire3")) TryRequestCast(2);
    }

    void TryRequestCast(int index)
    {
        if (index < 0 || index >= equippedSpells.Length)
        {
            Debug.LogWarning($"Tentative de cast sur un index invalide : {index}");
            return;
        }

        CmdCastSpell(index);
    }

    [Command]
    void CmdCastSpell(int index)
    {
        if (!IsValidSpell(index, out Spell spell)) return;

        double now = NetworkTime.time;
        if (now < lastCastTimes[index] + spell.cooldown)
        {
            Debug.Log($"[Server] Sort {index} encore en cooldown pour {gameObject.name}");
            return;
        }

        lastCastTimes[index] = now;

        // Logique principale de cast
        spell.Cast(gameObject);

        // Animation pour les clients
        RpcPlaySpellAnimation(index);
    }

    bool IsValidSpell(int index, out Spell spell)
    {
        spell = null;

        if (index < 0 || index >= equippedSpells.Length)
        {
            Debug.LogWarning($"[Server] Index de sort invalide : {index}");
            return false;
        }

        spell = equippedSpells[index];
        if (spell == null)
        {
            Debug.LogWarning($"[Server] Aucun sort équipé à l'index {index}");
            return false;
        }

        return true;
    }

    [ClientRpc]
    void RpcPlaySpellAnimation(int index)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"Animator manquant sur {gameObject.name}");
                return;
            }
        }

        string animTrigger = $"Spell{index + 1}";
        animator.SetTrigger(animTrigger);
    }
}
