using UnityEngine;
using UnityEngine.AI;

public class MageHealth : EnemyHealth
{
    private EnemyRagdoll3 ragdoll3;

    protected override void Start()
    {
        base.Start();

        ragdoll3 = GetComponent<EnemyRagdoll3>();
    }

    //protected override void Die()
    //{
    //    if (isDead) return;
    //    isDead = true;


    //    if (ai != null) ai.enabled = false;
    //    if (agent != null && agent.enabled)
    //    {
    //        agent.ResetPath();
    //        agent.velocity = Vector3.zero;
    //        agent.updatePosition = false;
    //        agent.updateRotation = false;
    //        agent.enabled = false;
    //    }

    //    if (mainCollider != null)
    //        mainCollider.enabled = false;

    //    cachedPosition = transform.position + Vector3.up * 0.05f;
    //    cachedRotation = transform.rotation;
    //    transform.position = cachedPosition;
    //    transform.rotation = cachedRotation;

    //    if (animator != null)
    //        animator.enabled = false;

    //    if (ragdoll3 != null)
    //        ragdoll3.SetRagdollActive(true);
    //    else if (ragdoll != null)
    //        ragdoll.SetRagdollActive(true);

    //    var drop = GetComponent<EnemyDrops>();
    //    if (drop != null)
    //        drop.DropCoins();


    //    if (GameController.Instance != null)
    //        GameController.Instance.EnemyDied();

    //    Destroy(gameObject, 5f);

    //}
    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notificar al AI
        MageAI mageAI = GetComponent<MageAI>();
        if (mageAI != null)
            mageAI.OnDeath();

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
            mainCollider.enabled = false;

        cachedPosition = transform.position + Vector3.up * 0.05f;
        cachedRotation = transform.rotation;
        transform.position = cachedPosition;
        transform.rotation = cachedRotation;

        if (animator != null)
            animator.enabled = false;

        if (ragdoll3 != null)
            ragdoll3.SetRagdollActive(true);
        else if (ragdoll != null)
            ragdoll.SetRagdollActive(true);

        var drop = GetComponent<EnemyDrops>();
        if (drop != null)
            drop.DropCoins();

        if (GameController.Instance != null)
            GameController.Instance.EnemyDied();

        Destroy(gameObject, 5f);
    }
}
