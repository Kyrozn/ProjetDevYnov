using Mirror;
using UnityEngine;

public class LuffyController : PlayerController
{
    private Transform caster;
    private void Awake()
    {
        if (caster == null)
            caster = transform;
    }

    protected override void Update()
    {
        base.Update(); // Appelle l'Update du parent
    }
    public void CastGatling(float attackDistance, Vector2 boxSize, int damage)
    {
        if (!isLocalPlayer) return;
        CmdCastGatling(attackDistance, boxSize, damage);
    }

    [Command]
    void CmdCastGatling(float attackDistance, Vector2 boxSize, int damage)
    {
        RpcPlayGatling();
        Vector3 attackCenter = caster.position + caster.right * attackDistance;

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, boxSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Cible touchée : " + hit.name);
            // applique dégâts si boss :
            if (hit.TryGetComponent<BossController>(out var boss))
            {
                boss.TakeDamage(damage);
            }
        }
    }

    [ClientRpc]
    void RpcPlayGatling()
    {
        StartCoroutine(ResetGatlingBool());
    }

    protected virtual System.Collections.IEnumerator ResetGatlingBool()
    {
        animator.SetBool("SimpleAttack", true);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool("SimpleAttack", false);
    }
    // private void OnDrawGizmosSelected()
    // {
    //     if (caster == null) caster = transform;
    //     Vector3 attackCenter = caster.position + caster.right * attackDistance;
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(attackCenter, boxSize);
    // }
}
