using Mirror;
using UnityEngine;
using System.Collections;

public class SanjiController : PlayerController
{
    [Header("Kick Settings")]
    public float kickDistance = 1f;
    public Vector2 kickBoxSize = new Vector2(0.5f, 0.5f);
    public int kickDamage = 10;

    private Transform caster;

    private void Awake()
    {
        if (caster == null)
            caster = transform;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void CastKick(float distance, Vector2 boxSize, int damage)
    {
        if (!isLocalPlayer) return;
        CmdCastKick(distance, boxSize, damage);
    }

    [Command]
    void CmdCastKick(float distance, Vector2 boxSize, int damage)
    {
        RpcPlayKickAnimation();

        Vector3 attackCenter = caster.position + caster.right * distance;
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, boxSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Sanji a touché : " + hit.name);
            if (hit.TryGetComponent<BossController>(out var boss))
            {
                boss.TakeDamage(damage);
            }
        }
    }

    [ClientRpc]
    void RpcPlayKickAnimation()
    {
        StartCoroutine(PlayKickAnimation());
    }

    private IEnumerator PlayKickAnimation()
    {
        animator.SetBool("SimpleAttack", true);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetBool("SimpleAttack", false);
    }

    // // Pour débogage
    // private void OnDrawGizmosSelected()
    // {
    //     if (caster == null) caster = transform;
    //     Vector3 attackCenter = caster.position + caster.right * kickDistance;
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireCube(attackCenter, kickBoxSize);
    // }
}
