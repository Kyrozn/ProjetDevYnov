using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Gatling", menuName = "Spells/Gatling")]
public class Gatling : Spell
{
    [Header("Gatling Settings")]
    public int damage = 10;
    public Vector2 boxSize = new Vector2(0.5f, 0.5f);
    public float attackDistance = 1f;
    

    public override void Cast(GameObject caster)
    {
        LuffyController controller = caster.GetComponent<LuffyController>();
        if (controller != null)
        {
            controller.CastGatling(attackDistance, boxSize, damage);
        }
    }
}