using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMovementRB : MonoBehaviour
{
    [Header("Componentes")]
    public Animator animator;
    private Rigidbody rb;

    [Header("Movimiento")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 10f;

    [Header("Salto")]
    public float jumpForce = 7f;
    public bool isGrounded;

    [Header("Ataques")]
    public bool isAttacking = false;
    private int lastAttack = 0;

    [Header("Combos")]
    public int comboStep = 0;          // 0 = ninguno, 1 = ataque1, 2 = ataque2...
    public float comboResetTime = 1f;  // tiempo para reiniciar combo si no seguiste
    private float lastAttackTime = 0f;

    private bool queuedAttack = false; // buffer de input

    public WeaponHitbox weapon;
    private Vector3 moveInput;
    private bool isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // ----- Input movimiento -----
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S
        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift);

        // ----- Animaciones locomoción -----
        animator.SetFloat("VelX", horizontal);
        animator.SetFloat("VelY", isRunning ? vertical * 2f : vertical);

        // ----- Suelo / salto -----
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        animator.SetBool("isJumping", !isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        // ----- Ataques -----
        if (Input.GetMouseButtonDown(0) && !isAttacking)      // click izq
            LightAttack();
        else if (Input.GetMouseButtonDown(1) && !isAttacking) // click der
            HeavyAttack();

        // ----- AUTOCIERRE DE ATAQUE (sin Animation Events) -----
        if (isAttacking)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            // Asegurate que estos nombres coincidan con los estados del Animator
            bool inAttackState = st.IsName("Sword And Shield Kick") || st.IsName("Attack2") || st.IsName("AttackPower");

            // normalizedTime >= 1 cuando el clip terminó (y no estamos en blend/transition)
            if (inAttackState && st.normalizedTime >= 0.8f && !animator.IsInTransition(0))
                EndAttack();
        }

        // BOTÓN DEBUG opcional:
        if (Input.GetKeyDown(KeyCode.R)) EndAttack();
    }

    void FixedUpdate()
    {
        if (isAttacking) return; // 🔒 Bloquea movimiento mientras dura un ataque

        // Personaje SIEMPRE mira hacia la cámara
        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

        // Movimiento relativo a la cámara
        Vector3 camRight = Camera.main.transform.right; camRight.y = 0;
        Vector3 moveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

        if (moveDir.magnitude > 0.1f && isGrounded)
        {
            float speed = isRunning ? runSpeed : walkSpeed;
            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
        }


    }

    void LightAttack()
    {
        // Si ya estoy atacando → marco que quiero encadenar
        if (isAttacking)
        {
            queuedAttack = true;
            return;
        }

        // Nuevo ataque
        isAttacking = true;
        comboStep++;

        // Alternamos entre 1 y 2 (podés extender más pasos si querés)
        if (comboStep > 2) comboStep = 1;

        animator.SetInteger("AttackIndex", comboStep);
        animator.SetBool("isAttacking", true);

        lastAttackTime = Time.time;

        // ✅ activar hitbox
        if (weapon != null) weapon.EnableHitbox();
    }

    void HeavyAttack()
    {
        // Ataque fuerte siempre rompe combo
        isAttacking = true;
        comboStep = 0; // reset combo chain

        animator.SetInteger("AttackIndex", 3);
        animator.SetBool("isAttacking", true);

        // ✅ activar hitbox
        if (weapon != null) weapon.EnableHitbox();
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        animator.SetInteger("AttackIndex", 0);

        // ✅ desactivar hitbox
        if (weapon != null) weapon.DisableHitbox();

        if (queuedAttack)
        {
            queuedAttack = false;
            LightAttack(); // encadena automáticamente
        }
        else
        {
            // Si pasó demasiado tiempo, reseteo combo
            if (Time.time - lastAttackTime > comboResetTime)
                comboStep = 0;
        }
    }



    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
