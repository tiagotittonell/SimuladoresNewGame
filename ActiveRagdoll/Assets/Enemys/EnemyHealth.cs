
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 50;
    protected int currentHealth;
    protected bool isDead = false; // 🔧 Antes era private

    [Header("Refs")]
    protected EnemyRagdoll ragdoll;     // 🔧 protected
    protected EnemyAI ai;               // 🔧 protected
    protected NavMeshAgent agent;       // 🔧 protected
    protected Animator animator;        // 🔧 protected
    protected Collider mainCollider;    // 🔧 protected

    protected Vector3 cachedPosition;
    protected Quaternion cachedRotation;


    protected virtual void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<EnemyRagdoll>();
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");
        var feedback = GetComponent<EnemyHitFeedback>();
        if (feedback != null)
            feedback.PlayHitFeedback();

        if (currentHealth <= 0)
            Die();
    }


    protected virtual void Die()
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

