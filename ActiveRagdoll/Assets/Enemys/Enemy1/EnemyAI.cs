
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
//    public float attackRange = 2f;
//    public float attackCooldown = 1.5f;

//    [Header("Ataques")]
//    public int totalAttacks = 3;
//    private bool isAttacking = false;
//    private float lastAttackTime = -999f;

//    [Header("Auto-fix NavMesh")]
//    public float sampleRadius = 2.0f;  // cuánto buscar alrededor
//    public int sampleTries = 1;        // intenta poner al agent en el mesh al iniciar

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//    }

//    IEnumerator Start()
//    {
//        // Espera un frame por si spawneas al principio
//        yield return null;

//        // Si no está en el NavMesh, intenta colocarlo
//        EnsureOnNavMesh();
//    }

//    void EnsureOnNavMesh()
//    {
//        if (agent.enabled && agent.isOnNavMesh) return;

//        NavMeshHit hit;
//        if (NavMesh.SamplePosition(transform.position, out hit, sampleRadius, NavMesh.AllAreas))
//        {
//            // Coloca al agent exactamente sobre el NavMesh
//            agent.Warp(hit.position);
//        }
//        else
//        {
//            Debug.LogWarning($"{name}: No se encontró NavMesh cerca. Asegúrate de hornearlo y de que el enemigo spawnee sobre él.");
//        }
//    }

//    void Update()
//    {
//        if (player == null || !agent.enabled) return;

//        // Si por alguna razón se “cayó” del NavMesh, intenta reubicarlo y corta el frame
//        if (!agent.isOnNavMesh)
//        {
//            EnsureOnNavMesh();
//            return;
//        }

//        float dist = Vector3.Distance(transform.position, player.position);

//        // Fuera de rango de detección → idle
//        if (dist > detectionRange)
//        {
//            agent.isStopped = true;
//            animator.SetBool("IsWalking", false);
//            return;
//        }

//        // Dentro de detección pero fuera de ataque → caminar
//        if (dist > attackRange && !isAttacking)
//        {
//            agent.isStopped = false;               // ¡en vez de Resume()!
//            agent.SetDestination(player.position);
//            animator.SetBool("IsWalking", true);
//        }
//        else
//        {
//            // En rango de ataque
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

//        // Mirar al jugador
//        Vector3 dir = (player.position - transform.position).normalized;
//        dir.y = 0;
//        transform.rotation = Quaternion.LookRotation(dir);

//        int attackIndex = Random.Range(1, totalAttacks + 1);
//        animator.SetInteger("AttackIndex", attackIndex);
//        animator.SetBool("InAttack", true);

//        float attackDuration = 1.0f;      // duración real de la animación
//        float dashDistance = 1.5f;        // cuánto avanza el enemigo
//        float elapsed = 0f;

//        Vector3 startPos = transform.position;
//        Vector3 targetPos = startPos + dir * dashDistance;

//        // Mover suavemente al enemigo mientras dura la animación
//        while (elapsed < attackDuration)
//        {
//            float t = elapsed / attackDuration;
//            agent.Warp(Vector3.Lerp(startPos, targetPos, t));
//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        agent.Warp(targetPos); // asegurar posición final
//        animator.SetBool("InAttack", false);
//        animator.SetInteger("AttackIndex", 0);

//        yield return new WaitForSeconds(0.3f);
//        isAttacking = false;
//    }

//}

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Transform player;
    private NavMeshAgent agent;

    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 4f;
    public float attackCooldown = 1.5f;

    [Header("Ataques")]
    public int totalAttacks = 3;
    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    [Header("Auto-fix NavMesh")]
    public float sampleRadius = 2.0f;
    public int sampleTries = 1;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    IEnumerator Start()
    {
        yield return null;
        EnsureOnNavMesh();
    }

    void EnsureOnNavMesh()
    {
        if (agent.enabled && agent.isOnNavMesh) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning($"{name}: No se encontró NavMesh cerca.");
        }
    }

    void Update()
    {
        if (player == null || !agent.enabled) return;

        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > detectionRange)
        {
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
            return;
        }

        if (dist > attackRange && !isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);

            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                StartCoroutine(AttackRoutine());
        }
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 👉 Rotar al jugador solo una vez al inicio del ataque
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        int attackIndex = Random.Range(1, totalAttacks + 1);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("InAttack", true);

        // ✨ Desactivar movimiento y rotación del agente mientras ataca
        agent.isStopped = true;
        agent.updateRotation = false;

        // 🗡️ Activar hitbox
        var weapon = GetComponentInChildren<EnemyDamage>();
        if (weapon != null) weapon.EnableHitbox();

        float attackDuration = 1.0f;
        float dashDistance = 1.5f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir * dashDistance;

        while (elapsed < attackDuration)
        {
            float t = elapsed / attackDuration;
            agent.Warp(Vector3.Lerp(startPos, targetPos, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.Warp(targetPos);

        // 🛑 Desactivar hitbox al terminar
        if (weapon != null) weapon.DisableHitbox();

        animator.SetBool("InAttack", false);
        animator.SetInteger("AttackIndex", 0);

        // 🕐 Mantener al enemigo quieto luego del ataque
        yield return StartCoroutine(IdleLock(2f));  // ⏳ 2 segundos quieto

        // ✅ Reactivar navegación
        agent.updateRotation = true;
        agent.isStopped = false;

        isAttacking = false;
    }
    IEnumerator IdleLock(float duration)
    {
        Vector3 initialPos = transform.position;
        float timer = 0f;

        while (timer < duration)
        {
            // Verificar si se movió más de un pequeño umbral
            float distance = Vector3.Distance(transform.position, initialPos);
            if (distance > 0.05f) // 🔸 sensibilidad mínima para detectar movimiento real
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    //IEnumerator AttackRoutine()
    //{
    //    isAttacking = true;
    //    lastAttackTime = Time.time;

    //    // 👉 Rotar al jugador solo una vez al inicio del ataque
    //    Vector3 dir = (player.position - transform.position).normalized;
    //    dir.y = 0;
    //    transform.rotation = Quaternion.LookRotation(dir);

    //    int attackIndex = Random.Range(1, totalAttacks + 1);
    //    animator.SetInteger("AttackIndex", attackIndex);
    //    animator.SetBool("InAttack", true);

    //    // ✨ Desactivar la rotación automática del agente mientras ataca
    //    agent.isStopped = true;
    //    agent.updateRotation = false;

    //    // 🗡️ Activar hitbox al iniciar
    //    var weapon = GetComponentInChildren<EnemyDamage>();
    //    if (weapon != null) weapon.EnableHitbox();

    //    float attackDuration = 1.0f;
    //    float dashDistance = 1.5f;
    //    float elapsed = 0f;

    //    Vector3 startPos = transform.position;
    //    Vector3 targetPos = startPos + dir * dashDistance;

    //    while (elapsed < attackDuration)
    //    {
    //        float t = elapsed / attackDuration;
    //        agent.Warp(Vector3.Lerp(startPos, targetPos, t));
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    agent.Warp(targetPos);

    //    // 🛑 Desactivar hitbox al terminar
    //    if (weapon != null) weapon.DisableHitbox();

    //    animator.SetBool("InAttack", false);
    //    animator.SetInteger("AttackIndex", 0);

    //    // 🕐 Pausa post ataque para dar oportunidad al jugador
    //    yield return new WaitForSeconds(1.5f);

    //    // ✅ Reactivar la rotación y navegación
    //    agent.updateRotation = true;
    //    agent.isStopped = false;

    //    isAttacking = false;
    //}


    public void DisableAI()
    {
        enabled = false;
        if (agent != null)
        {
            agent.enabled = false;
        }
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("InAttack", false);
        }
    }
}


