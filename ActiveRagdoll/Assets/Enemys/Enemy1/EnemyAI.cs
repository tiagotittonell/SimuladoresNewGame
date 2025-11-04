
//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections;

//[RequireComponent(typeof(NavMeshAgent))]
//public class EnemyAI : MonoBehaviour
//{
//    [Header("Refs")]
//    public Animator animator;
//    public Transform player;
//    private NavMeshAgent agent;

//    [Header("Rangos")]
//    public float detectionRange = 10f;
//    public float attackRange = 4f;
//    public float attackCooldown = 1.5f;

//    [Header("Ataques")]
//    public int totalAttacks = 3;
//    private bool isAttacking = false;
//    private float lastAttackTime = -999f;

//    [Header("Auto-fix NavMesh")]
//    public float sampleRadius = 2.0f;
//    public int sampleTries = 1;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//    }

//    IEnumerator Start()
//    {
//        yield return null;
//        EnsureOnNavMesh();
//    }

//    void EnsureOnNavMesh()
//    {
//        if (agent.enabled && agent.isOnNavMesh) return;

//        NavMeshHit hit;
//        if (NavMesh.SamplePosition(transform.position, out hit, sampleRadius, NavMesh.AllAreas))
//        {
//            agent.Warp(hit.position);
//        }
//        else
//        {
//            Debug.LogWarning($"{name}: No se encontró NavMesh cerca.");
//        }
//    }

//    void Update()
//    {
//        if (player == null || !agent.enabled) return;

//        if (!agent.isOnNavMesh)
//        {
//            EnsureOnNavMesh();
//            return;
//        }

//        float dist = Vector3.Distance(transform.position, player.position);

//        if (dist > detectionRange)
//        {
//            agent.isStopped = true;
//            animator.SetBool("IsWalking", false);
//            return;
//        }

//        if (dist > attackRange && !isAttacking)
//        {
//            agent.isStopped = false;
//            agent.SetDestination(player.position);
//            animator.SetBool("IsWalking", true);
//        }
//        else
//        {
//            agent.isStopped = true;
//            animator.SetBool("IsWalking", false);

//            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
//                StartCoroutine(AttackRoutine());
//        }
//    }
//    IEnumerator AttackRoutine()
//    {
//        isAttacking = true;
//        lastAttackTime = Time.time;

//        // 👉 Rotar al jugador solo una vez al inicio del ataque
//        Vector3 dir = (player.position - transform.position).normalized;
//        dir.y = 0;
//        transform.rotation = Quaternion.LookRotation(dir);

//        int attackIndex = Random.Range(1, totalAttacks + 1);
//        animator.SetInteger("AttackIndex", attackIndex);
//        animator.SetBool("InAttack", true);

//        // ✨ Desactivar movimiento y rotación del agente mientras ataca
//        agent.isStopped = true;
//        agent.updateRotation = false;

//        // 🗡️ Activar hitbox
//        var weapon = GetComponentInChildren<EnemyDamage>();
//        if (weapon != null) weapon.EnableHitbox();

//        float attackDuration = 1.0f;
//        float dashDistance = 1.5f;
//        float elapsed = 0f;

//        Vector3 startPos = transform.position;
//        Vector3 targetPos = startPos + dir * dashDistance;

//        while (elapsed < attackDuration)
//        {
//            float t = elapsed / attackDuration;
//            agent.Warp(Vector3.Lerp(startPos, targetPos, t));
//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        agent.Warp(targetPos);

//        // 🛑 Desactivar hitbox al terminar
//        if (weapon != null) weapon.DisableHitbox();

//        animator.SetBool("InAttack", false);
//        animator.SetInteger("AttackIndex", 0);

//        // 🕐 Mantener al enemigo quieto luego del ataque
//        yield return StartCoroutine(IdleLock(2f));  // ⏳ 2 segundos quieto

//        // ✅ Reactivar navegación
//        agent.updateRotation = true;
//        agent.isStopped = false;

//        isAttacking = false;
//    }
//    IEnumerator IdleLock(float duration)
//    {
//        Vector3 initialPos = transform.position;
//        float timer = 0f;

//        while (timer < duration)
//        {
//            // Verificar si se movió más de un pequeño umbral
//            float distance = Vector3.Distance(transform.position, initialPos);
//            if (distance > 0.05f) // 🔸 sensibilidad mínima para detectar movimiento real
//            {
//                animator.SetBool("IsWalking", true);
//            }
//            else
//            {
//                animator.SetBool("IsWalking", false);
//            }

//            timer += Time.deltaTime;
//            yield return null;
//        }
//    }

//}using UnityEngine;
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
    public float attackCooldown = 2f;      // tiempo mínimo entre ataques
    public float postAttackIdleTime = 2f;  // tiempo que queda quieto después del ataque
    public int totalAttacks = 3;

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
        agent.updateRotation = false; // rotación manual
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 🔸 Si está fuera del rango de detección → nada
        if (distance > detectionRange)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        // 🔸 Si no está atacando, sigue su comportamiento normal
        if (!isAttacking)
        {
            // Rota siempre hacia el jugador
            FacePlayer();

            // Si está dentro del rango de ataque → atacar
            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                // Si está fuera del rango de ataque → moverse hacia el jugador
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (!player) return;

        animator.SetBool("IsWalking", true);

        Vector3 target = player.position;
        agent.SetDestination(target);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsWalking", false);

        // 👉 Mirar hacia el jugador
        FacePlayer();

        // 👉 Elegir animación de ataque
        int attackIndex = Random.Range(1, totalAttacks + 1);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("InAttack", true);

        // 👉 Activar hitbox
        var weapon = GetComponentInChildren<EnemyDamage>();
        if (weapon != null) weapon.EnableHitbox();

        // Duración estimada de la animación
        yield return new WaitForSeconds(1.2f);

        // 👉 Desactivar hitbox y volver a idle
        if (weapon != null) weapon.DisableHitbox();
        animator.SetBool("InAttack", false);
        animator.SetInteger("AttackIndex", 0);

        lastAttackTime = Time.time;

        // 👉 Esperar un tiempo antes de volver a moverse
        yield return new WaitForSeconds(postAttackIdleTime);

        isAttacking = false;
        animator.SetBool("IsWalking", true);
    }

    private void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
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


