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

        // Desactiva ragdoll al inicio
        if (ragdoll != null)
            ragdoll.SetRagdollState(false);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{name} (Berserker) murió 💀");

        // Desactivar IA
        if (ai != null)
            ai.enabled = false;

        // Desactivar collider principal antes de activar el ragdoll
        if (mainCollider != null)
            mainCollider.enabled = false;

        // Desactivar animaciones
        if (animator != null)
            animator.enabled = false;

        // Activar el ragdoll
        if (ragdoll != null)
            ragdoll.SetRagdollState(true);

        // (opcional) destruir el cuerpo después de unos segundos
        Destroy(gameObject, 12f);
    }
}
