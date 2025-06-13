using Mirror;
using UnityEngine;
using System.Collections;

public class UsoppController : PlayerController
{
    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public int bulletDamage = 8;

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

    public void CastShoot()
    {
        if (!isLocalPlayer) return;
        CmdShoot(caster.right); // direction par défaut : vers la droite du joueur
    }

    [Command]
    void CmdShoot(Vector2 direction)
    {
        RpcPlayShootAnimation();

        GameObject bullet = Instantiate(bulletPrefab, caster.position, Quaternion.identity);
        if (bullet.TryGetComponent(out UsoppBullet bulletScript))
        {
            bulletScript.Initialize(direction, bulletSpeed, bulletDamage, gameObject);
        }

        NetworkServer.Spawn(bullet);
    }

    [ClientRpc]
    void RpcPlayShootAnimation()
    {
        StartCoroutine(PlayShootAnimation());
    }

    private IEnumerator PlayShootAnimation()
    {
        animator.SetBool("SimpleAttack", true);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetBool("SimpleAttack", false);
    }
}
