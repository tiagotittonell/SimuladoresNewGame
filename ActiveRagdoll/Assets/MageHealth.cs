using UnityEngine;
using UnityEngine.AI;

public class MageHealth : EnemyHealth
{
    private EnemyRagdoll3 ragdoll3;

    protected override void Start()
    {
        base.Start();

        // Intentamos buscar el tipo de ragdoll correcto
        ragdoll3 = GetComponent<EnemyRagdoll3>();

        if (ragdoll3 != null)
            Debug.Log($"{gameObject.name}: MageHealth usando EnemyRagdoll3");
        else
            Debug.LogWarning($"{gameObject.name}: No se encontró EnemyRagdoll3, revisá el prefab");
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} (mago) murió ☠️");

        // Desactivar AI y NavMesh (igual que el base)
        if (ai != null) ai.enabled = false;
        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.enabled = false;
        }

        // Collider principal
        if (mainCollider != null)
            mainCollider.enabled = false;

        // Guardar posición y rotación
        cachedPosition = transform.position + Vector3.up * 0.05f;
        cachedRotation = transform.rotation;
        transform.position = cachedPosition;
        transform.rotation = cachedRotation;

        // Desactivar animator
        if (animator != null)
            animator.enabled = false;

        // ✅ Activar ragdoll3 en lugar del normal
        if (ragdoll3 != null)
            ragdoll3.SetRagdollActive(true);
        else if (ragdoll != null)
            ragdoll.SetRagdollActive(true);

        // Avisar al GameController
        if (GameController.Instance != null)
            GameController.Instance.EnemyDied();

        //// Destruir después de unos segundos
        //Destroy(gameObject, 10f);
    }
}
