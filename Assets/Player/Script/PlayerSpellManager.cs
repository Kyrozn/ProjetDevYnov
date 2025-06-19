using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
public class PlayerSpellManager : NetworkBehaviour
{
    [Header("Spells")]
    public Spell[] equippedSpells = new Spell[3];
    private double[] lastCastTimes;

    void Start()
    {
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
        if (Input.GetButtonDown("Fire1")) equippedSpells[0].Cast(gameObject);
        if (Input.GetButtonDown("Fire2")) equippedSpells[1].Cast(gameObject);
        if (Input.GetButtonDown("Fire3")) equippedSpells[2].Cast(gameObject);
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
}
