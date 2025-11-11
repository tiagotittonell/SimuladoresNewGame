using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Configuración del Ragdoll")]
    public Animator animator;             // referencia al Animator
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Rigidbody mainRigidbody;
    private Collider mainCollider;

    void Awake()
    {
        // buscamos todos los rigidbodies secundarios del cuerpo
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // cuerpo principal (enemigo “vivo”)
        mainRigidbody = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        SetRagdollState(false); // ✅ desactivado por defecto
    }

    public void SetRagdollState(bool enabled)
    {
        // animator ON solo cuando el ragdoll está desactivado
        if (animator != null)
            animator.enabled = !enabled;

        // desactiva o activa todas las físicas del ragdoll
        foreach (var body in ragdollBodies)
        {
            if (body == mainRigidbody) continue;
            body.isKinematic = !enabled;
            body.useGravity = enabled;
        }

        foreach (var col in ragdollColliders)
        {
            if (col == mainCollider) continue;
            col.enabled = enabled;
        }

        // el rigidbody principal se mantiene kinematic mientras vive
        if (mainRigidbody != null)
            mainRigidbody.isKinematic = enabled;
    }
}
