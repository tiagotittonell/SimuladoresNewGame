using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDamage : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 50;

    [Header("Ignorar colisiones del propio enemigo")]
    [SerializeField] private Transform ownerRoot; // el root del enemigo (así cubrimos ragdolls también)

    private Collider weaponCollider;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;

        // Buscar el root si no está asignado
        if (ownerRoot == null)
            ownerRoot = transform.root;

        // Ignorar TODAS las colisiones con colliders del propio enemigo (ragdoll incluido)
        var ownerColliders = ownerRoot.GetComponentsInChildren<Collider>(true);
        foreach (var col in ownerColliders)
        {
            if (col != weaponCollider)
                Physics.IgnoreCollision(weaponCollider, col, true);
        }
    }

    public void EnableHitbox() => weaponCollider.enabled = true;
    public void DisableHitbox() => weaponCollider.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ignorar colisiones con cualquier parte del propio enemigo
        if (other.transform.root == ownerRoot) return;

        // Dañar solo al jugador
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"🔥 Daño {damage} aplicado a {other.name}");
            }
        }
    }
}
