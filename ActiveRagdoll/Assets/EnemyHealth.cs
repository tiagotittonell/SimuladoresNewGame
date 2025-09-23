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
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió!");

        if (ragdoll != null)
        {
            ragdoll.SetRagdoll(true); // activar ragdoll
        }

        // Opcional: desactivar este script para que no reciba más daño
        this.enabled = false;
    }
}
