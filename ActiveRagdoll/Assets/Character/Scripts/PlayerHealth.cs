//using UnityEngine;

//public class PlayerHealth : MonoBehaviour
//{
//    [Header("Vida del jugador")]
//    public int maxHealth = 100;
//    private int currentHealth;

//    [Header("Referencias")]
//    private PlayerRagdoll ragdoll;
//    private bool isDead = false;

//    void Start()
//    {
//        currentHealth = maxHealth;
//        ragdoll = GetComponent<PlayerRagdoll>();
//    }

//    void Update()
//    {
//         🔹 Tecla de debug: mata al jugador instantáneamente
//        if (Input.GetKeyDown(KeyCode.C))
//        {
//            Debug.Log("DEBUG: Forzando muerte del jugador (tecla C).");
//            TakeDamage(currentHealth); // aplica daño igual a la vida restante
//        }
//    }

//    public void TakeDamage(int amount)
//    {
//        if (isDead) return;

//        currentHealth -= amount;
//        Debug.Log($"Jugador recibió {amount} de daño. Vida: {currentHealth}");

//        if (currentHealth <= 0)
//            Die();
//    }

//    void Die()
//    {
//        if (isDead) return;

//        isDead = true;
//        Debug.Log("Jugador murió!");

//        if (ragdoll != null)
//        {
//            ragdoll.SetRagdoll(true); // activar físicas
//        }

//        Avisar al GameManager
//        GameController.Instance.PlayerDied();
//    }

//    void PantallaMuerte()
//    {

//    }
//}

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = false;

    private PlayerRagdoll ragdoll; // si tu personaje usa ragdoll
    private GameController gameController; // para activar pantalla de muerte

    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<PlayerRagdoll>();
        gameController = FindObjectOfType<GameController>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvulnerable)
            return;

        currentHealth -= amount;
        Debug.Log($"Jugador recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Jugador murió!");

        if (ragdoll != null)
            ragdoll.SetRagdoll(true);

        if (gameController != null)
            gameController.PlayerDied();
    }

    // 💫 Este método lo usa el dodge para activar/desactivar invulnerabilidad
    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
        Debug.Log($"Invulnerable: {isInvulnerable}");
    }

    // Método para testear muerte (tecla C)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Die();
    }
}
