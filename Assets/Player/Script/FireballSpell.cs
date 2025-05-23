using UnityEngine;

[CreateAssetMenu(fileName = "FireballSpell", menuName = "Spells/Fireball")]
public class FireballSpell : Spell
{
    public GameObject fireballPrefab;
    public float baseDamage = 10f;

    public override void Upgrade(SpellInstance instance)
    {
        base.Upgrade(instance);
        baseDamage += 5f * instance.level; // augmente les dégâts à chaque niveau
    }

    public void Cast(Vector3 position, Vector3 direction)
    {
        GameObject fireball = Instantiate(fireballPrefab, position, Quaternion.identity);
        // Ici tu peux ajouter un script Fireball pour gérer déplacement/dégâts
    }
}
