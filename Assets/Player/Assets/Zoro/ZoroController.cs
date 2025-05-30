
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
        if (isDashing)
        {
            DashMovement();
            return;
        }
        base.Update();
    }

    public void TryDash()
    {
        if (!isLocalPlayer) return;
        if (!isDashing)
            CmdDash(h, v);
    }

    [Command]
    void CmdDash(float inputH, float inputV)
    {
        RpcPlayDash(inputH, inputV); 
    }

    [ClientRpc]
    void RpcPlayDash(float inputH, float inputV)
    {
        StartDash(inputH, inputV, dashDistance, dashDuration);
    }

    public void StartDash(float inputH, float inputV, float distance, float duration)
    {
        dashDirection = new Vector3(inputH, inputV, 0f).normalized;
        if (dashDirection == Vector3.zero)
            dashDirection = Vector3.right;

        dashSpeed = distance / duration;
        dashTimer = duration;
        isDashing = true;

        if (animator != null)
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
