using UnityEngine;
using Mirror;

public class ZoroController : PlayerController
{
    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;

    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashTimer;
    private float dashSpeed;

    protected override void Update()
    {
        if (!isLocalPlayer) return;

        if (isDashing)
        {
            DashMovement();
            return;
        }

        base.Update();
    }

    public void TryDash()
    {
        if (!isDashing)
        {
            Vector3 dir = new Vector3(h, v, 0f);
            if (dir == Vector3.zero)
                dir = Vector3.right;

            StartDash(dir.normalized, dashDistance, dashDuration);
            CmdPlayDashAnimation(); // uniquement animation sur tous
        }
    }

    [Command]
    void CmdPlayDashAnimation()
    {
        RpcPlayDashAnimation();
    }

    [ClientRpc]
    void RpcPlayDashAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }
    }

    void StartDash(Vector3 direction, float distance, float duration)
    {
        dashDirection = direction;
        dashSpeed = distance / duration;
        dashTimer = duration;
        isDashing = true;

        if (animator != null && isLocalPlayer)
        {
            animator.SetTrigger("Dash");
        }
    }

    private void DashMovement()
    {
        transform.position += dashDirection * dashSpeed * Time.deltaTime;
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }
}
