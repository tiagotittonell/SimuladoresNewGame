//using UnityEngine;

//public class EnemyHealth : MonoBehaviour
//{
//    public int maxHealth = 50;
//    private int currentHealth;
//    private EnemyRagdoll ragdoll;

//    void Start()
//    {
//        currentHealth = maxHealth;
//        ragdoll = GetComponent<EnemyRagdoll>();
//    }

//    public void TakeDamage(int amount)
//    {
//        if (IsDead()) return; // ignorar daño si ya está muerto

//        currentHealth -= amount;
//        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

//        if (currentHealth <= 0)
//            Die();
//    }

//    public void Die()
//    {
//        Debug.Log(gameObject.name + " murió!");

//        if (ragdoll != null)
//        {
//            ragdoll.SetRagdollActive(true); // activar ragdoll
//        }

//        // Opcional: desactivar este script para que no reciba más daño
//        this.enabled = false;
//    }

//    public bool IsDead()
//    {
//        return currentHealth <= 0;
//    }
//}
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;
    private bool isDead = false;

    [Header("Refs")]
    private EnemyRagdoll ragdoll;
    private EnemyAI ai;
    private NavMeshAgent agent;
    private Animator animator;
    private Collider mainCollider;

    private Vector3 cachedPosition;
    private Quaternion cachedRotation;

    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<EnemyRagdoll>();
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} murió ☠️");

        // 1️⃣ Desactivar AI y NavMesh antes de todo
        if (ai != null) ai.enabled = false;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;
        }

        // 2️⃣ Desactivar el collider principal ANTES de activar el ragdoll
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }

        // 3️⃣ Guardar posición y elevar el transform unos cm (evita clip inicial)
        cachedPosition = transform.position + Vector3.up * 0.05f;
        cachedRotation = transform.rotation;
        transform.position = cachedPosition;
        transform.rotation = cachedRotation;

        // 4️⃣ Desactivar animator
        if (animator != null)
        {
            animator.enabled = false;
        }

        // 5️⃣ Activar ragdoll sin fuerzas
        if (ragdoll != null)
        {
            ragdoll.SetRagdollActive(true);
        }

        // 6️⃣ Avisar al GameController que un enemigo murió
        if (GameController.Instance != null)
        {
            GameController.Instance.EnemyDied();
        }

        // 7️⃣ (Opcional) destruir enemigo después de un tiempo
        Destroy(gameObject, 10f);
    }


}


//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyHealth : MonoBehaviour
//{
//    [Header("Vida")]
//    public int maxHealth = 50;
//    private int currentHealth;
//    private bool isDead = false;

//    [Header("Referencias")]
//    private EnemyRagdoll ragdoll;
//    private EnemyAI ai;
//    private NavMeshAgent agent;
//    private Collider mainCollider;

//    void Start()
//    {
//        currentHealth = maxHealth;

//        ragdoll = GetComponent<EnemyRagdoll>();
//        ai = GetComponent<EnemyAI>();
//        agent = GetComponent<NavMeshAgent>();
//        mainCollider = GetComponent<Collider>();
//    }

//    public void TakeDamage(int amount)
//    {
//        if (isDead) return;

//        currentHealth -= amount;
//        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    private void Die()
//    {
//        if (isDead) return;
//        isDead = true;

//        Debug.Log($"{gameObject.name} ha muerto ☠️");

//        // 🦴 1. Activar ragdoll primero
//        if (ragdoll != null)
//        {
//            ragdoll.SetRagdollActive(true);
//        }

//        // 📐 2. Asegurar que el transform root no tenga fuerza residual
//        if (agent != null && agent.enabled)
//        {
//            agent.velocity = Vector3.zero;
//            agent.ResetPath();
//        }

//        // 🧠 3. Desactivar control IA y colisión padre
//        if (ai != null) ai.DisableAI();
//        if (agent != null) agent.enabled = false;
//        if (mainCollider != null) mainCollider.enabled = false;

//        // 🕒 4. Destruir después de un tiempo opcional
//        Destroy(gameObject, 10f);
//    }

//    public bool IsDead()
//    {
//        return isDead;
//    }
//}
