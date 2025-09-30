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
        if (IsDead()) return; // ignorar da�o si ya est� muerto

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibi� {amount} de da�o. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " muri�!");

        if (ragdoll != null)
        {
            ragdoll.SetRagdoll(true); // activar ragdoll
        }

        // Opcional: desactivar este script para que no reciba m�s da�o
        this.enabled = false;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
