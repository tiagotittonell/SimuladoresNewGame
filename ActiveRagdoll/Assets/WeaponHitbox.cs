using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 10;
    private Collider hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false; // apagado por defecto
    }

    public void EnableHitbox() => hitbox.enabled = true;
    public void DisableHitbox() => hitbox.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }
}
