using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public int maxLevel = 3;

    [TextArea] public string description;

    // Amélioration du sort
    public virtual void Upgrade(SpellInstance instance)
    {
        instance.level = Mathf.Min(instance.level + 1, maxLevel);
    }
}
