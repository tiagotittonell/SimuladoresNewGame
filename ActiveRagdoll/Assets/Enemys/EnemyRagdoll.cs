
//using UnityEngine;

//public class EnemyRagdoll : MonoBehaviour
//{
//    [Header("Root del Ragdoll (normalmente el cuerpo)")]
//    public Transform ragdollRoot;

//    private Collider[] ragdollColliders;
//    private Rigidbody[] ragdollRigidbodies;
//    private Animator animator;

//    void Awake()
//    {
//        animator = GetComponent<Animator>();

//        // Buscar colliders y rigidbodies en el ragdoll
//        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>();
//        ragdollRigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();

//        // Desactivarlos al inicio (personaje vivo)
//        SetRagdollActive(false);
//    }

//    public void SetRagdollActive(bool active)
//    {
//        foreach (var col in ragdollColliders)
//            col.enabled = active;

//        foreach (var rb in ragdollRigidbodies)
//        {
//            rb.isKinematic = !active;
//            rb.useGravity = active;
//        }

//        if (animator != null)
//            animator.enabled = !active;
//    }
//}
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    [Header("Root del Ragdoll")]
    public Transform ragdollRoot;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    void Awake()
    {
        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>();
        ragdollRigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        SetRagdollActive(false);
    }

    public void SetRagdollActive(bool active)
    {
        foreach (var col in ragdollColliders)
            col.enabled = active;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
            rb.useGravity = active;

            if (active)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = rb.transform.position; // evita offset
                rb.rotation = rb.transform.rotation;
                rb.Sleep();
            }
        }
    }
}


