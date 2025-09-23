using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    private Animator animator;
    private Rigidbody mainRb;
    private Collider mainCollider;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    void Awake()
    {
        animator = GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        // Buscar todos los rigidbodies/colliders en los huesos
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Inicialmente desactivar ragdoll
        SetRagdoll(false);
    }

    public void SetRagdoll(bool active)
    {
        // Activa/desactiva Animator y el rigidbody principal
        if (animator) animator.enabled = !active;
        if (mainRb) mainRb.isKinematic = active;
        if (mainCollider) mainCollider.enabled = !active;

        // Configurar huesos
        foreach (var rb in ragdollBodies)
        {
            if (rb != mainRb) rb.isKinematic = !active;
        }
        foreach (var col in ragdollColliders)
        {
            if (col != mainCollider) col.enabled = active;
        }
    }
}
