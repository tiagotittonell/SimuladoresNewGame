using UnityEngine;
using System.Collections;

public class MageAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    private Animator animator;
    private EnemyRagdoll ragdoll;

    [Header("Rangos y movimiento")]
    public float detectionRange = 15f;  
    public float safeDistance = 7f;     
    public float attackRange = 10f;      
    public float moveSpeed = 2f;
    public float strafeSpeed = 1.8f;
    public float turnSpeed = 5f;

    [Header("Ataque")]
    public float attackCooldown = 4f;
    private bool isAttacking = false;
    private bool isIdleCooldown = false;

    [Header("FireBall")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint; 
    void Start()
    {
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<EnemyRagdoll>();

        if (ragdoll != null)
            ragdoll.SetRagdollActive(false);

    }

    void Update()
    {
        if (player == null || isIdleCooldown) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * turnSpeed);

        if (distance <= detectionRange)
        {
            if (!isAttacking)
            {
                if (distance <= safeDistance)
                {
                    MoveBackOrStrafe();
                }
                else if (distance <= attackRange)
                {
                    StartCoroutine(AttackRoutine());
                }
                else
                {
                    MoveForward();
                }
            }
        }
        else
        {
            SetIdle();
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        SetIdle();

        animator.SetBool("IsAttacking", true);
        Debug.Log("🔮 Mago: animación de ataque iniciada");
        yield return new WaitForSeconds(1.03f);

        if (fireballPrefab && firePoint && player)
        {
            Vector3 direction = player.position - firePoint.position;
            direction.y = 0;
            direction.Normalize();

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            var fireball = Instantiate(fireballPrefab, firePoint.position, lookRotation);

            Collider fireballCol = fireball.GetComponent<Collider>();
            Collider myCol = GetComponent<Collider>();
            if (fireballCol != null && myCol != null)
            {
                Physics.IgnoreCollision(fireballCol, myCol);
            }

            FireballMove fbMove = fireball.AddComponent<FireballMove>();
            fbMove.speed = 20f;

            Debug.Log("Fireball lanzada");
        }


        yield return new WaitForSeconds(1.5f); 
        animator.SetBool("IsAttacking", false);

        isIdleCooldown = true;
        yield return new WaitForSeconds(3f); 
        isIdleCooldown = false;
        isAttacking = false;
    }

    void MoveForward()
    {
        ResetAllMovementBools();
        animator.SetBool("IsApproaching", true);

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        Debug.Log("🧙‍♂️ Caminando hacia el jugador");
    }

    void MoveBackOrStrafe()
    {
        ResetAllMovementBools();

        float dir = Random.Range(-1f, 1f);
        if (dir > 0.5f)
        {
            animator.SetBool("IsStrafingRight", true);
            transform.Translate(Vector3.right * strafeSpeed * Time.deltaTime);
        }
        else if (dir < -0.5f)
        {
            animator.SetBool("IsStrafingLeft", true);
            transform.Translate(Vector3.left * strafeSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMovingBack", true);
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

    }

    void SetIdle()
    {
        ResetAllMovementBools();
        animator.SetBool("IsApproaching", false);
        animator.SetBool("IsAttacking", false);
    }

    void ResetAllMovementBools()
    {
        animator.SetBool("IsMovingBack", false);
        animator.SetBool("IsStrafingLeft", false);
        animator.SetBool("IsStrafingRight", false);
        animator.SetBool("IsApproaching", false);
    }
}
