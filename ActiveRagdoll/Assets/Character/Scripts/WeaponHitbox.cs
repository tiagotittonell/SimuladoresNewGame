
//using UnityEngine;

//[RequireComponent(typeof(Collider))]
//public class WeaponHitbox : MonoBehaviour
//{
//    public int damage = 50;
//    private Collider hitbox;

//    void Awake()
//    {
//        hitbox = GetComponent<Collider>();
//        hitbox.isTrigger = true;      // asegurar que sea trigger
//        hitbox.enabled = false;       // desactivado al inicio
//    }

//    public void EnableHitbox()
//    {
//        hitbox.enabled = true;
//    }

//    public void DisableHitbox()
//    {
//        hitbox.enabled = false;
//    }



//    void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("Colisión detectada con: " + other.name);


//        if (other.CompareTag("Enemy"))
//        {
//            EnemyHealth enemy = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();

//            if (enemy != null)
//            {
//                enemy.TakeDamage(damage);
//                Debug.Log("Daño aplicado a: " + other.name + " por " + damage);
//            }
//            else
//            {
//                Debug.LogWarning("El objeto con tag Enemy no tiene EnemyHealth");
//            }
//        }
//    }
//}
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponHitbox : MonoBehaviour
{
    public int damage = 50;
    private Collider hitbox;
    private Animator animator; // ← Necesario para saber si fue ataque pesado

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;

        // Obtener Animator del Player
        animator = GetComponentInParent<Animator>();
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

        // --- 1) Golpe a enemigos normales ---
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>()
                                ?? other.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Daño aplicado a enemigo: " + damage);
            }
        }

        // --- 2) Golpe al Dummy del Tutorial ---
        if (other.TryGetComponent(out DummyTarget dummy))
        {
            bool isHeavy = false;
            Animator anim = GetComponentInParent<Animator>();

            if (anim != null)
                isHeavy = anim.GetInteger("AttackIndex") == 3;

            dummy.GetHit(isHeavy);
            return;
        }

    }
}




