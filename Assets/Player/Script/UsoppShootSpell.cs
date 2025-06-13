using UnityEngine;

[CreateAssetMenu(fileName = "UsoppShoot", menuName = "Spells/UsoppShoot")]
public class UsoppShootSpell : Spell
{
    public override void Cast(GameObject caster)
    {
        UsoppController controller = caster.GetComponent<UsoppController>();
        if (controller != null)
        {
            controller.CastShoot();
        }
        else
        {
            Debug.LogWarning($"UsoppShoot: Aucun UsoppController sur {caster.name}");
        }
    }
}
