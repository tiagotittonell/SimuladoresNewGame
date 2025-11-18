using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BerserkerAI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyDamage swordHitbox; 

    private Rigidbody rb;

    [Header("Comportamiento")]
    public float detectionRange = 18f;
    public float attackRange = 4.5f;
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 6f;
    public int totalAttacks = 2;
    public float attackCooldown = 1.2f;
    public float restDuration = 3f; 

    [Header("Balance de agresión")]
    public int attacksBeforeRest = 5; 

    private bool isAttacking = false;
    private bool isResting = false;
    private float lastAttackTime = -999f;
    private int attackCounter = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!player || isResting) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        if (isAttacking)
        {
            FacePlayer();
            return;
        }

        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            animator.SetBool("IsRunning", true);
            if (animator.GetBool("IsRunning"))
                MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        FacePlayer();
        animator.SetBool("IsRunning", true);
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsRunning", false);
        FacePlayer();

        int attackIndex = Random.Range(1, totalAttacks + 1);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("InAttack", true);

        lastAttackTime = Time.time;
        attackCounter++;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetBool("InAttack", false);
        animator.SetInteger("AttackIndex", 0);

        isAttacking = false;

        if (attackCounter >= attacksBeforeRest)
        {
            StartCoroutine(RestRoutine());
        }
    }

    private IEnumerator RestRoutine()
    {
        isResting = true;
        attackCounter = 0;
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Tired"); 
        yield return new WaitForSeconds(restDuration);
        isResting = false;
    }

    public void EnableHitbox() => swordHitbox?.EnableHitbox();
    public void DisableHitbox() => swordHitbox?.DisableHitbox();

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
