using UnityEngine;
using System.Linq;

/// Controla el ragdoll: OFF al iniciar, ON sólo si lo pedís.
public class EnemyRagdoll3 : MonoBehaviour
{
    [Header("Root del Ragdoll (ej: Hips / Pelvis)")]
    public Transform ragdollRoot;

    [Header("Opcional")]
    public Collider mainCollider;
    public bool forceOffOnStart = true;

    private Animator animator;
    private Rigidbody[] ragRBs = System.Array.Empty<Rigidbody>();
    private Collider[] ragCols = System.Array.Empty<Collider>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (!mainCollider)
            mainCollider = GetComponent<Collider>();

        // 🔍 Detectar el root del ragdoll automáticamente
        if (ragdollRoot == null)
        {
            Animator anim = GetComponent<Animator>();
            if (anim && anim.isHuman)
            {
                ragdollRoot = anim.GetBoneTransform(HumanBodyBones.Hips);
            }
            if (ragdollRoot == null)
            {
                ragdollRoot = GetComponentsInChildren<Rigidbody>(true)
                    .Select(rb => rb.transform)
                    .FirstOrDefault(t => t != transform);
            }
        }

        // 🔍 Capturar TODOS los rigidbodies y colliders
        RefreshRagdollParts();

        // 🔒 Desactivar ragdoll siempre al inicio
        SetRagdollActive(false);
    }

    void Start()
    {
        if (forceOffOnStart)
            SetRagdollActive(false);
    }

    private void RefreshRagdollParts()
    {
        if (ragdollRoot != null)
        {
            ragRBs = ragdollRoot.GetComponentsInChildren<Rigidbody>(true)
                .Where(rb => rb.transform != transform)
                .ToArray();

            ragCols = ragdollRoot.GetComponentsInChildren<Collider>(true)
                .Where(c => c.transform != transform)
                .ToArray();
        }
        else
        {
            Debug.LogWarning($"{name}: No se encontró ragdollRoot");
        }
    }

    public void SetRagdollActive(bool active)
    {
        // ⚙️ Reasegurar referencias por si se instanció tarde
        if (ragRBs == null || ragRBs.Length == 0)
            RefreshRagdollParts();

        if (animator) animator.enabled = !active;

        foreach (var c in ragCols)
            if (c) c.enabled = active;

        foreach (var rb in ragRBs)
        {
            if (!rb) continue;

            rb.isKinematic = !active;
            rb.useGravity = active;

            if (!active)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }

        if (mainCollider)
            mainCollider.enabled = !active;

        Debug.Log($"{name} → Ragdoll {(active ? "ACTIVADO" : "DESACTIVADO")} con {ragRBs.Length} huesos detectados");
    }
}
