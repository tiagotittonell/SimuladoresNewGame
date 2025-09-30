
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponHitbox : MonoBehaviour
{
    public int damage = 50;
    private Collider hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.isTrigger = true;      // asegurar que sea trigger
        hitbox.enabled = false;       // desactivado al inicio
    }

    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }

   

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisión detectada con: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Daño aplicado a: " + other.name + " por " + damage);
            }
            else
            {
                Debug.LogWarning("El objeto con tag Enemy no tiene EnemyHealth");
            }
        }
    }
}



