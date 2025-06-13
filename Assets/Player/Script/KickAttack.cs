using UnityEngine;

[CreateAssetMenu(fileName = "KickAttack", menuName = "Spells/KickAttack")]
public class KickAttack : Spell
{
    public float attackDistance = 1f;
    public Vector2 boxSize = new Vector2(0.5f, 0.5f);
    public int damage = 10;

    public override void Cast(GameObject caster)
    {
        SanjiController controller = caster.GetComponent<SanjiController>();
        if (controller != null)
        {
            controller.CastKick(attackDistance, boxSize, damage);
        }
        else
        {
            Debug.LogWarning($"KickAttack: Le caster {caster.name} n'a pas de SanjiController.");
        }
    }
}
