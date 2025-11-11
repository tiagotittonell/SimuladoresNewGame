//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections;

//[RequireComponent(typeof(NavMeshAgent))]
//public class EnemyAI : MonoBehaviour
//{
//    [Header("Referencias")]
//    [SerializeField] private Transform player;
//    [SerializeField] private Animator animator;
//    private NavMeshAgent agent;

//    [Header("Movimiento")]
//    public float detectionRange = 20f;
//    public float attackRange = 6f;
//    public float moveSpeed = 3.5f;
//    public float rotationSpeed = 6f;

//    [Header("Ataque")]
//    public float attackCooldown = 2f;      
//    public float postAttackIdleTime = 2f;  
//    public int totalAttacks = 2;
//    [SerializeField] private EnemyHitbox hitbox;

//    private bool isAttacking = false;
//    private float lastAttackTime = -999f;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        if (animator == null) animator = GetComponent<Animator>();
//    }

//    void Start()
//    {
//        agent.speed = moveSpeed;
//        agent.updateRotation = false; 
//    }

//    void Update()
//    {
//        if (!player) return;

//        float distance = Vector3.Distance(transform.position, player.position);

//        if (distance > detectionRange)
//        {
//            animator.SetBool("IsWalking", false);
//            return;
//        }

//        if (!isAttacking)
//        {
//            FacePlayer();

//            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
//            {
//                StartCoroutine(AttackRoutine());
//            }
//            else
//            {
//                MoveTowardsPlayer();
//            }
//        }
//    }

//    private void MoveTowardsPlayer()
//    {
//        if (!player) return;

//        animator.SetBool("IsWalking", true);

//        Vector3 target = player.position;
//        agent.SetDestination(target);
//    }
//    public void EnableHitbox()
//    {
//        if (hitbox != null)
//            hitbox.EnableHitbox();
//    }

//    public void DisableHitbox()
//    {
//        if (hitbox != null)
//            hitbox.DisableHitbox();
//    }
//    public void EndAttack()
//    {
//        animator.SetBool("InAttack", false);
//        animator.SetInteger("AttackIndex", -1);
//        isAttacking = false;
//        animator.SetBool("IsWalking", true); // ← Volvemos al caminar
//    }

//    private IEnumerator AttackRoutine()
//    {
//        isAttacking = true;
//        animator.SetBool("IsWalking", false);

//        FacePlayer();

//        // Elegir cuál ataque usar (1 o 2)
//        int attackIndex = Random.Range(1, totalAttacks + 1);

//        animator.SetInteger("AttackIndex", attackIndex);
//        animator.SetBool("InAttack", true);

//        // IMPORTANTE → No activamos/desactivamos hitbox acá
//        // Eso lo hace la animación usando eventos

//        // Espera el tiempo de cooldown ANTES de poder atacar otra vez
//        lastAttackTime = Time.time;

//        // Espera el tiempo de relajación después del ataque
//        yield return new WaitForSeconds(postAttackIdleTime);

//        // NO restablecemos nada acá → lo hace EndAttack() desde la animación
//    }


//    //private IEnumerator AttackRoutine()
//    //{
//    //    isAttacking = true;
//    //    animator.SetBool("IsWalking", false);

//    //    FacePlayer();

//    //    int attackIndex = Random.Range(1, totalAttacks + 1);
//    //    animator.SetInteger("AttackIndex", attackIndex);
//    //    animator.SetBool("InAttack", true);

//    //    var weapon = GetComponentInChildren<EnemyDamage>();
//    //    if (weapon != null) weapon.EnableHitbox();

//    //    yield return new WaitForSeconds(1.2f);

//    //    if (weapon != null) weapon.DisableHitbox();
//    //    animator.SetBool("InAttack", false);
//    //    animator.SetInteger("AttackIndex", -1);

//    //    lastAttackTime = Time.time;

//    //    yield return new WaitForSeconds(postAttackIdleTime);

//    //    isAttacking = false;
//    //    animator.SetBool("IsWalking", true);
//    //}

//    private void FacePlayer()
//    {
//        Vector3 dir = (player.position - transform.position);
//        dir.y = 0;
//        if (dir.sqrMagnitude > 0.001f)
//        {
//            Quaternion targetRot = Quaternion.LookRotation(dir);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectionRange);
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }
//}
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    [Header("Movimiento")]
    public float detectionRange = 20f;
    public float attackRange = 6f;
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 6f;

    [Header("Ataque")]
    public float attackCooldown = 2f;
    public float postAttackIdleTime = 2f;
    public int totalAttacks = 2; // AttackIndex = 1 or 2
    [SerializeField] private EnemyHitbox hitbox; // hitbox activado por eventos

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        agent.speed = moveSpeed;
        agent.updateRotation = false; // Rotación manual
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Fuera de rango → no hacemos nada
        if (distance > detectionRange)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        if (!isAttacking)
        {
            FacePlayer();

            // Si está en rango → atacar
            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        animator.SetBool("IsWalking", true);
        agent.SetDestination(player.position);
    }
    public void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (hitbox != null)
            hitbox.DisableHitbox();
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsWalking", false);

        FacePlayer();

        int attackIndex = Random.Range(1, totalAttacks + 1);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("InAttack", true);

        //var weapon = GetComponentInChildren<EnemyDamage>();
        //if (weapon != null) weapon.EnableHitbox();

        yield return new WaitForSeconds(1.2f);

        //if (weapon != null) weapon.DisableHitbox();
        animator.SetBool("InAttack", false);
        animator.SetInteger("AttackIndex", 0);

        lastAttackTime = Time.time;

        yield return new WaitForSeconds(postAttackIdleTime);

        isAttacking = false;
        animator.SetBool("IsWalking", true);
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
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




