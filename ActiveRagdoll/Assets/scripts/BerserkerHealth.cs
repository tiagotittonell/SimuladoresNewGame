using UnityEngine;

public class BerserkerHealth : EnemyHealth
{
    private RagdollController ragdoll;
    private BerserkerAI ai;

    protected override void Start()
    {
        base.Start();
        ragdoll = GetComponent<RagdollController>();
        ai = GetComponent<BerserkerAI>();

        if (ragdoll != null)
            ragdoll.SetRagdollState(false);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        if (ai != null)
            ai.enabled = false;

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (animator != null)
            animator.enabled = false;

        if (ragdoll != null)
            ragdoll.SetRagdollState(true);

        var drop = GetComponent<EnemyDrops>();
        if (drop != null)
            drop.DropCoins();

        if (GameController.Instance != null)
            GameController.Instance.EnemyDied();

        Destroy(gameObject, 12f);
    }

}
