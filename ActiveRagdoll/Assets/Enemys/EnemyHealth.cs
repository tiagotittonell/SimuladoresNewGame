
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 50;
    protected int currentHealth;
    protected bool isDead = false; 

    [Header("Refs")]
    protected EnemyRagdoll ragdoll;    
    protected EnemyAI ai;               
    protected NavMeshAgent agent;       
    protected Animator animator;       
    protected Collider mainCollider;    

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


        if (ai != null) ai.enabled = false;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;
        }

        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }

        cachedPosition = transform.position + Vector3.up * 0.05f;
        cachedRotation = transform.rotation;
        transform.position = cachedPosition;
        transform.rotation = cachedRotation;

        if (animator != null)
        {
            animator.enabled = false;
        }

        if (ragdoll != null)
        {
            ragdoll.SetRagdollActive(true);
        }

        var drop = GetComponent<EnemyDrops>();
        if (drop != null)
            drop.DropCoins();

        if (GameController.Instance != null)
        {
            GameController.Instance.EnemyDied();
        }

        Destroy(gameObject, 10f);
    }


}

