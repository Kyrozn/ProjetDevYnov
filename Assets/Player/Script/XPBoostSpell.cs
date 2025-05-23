using UnityEngine;

[CreateAssetMenu(fileName = "XPBoostSpell", menuName = "Spells/XPBoost")]
public class XPBoostSpell : Spell
{
    public float xpMultiplier = 1.2f;

    public override void Upgrade(SpellInstance instance)
    {
        base.Upgrade(instance);
        xpMultiplier += 0.1f * instance.level; // boost d'XP augmente avec le niveau
    }

    // Exemple d’application du boost : à intégrer dans la logique de gain XP du joueur
    public float GetMultiplier()
    {
        return xpMultiplier;
    }
}
