using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell")]
public abstract class Spell : ScriptableObject
{
    public float cooldown = 1f;
    public abstract void Cast(GameObject caster);
}
