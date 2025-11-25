using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ArcherAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Animator animator;
    public EnemyDamage meleeHitbox;
    public GameObject arrowPrefab;
    public Transform shootPoint;

    private Rigidbody rb;

    [Header("Movimiento")]
    public float idealRange = 12f;
    public float tooCloseRange = 7f;
    public float moveSpeed = 2f;
    public float strafeSpeed = 1.5f;

    [Header("Ataques")]
    public float shootRange = 25f;
    public float shootCooldown = 2f;
    public float meleeCooldown = 1.5f;

    private float lastShootTime = -999f;
    private float lastMeleeTime = -999f;

    private bool isAttacking = false;
    private bool arrowReleased = false;    // 🔥 evita doble disparo

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        HandleAttacks(dist);

        if (isAttacking)
        {
            ResetMovementBools();
            return;
        }

        FacePlayer();
        HandleMovement(dist);
    }

    // ---------------- ATAQUES ----------------
    private void HandleAttacks(float dist)
    {
        // MELEE
        if (dist < tooCloseRange && Time.time - lastMeleeTime >= meleeCooldown)
        {
            StartCoroutine(MeleeRoutine());
            return;
        }

        // DISPARO
        if (dist <= shootRange && Time.time - lastShootTime >= shootCooldown)
        {
            StartCoroutine(ShootRoutine());
            return;
        }
    }

    private IEnumerator ShootRoutine()
    {
        isAttacking = true;
        arrowReleased = false;     // 🔥 importante
        ResetMovementBools();

        animator.SetTrigger("Shoot");

        // Esperar a que la animación termine ANTES del cooldown
        yield return new WaitForSeconds(0.9f);

        lastShootTime = Time.time;
        isAttacking = false;
    }

    // 📌 LLAMADO SOLO POR EL ANIMATION EVENT
    public void FireArrow()
    {
        if (arrowReleased) return;    // ❗ evita duplicado
        arrowReleased = true;

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

        Vector3 dir = (player.position + Vector3.up * 1.2f) - shootPoint.position;
        dir.Normalize();

        arrow.transform.rotation = Quaternion.LookRotation(dir);

        Rigidbody rbArrow = arrow.GetComponent<Rigidbody>();
        if (rbArrow != null)
            rbArrow.velocity = dir * 35f;
    }

    private IEnumerator MeleeRoutine()
    {
        isAttacking = true;
        ResetMovementBools();

        animator.SetTrigger("Melee");

        yield return new WaitForSeconds(0.2f);
        meleeHitbox.EnableHitbox();

        yield return new WaitForSeconds(0.2f);
        meleeHitbox.DisableHitbox();

        lastMeleeTime = Time.time;

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    // ---------------- MOVIMIENTO ----------------
    private void HandleMovement(float dist)
    {
        ResetMovementBools();

        // 🟦 Lejos → acercarse
        if (dist > idealRange + 2f)
        {
            MoveForward();
            return;
        }

        // 🟧 Si está en distancia de disparo pero demasiado cerca del ideal → retroceder un poco
        if (dist < idealRange && dist > tooCloseRange)
        {
            MoveBack();
            return;
        }

        // 🟥 Si está MUY cerca → retroceder rápido
        if (dist <= tooCloseRange)
        {
            MoveBack();
            return;
        }

        // 🟨 Distancia óptima → movimiento lateral (difícil apuntarle ✔)
        float r = Random.Range(0f, 1f);

        if (r < 0.5f)
            MoveLeft();
        else
            MoveRight();
    }


    private void MoveForward()
    {
        animator.SetBool("IsMoving", true);
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveBack()
    {
        animator.SetBool("IsMovingBack", true);
        rb.MovePosition(rb.position + -transform.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveLeft()
    {
        animator.SetBool("IsMovingLeft", true);
        rb.MovePosition(rb.position + -transform.right * strafeSpeed * Time.deltaTime);
    }

    private void MoveRight()
    {
        animator.SetBool("IsMovingRight", true);
        rb.MovePosition(rb.position + transform.right * strafeSpeed * Time.deltaTime);
    }

    private void ResetMovementBools()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsMovingRight", false);
        animator.SetBool("IsMovingBack", false);
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 7f
        );
    }
}
