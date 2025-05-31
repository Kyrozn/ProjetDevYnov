using Mirror;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] public LayerMask enemyLayer;
    
    [Header("Déplacement")]
    public float speed = 5f;
    protected float h = 0f;
    protected float v = 0f;
    protected Animator animator;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Move();
        UpdateAnimator();
    }
    protected virtual void Move()
    {
        Vector3 move = new Vector3(h, v, 0f) * speed * Time.deltaTime;
        transform.position += move;
    }

    protected virtual void UpdateAnimator()
    {
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
    }
}