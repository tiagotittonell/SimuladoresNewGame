
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del jugador")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = false;

    [Header("Referencias del Jugador")]
    private PlayerRagdoll ragdoll;
    private GameController gameController;

    private ThirdPersonCamera cam;

    [Header("UI de Vida")]
    public Image healthBarFill; // ← arrastrá acá la imagen roja del Canvas

    void Start()
    {
        currentHealth = maxHealth;
        cam = FindObjectOfType<ThirdPersonCamera>();
        ragdoll = GetComponent<PlayerRagdoll>();
        gameController = FindObjectOfType<GameController>();
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvulnerable)
            return;

        currentHealth -= amount;
        if (cam != null)
            cam.ShakeCamera(amount / 20f);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Jugador recibió {amount} de daño. Vida: {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Jugador murió!");

        if (ragdoll != null)
            ragdoll.SetRagdoll(true);

        if (gameController != null)
            gameController.PlayerDied();
    }

    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
        Debug.Log($"Invulnerable: {isInvulnerable}");
    }

    // 🧪 Test: quitar vida con H
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10);

        if (Input.GetKeyDown(KeyCode.C))
            Die();
    }
}
