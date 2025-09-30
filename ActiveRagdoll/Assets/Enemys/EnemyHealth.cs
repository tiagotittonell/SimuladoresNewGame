using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    private EnemyRagdoll ragdoll;

    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<EnemyRagdoll>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead()) return; // ignorar daño si ya está muerto

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " murió!");

        if (ragdoll != null)
        {
            ragdoll.SetRagdoll(true); // activar ragdoll
        }

        // Opcional: desactivar este script para que no reciba más daño
        this.enabled = false;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
