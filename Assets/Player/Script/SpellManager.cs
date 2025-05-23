using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : NetworkBehaviour
{
    public List<SpellInstance> learnedSpells = new();
    public List<Spell> allSpells; // assigner dans l'inspecteur avec tous les sorts disponibles

    private List<Spell> currentChoices = new();

    public delegate void SpellChoiceDelegate(List<Spell> spells);
    public event SpellChoiceDelegate OnSpellChoices;

    [Server]
    public void ProposeNewSpells()
    {
        currentChoices.Clear();

        // Tirer 3 sorts aléatoires parmi allSpells
        while (currentChoices.Count < 3 && currentChoices.Count < allSpells.Count)
        {
            Spell s = allSpells[Random.Range(0, allSpells.Count)];
            if (!currentChoices.Contains(s))
                currentChoices.Add(s);
        }

        TargetShowChoices(connectionToClient, currentChoices.ToArray());
    }

    [TargetRpc]
    void TargetShowChoices(NetworkConnection target, Spell[] spells)
    {
        OnSpellChoices?.Invoke(new List<Spell>(spells));
    }

    [Command]
    public void CmdChooseSpell(string spellName)
    {
        Spell chosenSpell = allSpells.Find(s => s.spellName == spellName);
        if (chosenSpell == null)
            return;

        SpellInstance existing = learnedSpells.Find(si => si.spell == chosenSpell);
        if (existing != null)
        {
            existing.Upgrade();
            Debug.Log($"Sort amélioré : {chosenSpell.spellName} Niveau {existing.level}");
        }
        else
        {
            learnedSpells.Add(new SpellInstance(chosenSpell));
            Debug.Log($"Sort appris : {chosenSpell.spellName}");
        }
    }
}
