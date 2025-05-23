using UnityEngine;

[CreateAssetMenu(fileName = "HealSpell", menuName = "Spells/Heal")]
public class HealSpell : Spell
{
    public float baseHealAmount = 20f;

    public override void Upgrade(SpellInstance instance)
    {
        base.Upgrade(instance);
        baseHealAmount += 10f * instance.level; // soin augmente avec le niveau
    }

    public void Heal(PlayerStats target)
    {
        // Exemple simple : ajoute de la vie au joueur (à adapter selon ta gestion santé)
        target.Heal(baseHealAmount);
    }
}
