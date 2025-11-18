using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 50;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false; 
    }

    public void EnableHitbox() => col.enabled = true;
    public void DisableHitbox() => col.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
