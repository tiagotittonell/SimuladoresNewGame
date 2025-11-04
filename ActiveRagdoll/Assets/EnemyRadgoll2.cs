using UnityEngine;
using System.Linq;

/// Controla el ragdoll: OFF al iniciar, ON sólo si lo pedís.
public class EnemyRagdoll2 : MonoBehaviour
{
    [Header("Root del Ragdoll (ej: Hips / Pelvis)")]
    [Tooltip("Si lo dejás vacío intento detectarlo (primer Rigidbody hijo con Joint o 'Hips').")]
    public Transform ragdollRoot;

    [Header("Opcional")]
    [Tooltip("Collider principal del personaje (capsule del root). Lo dejamos activo cuando el ragdoll está OFF).")]
    public Collider mainCollider;

    [Tooltip("Forzá desactivar ragdoll en Start por si otro script lo ensucia.")]
    public bool forceOffOnStart = true;

    Animator animator;
    Rigidbody[] ragRBs = System.Array.Empty<Rigidbody>();
    Collider[] ragCols = System.Array.Empty<Collider>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (!mainCollider) mainCollider = GetComponents<Collider>().FirstOrDefault(); // capsule del root si existe

        // Autodetectar root si no fue asignado
        if (ragdollRoot == null)
        {
            var anim = GetComponent<Animator>();
            if (anim && anim.isHuman && anim.avatar && anim.avatar.isValid)
            {
                // intentamos hips
                var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
                if (hips) ragdollRoot = hips;
            }

            if (ragdollRoot == null)
            {
                // fallback: 1er rigidbody hijo que NO sea el root y tenga Joint o esté en la cadena de huesos
                ragdollRoot = GetComponentsInChildren<Rigidbody>(true)
                               .Select(rb => rb.transform)
                               .FirstOrDefault(t => t != transform);
            }
        }

        // Capturar todos los RB/Colliders del ragdoll (excluyendo el root si tuviera uno)
        if (ragdollRoot != null)
        {
            ragRBs = ragdollRoot.GetComponentsInChildren<Rigidbody>(true)
                                 .Where(rb => rb.transform != transform).ToArray();

            ragCols = ragdollRoot.GetComponentsInChildren<Collider>(true)
                                 .Where(c => c.transform != transform).ToArray();
        }

        // Apagar SIEMPRE en Awake
        SetRagdollActive(false);
    }

    void Start()
    {
        if (forceOffOnStart)
            SetRagdollActive(false); // por si otro script tocó algo durante el awake chain
    }

    /// Enciende/apaga el ragdoll.
    public void SetRagdollActive(bool active)
    {
        // Animator ON si ragdoll OFF; Animator OFF si ragdoll ON
        if (animator) animator.enabled = !active;

        // Colliders del ragdoll
        foreach (var c in ragCols)
            if (c) c.enabled = active;

        // Rigidbodies del ragdoll
        foreach (var rb in ragRBs)
        {
            if (!rb) continue;
            rb.isKinematic = !active;
            rb.useGravity = active;

            if (!active)
            {
                // Estado “congelado” limpio
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }

        // Mantener el collider principal (capsule) activo cuando el ragdoll está OFF
        if (mainCollider) mainCollider.enabled = !active;
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG: Log status")]
    void DebugLog()
    {
        Debug.Log($"[EnemyRagdoll] Animator:{(animator && animator.enabled)} " +
                  $"RBs:{ragRBs.Length} Cols:{ragCols.Length} Root:{(ragdollRoot ? ragdollRoot.name : "<null>")}", this);
    }
#endif
}
