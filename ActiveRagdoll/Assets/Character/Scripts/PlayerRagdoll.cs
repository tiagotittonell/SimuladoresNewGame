using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private Animator animator;
    private Rigidbody mainRb;
    private Collider mainCollider;

    void Awake()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        SetRagdoll(false); // arrancar desactivado
    }

    public void SetRagdoll(bool active)
    {
        // Animator apagado cuando ragdoll activo
        if (animator != null)
            animator.enabled = !active;

        foreach (var rb in ragdollBodies)
        {
            if (rb != mainRb)
            {
                rb.isKinematic = !active;
                rb.useGravity = active;
            }
        }

        foreach (var col in ragdollColliders)
        {
            if (col != mainCollider)
                col.enabled = active;
        }

        // el collider principal solo cuando está vivo
        if (mainCollider != null) mainCollider.enabled = !active;
        if (mainRb != null) mainRb.isKinematic = active;
    }
}
