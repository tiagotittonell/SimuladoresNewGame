using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private float lastAttackTime = 0f;
    private EnemyHealth health;
    private Rigidbody rb;

    float startWalkDist;  // histéresis
    float stopWalkDist;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();
        startWalkDist = attackRange + 0.3f;
        stopWalkDist = attackRange;

        // Opcional: forzar sin Root Motion
        if (animator != null) animator.applyRootMotion = false;
    }

    void Update()
    {
        if (health != null && health.IsDead()) return;
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Histéresis para IsWalking
        bool wantsWalk = animator.GetBool("IsWalking");
        if (!wantsWalk && dist > startWalkDist) wantsWalk = true;
        if (wantsWalk && dist <= stopWalkDist) wantsWalk = false;
        animator.SetBool("IsWalking", wantsWalk);

        // Ataque
        if (dist <= attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (health != null && health.IsDead()) return;
        if (!player) return;

        if (animator.GetBool("IsWalking"))
        {
            Vector3 dir = (player.position - transform.position);
            dir.y = 0f;
            dir.Normalize();

            rb.MovePosition(transform.position + dir * moveSpeed * Time.fixedDeltaTime);

            Quaternion lookRot = Quaternion.LookRotation(dir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRot, 10f * Time.fixedDeltaTime));
        }
    }
}
