//using UnityEngine;

//[RequireComponent(typeof(Rigidbody), typeof(Collider))]
//public class PlayerMovementRB : MonoBehaviour
//{
//    [Header("Componentes")]
//    public Animator animator;
//    private Rigidbody rb;

//    [Header("Movimiento")]
//    public float walkSpeed = 2f;
//    public float runSpeed = 4f;
//    public float rotationSpeed = 10f;

//    [Header("Salto")]
//    public float jumpForce = 7f;
//    public bool isGrounded;

//    [Header("Ataques")]
//    public bool isAttacking = false;
//    private int lastAttack = 0;

//    [Header("Combos")]
//    public int comboStep = 0;          // 0 = ninguno, 1 = ataque1, 2 = ataque2...
//    public float comboResetTime = 1f;  // tiempo para reiniciar combo si no seguiste
//    private float lastAttackTime = 0f;

//    private bool queuedAttack = false; // buffer de input

//    public WeaponHitbox weapon;
//    private Vector3 moveInput;
//    private bool isRunning;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//    }

//    void Update()
//    {
//        // ----- Input movimiento -----
//        float horizontal = Input.GetAxis("Horizontal"); // A/D
//        float vertical = Input.GetAxis("Vertical");     // W/S
//        moveInput = new Vector3(horizontal, 0, vertical).normalized;

//        isRunning = Input.GetKey(KeyCode.LeftShift);

//        // ----- Animaciones locomoción -----
//        animator.SetFloat("VelX", horizontal);
//        animator.SetFloat("VelY", isRunning ? vertical * 2f : vertical);

//        // ----- Suelo / salto -----
//        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
//        animator.SetBool("isJumping", !isGrounded);
//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//            Jump();

//        // ----- Ataques -----
//        if (Input.GetMouseButtonDown(0) && !isAttacking)      // click izq
//            LightAttack();
//        else if (Input.GetMouseButtonDown(1) && !isAttacking) // click der
//            HeavyAttack();

//        // ----- AUTOCIERRE DE ATAQUE (sin Animation Events) -----
//        if (isAttacking)
//        {
//            var st = animator.GetCurrentAnimatorStateInfo(0);
//            // Asegurate que estos nombres coincidan con los estados del Animator
//            bool inAttackState = st.IsName("Sword And Shield Kick") || st.IsName("Attack2") || st.IsName("AttackPower");

//            // normalizedTime >= 1 cuando el clip terminó (y no estamos en blend/transition)
//            if (inAttackState && st.normalizedTime >= 0.8f && !animator.IsInTransition(0))
//                EndAttack();
//        }

//        // BOTÓN DEBUG opcional:
//        if (Input.GetKeyDown(KeyCode.R)) EndAttack();
//    }

//    void FixedUpdate()
//    {
//        if (isAttacking) return; // 🔒 Bloquea movimiento mientras dura un ataque

//        // Personaje SIEMPRE mira hacia la cámara
//        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0;
//        Quaternion targetRotation = Quaternion.LookRotation(camForward);
//        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

//        // Movimiento relativo a la cámara
//        Vector3 camRight = Camera.main.transform.right; camRight.y = 0;
//        Vector3 moveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

//        if (moveDir.magnitude > 0.1f && isGrounded)
//        {
//            float speed = isRunning ? runSpeed : walkSpeed;
//            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
//        }


//    }

//    void LightAttack()
//    {
//        // Si ya estoy atacando → marco que quiero encadenar
//        if (isAttacking)
//        {
//            queuedAttack = true;
//            return;
//        }

//        // Nuevo ataque
//        isAttacking = true;
//        comboStep++;

//        // Alternamos entre 1 y 2 (podés extender más pasos si querés)
//        if (comboStep > 2) comboStep = 1;

//        animator.SetInteger("AttackIndex", comboStep);
//        animator.SetBool("isAttacking", true);

//        lastAttackTime = Time.time;

//        // ✅ activar hitbox
//        if (weapon != null) weapon.EnableHitbox();
//    }

//    void HeavyAttack()
//    {
//        // Ataque fuerte siempre rompe combo
//        isAttacking = true;
//        comboStep = 0; // reset combo chain

//        animator.SetInteger("AttackIndex", 3);
//        animator.SetBool("isAttacking", true);

//        // ✅ activar hitbox
//        if (weapon != null) weapon.EnableHitbox();
//    }

//    public void EndAttack()
//    {
//        isAttacking = false;
//        animator.SetBool("isAttacking", false);
//        animator.SetInteger("AttackIndex", 0);

//        // ✅ desactivar hitbox
//        if (weapon != null) weapon.DisableHitbox();

//        if (queuedAttack)
//        {
//            queuedAttack = false;
//            LightAttack(); // encadena automáticamente
//        }
//        else
//        {
//            // Si pasó demasiado tiempo, reseteo combo
//            if (Time.time - lastAttackTime > comboResetTime)
//                comboStep = 0;
//        }
//    }



//    void Jump()
//    {
//        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//    }
////}
//using UnityEngine;
//using System.Collections;

//[RequireComponent(typeof(Rigidbody), typeof(Collider))]
//public class PlayerMovementRB : MonoBehaviour
//{
//    [Header("Componentes")]
//    public Animator animator;
//    private Rigidbody rb;

//    [Header("Movimiento")]
//    public float walkSpeed = 2f;
//    public float runSpeed = 4f;
//    public float rotationSpeed = 10f;

//    [Header("Salto")]
//    public float jumpForce = 7f;
//    public bool isGrounded;

//    [Header("Ataques")]
//    public bool isAttacking = false;
//    private int lastAttack = 0;

//    [Header("Combos")]
//    public int comboStep = 0;          // 0 = ninguno, 1 = ataque1, 2 = ataque2...
//    public float comboResetTime = 1f;  // tiempo para reiniciar combo si no seguiste
//    private float lastAttackTime = 0f;

//    private bool queuedAttack = false; // buffer de input

//    public WeaponHitbox weapon;
//    private Vector3 moveInput;
//    private bool isRunning;

//    // ================= DODGE (AÑADIDO MÍNIMO) =================
//    [Header("Dodge")]
//    public KeyCode dodgeKey = KeyCode.LeftControl;
//    public float dodgeDistance = 4f;
//    public float dodgeDuration = 0.25f;
//    public float dodgeCooldown = 0.6f;
//    private bool isDodging = false;
//    private float lastDodgeTime = -999f;
//    // ==========================================================

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//        rb.interpolation = RigidbodyInterpolation.Interpolate;
//    }

//    void Update()
//    {
//        // ----- Input movimiento -----
//        float horizontal = Input.GetAxis("Horizontal"); // A/D
//        float vertical = Input.GetAxis("Vertical");     // W/S
//        moveInput = new Vector3(horizontal, 0, vertical).normalized;

//        isRunning = Input.GetKey(KeyCode.LeftShift);

//        // ----- Animaciones locomoción -----
//        animator.SetFloat("VelX", horizontal);
//        animator.SetFloat("VelY", isRunning ? vertical * 2f : vertical);

//        // ----- Suelo / salto -----
//        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
//        animator.SetBool("isJumping", !isGrounded);
//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//            Jump();

//        // ----- Ataques -----
//        if (Input.GetMouseButtonDown(0) && !isAttacking)      // click izq
//            LightAttack();
//        else if (Input.GetMouseButtonDown(1) && !isAttacking) // click der
//            HeavyAttack();

//        // ----- AUTOCIERRE DE ATAQUE (sin Animation Events) -----
//        if (isAttacking)
//        {
//            var st = animator.GetCurrentAnimatorStateInfo(0);
//            // OJO: usá exactamente el nombre del estado de tu Attack1
//            bool inAttackState = st.IsName("Sword And Shield Kick") || st.IsName("Attack2") || st.IsName("AttackPower");

//            if (inAttackState && st.normalizedTime >= 0.8f && !animator.IsInTransition(0))
//                EndAttack();
//        }

//        // BOTÓN DEBUG opcional:
//        if (Input.GetKeyDown(KeyCode.R)) EndAttack();

//        // ================= DODGE (AÑADIDO MÍNIMO) =================
//        if (Input.GetKeyDown(dodgeKey))
//            TryDodge();
//        // ==========================================================
//    }

//    void FixedUpdate()
//    {
//        if (isAttacking) return; // 🔒 Bloquea movimiento mientras dura un ataque
//        if (isDodging) return;   // 🔒 Bloquea movimiento mientras dura el dash

//        // Personaje SIEMPRE mira hacia la cámara
//        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0;
//        Quaternion targetRotation = Quaternion.LookRotation(camForward);
//        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

//        // Movimiento relativo a la cámara
//        Vector3 camRight = Camera.main.transform.right; camRight.y = 0;
//        Vector3 moveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;

//        if (moveDir.magnitude > 0.1f && isGrounded)
//        {
//            float speed = isRunning ? runSpeed : walkSpeed;
//            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
//        }
//    }

//    void LightAttack()
//    {
//        // Si ya estoy atacando → marco que quiero encadenar
//        if (isAttacking)
//        {
//            queuedAttack = true;
//            return;
//        }

//        // Nuevo ataque
//        isAttacking = true;
//        comboStep++;

//        // Alternamos entre 1 y 2 (podés extender más pasos si querés)
//        if (comboStep > 2) comboStep = 1;

//        animator.SetInteger("AttackIndex", comboStep);
//        animator.SetBool("isAttacking", true);

//        lastAttackTime = Time.time;

//        // ✅ activar hitbox
//        if (weapon != null) weapon.EnableHitbox();
//    }

//    void HeavyAttack()
//    {
//        // Ataque fuerte siempre rompe combo
//        isAttacking = true;
//        comboStep = 0; // reset combo chain

//        animator.SetInteger("AttackIndex", 3);
//        animator.SetBool("isAttacking", true);

//        // ✅ activar hitbox
//        if (weapon != null) weapon.EnableHitbox();
//    }

//    public void EndAttack()
//    {
//        isAttacking = false;
//        animator.SetBool("isAttacking", false);
//        animator.SetInteger("AttackIndex", 0);

//        // ✅ desactivar hitbox
//        if (weapon != null) weapon.DisableHitbox();

//        if (queuedAttack)
//        {
//            queuedAttack = false;
//            LightAttack(); // encadena automáticamente
//        }
//        else
//        {
//            if (Time.time - lastAttackTime > comboResetTime)
//                comboStep = 0;
//        }
//    }

//    void Jump()
//    {
//        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//    }

//    // ================= DODGE (AÑADIDO MÍNIMO) =================
//    void TryDodge()
//    {
//        if (isDodging) return;
//        if (Time.time - lastDodgeTime < dodgeCooldown) return;
//        if (!isGrounded) return;          // mantengo tu lógica simple
//        if (isAttacking) return;          // no cancelar ataques

//        // Dirección relativa a cámara según input actual; si no hay, backstep
//        Vector3 camF = Camera.main.transform.forward; camF.y = 0; camF.Normalize();
//        Vector3 camR = Camera.main.transform.right; camR.y = 0; camR.Normalize();

//        float h = Input.GetAxisRaw("Horizontal");
//        float v = Input.GetAxisRaw("Vertical");
//        Vector3 dir = (camF * v + camR * h);
//        if (dir.sqrMagnitude < 0.01f) dir = -camF; // backstep por defecto

//        StartCoroutine(DodgeRoutine(dir.normalized));
//    }
//    IEnumerator DodgeRoutine(Vector3 dir)
//    {
//        isDodging = true;
//        lastDodgeTime = Time.time;

//        // === Activar animación de dodge ===
//        animator.SetBool("isDodging", true);
//        animator.SetInteger("DodgeDir", DirToIndexInt(dir));

//        // === Movimiento físico ===
//        float elapsed = 0f;
//        float speed = dodgeDistance / dodgeDuration;

//        while (elapsed < dodgeDuration)
//        {
//            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
//            elapsed += Time.fixedDeltaTime;
//            yield return new WaitForFixedUpdate();
//        }

//        // === Fin del dash ===
//        isDodging = false;
//        animator.SetBool("isDodging", false);
//        animator.SetInteger("DodgeDir", 0);
//    }


//    // Utilidades opcionales si querés setear DodgeDir:
//    int DirToIndexInt(Vector3 dir)
//    {
//        Vector3 camF = Camera.main.transform.forward; camF.y = 0; camF.Normalize();
//        Vector3 camR = Camera.main.transform.right; camR.y = 0; camR.Normalize();
//        float dotF = Vector3.Dot(dir, camF);
//        float dotR = Vector3.Dot(dir, camR);
//        if (Mathf.Abs(dotF) >= Mathf.Abs(dotR)) return (dotF >= 0f) ? 0 : 1; // 0 fwd, 1 back
//        else return (dotR >= 0f) ? 3 : 2; // 2 left, 3 right
//    }
//    float DirToIndexFloat(Vector3 dir) => (float)DirToIndexInt(dir);
//    // ==========================================================
//}
using UnityEngine;
using System.Collections;

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

    // ================= DODGE (nuevo sistema físico) =================
    [Header("Dodge")]
    public KeyCode dodgeKey = KeyCode.LeftControl;
    public float dodgeDistance = 4f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 0.6f;
    private bool isDodging = false;
    private float lastDodgeTime = -999f;
    [Range(0.8f, 1.5f)]
    public float dashSyncMultiplier = 1.15f; // Ajusta la sincronía entre animación y movimiento

    // ================================================================

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
        if (animator != null)
        {
            animator.SetFloat("VelX", horizontal);
            animator.SetFloat("VelY", isRunning ? vertical * 2f : vertical);
        }

        // ----- Suelo / salto -----
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (animator != null)
            animator.SetBool("isJumping", !isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        // ----- Ataques -----
        if (Input.GetMouseButtonDown(0) && !isAttacking)
            LightAttack();
        else if (Input.GetMouseButtonDown(1) && !isAttacking)
            HeavyAttack();

        // ----- Autocierre de ataque -----
        if (isAttacking)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            bool inAttackState = st.IsName("Sword And Shield Kick") || st.IsName("Attack2") || st.IsName("AttackPower");

            if (inAttackState && st.normalizedTime >= 0.8f && !animator.IsInTransition(0))
                EndAttack();
        }

        // BOTÓN DEBUG opcional:
        if (Input.GetKeyDown(KeyCode.R)) EndAttack();

        // ----- DODGE -----
        if (Input.GetKeyDown(dodgeKey))
            TryDodge();
    }

    void FixedUpdate()
    {
        if (isAttacking) return;
        if (isDodging) return;

        // Personaje mira hacia la cámara
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

    // ================= ATAQUES =================
    void LightAttack()
    {
        if (isAttacking)
        {
            queuedAttack = true;
            return;
        }

        isAttacking = true;
        comboStep++;
        if (comboStep > 2) comboStep = 1;

        animator.SetInteger("AttackIndex", comboStep);
        animator.SetBool("isAttacking", true);
        lastAttackTime = Time.time;

        if (weapon != null) weapon.EnableHitbox();
    }

    void HeavyAttack()
    {
        isAttacking = true;
        comboStep = 0;

        animator.SetInteger("AttackIndex", 3);
        animator.SetBool("isAttacking", true);

        if (weapon != null) weapon.EnableHitbox();
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        animator.SetInteger("AttackIndex", 0);

        if (weapon != null) weapon.DisableHitbox();

        if (queuedAttack)
        {
            queuedAttack = false;
            LightAttack();
        }
        else
        {
            if (Time.time - lastAttackTime > comboResetTime)
                comboStep = 0;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ================= DODGE =================
    void TryDodge()
    {
        if (isDodging) return;
        if (Time.time - lastDodgeTime < dodgeCooldown) return;
        if (!isGrounded) return;
        if (isAttacking) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Si no hay input, dash hacia atrás
        Vector3 dir = new Vector3(h, 0, v);
        if (dir.sqrMagnitude < 0.01f) dir = Vector3.back;

        // Dirección relativa a cámara
        Vector3 camF = Camera.main.transform.forward; camF.y = 0;
        Vector3 camR = Camera.main.transform.right; camR.y = 0;
        Vector3 moveDir = (camF * dir.z + camR * dir.x).normalized;

        StartCoroutine(DodgeRoutine(moveDir));
    }

    IEnumerator DodgeRoutine(Vector3 dir)
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        // --- Determinar dirección (0=fwd, 1=back, 2=left, 3=right)
        float dodgeDir = DirToIndexFloat(dir);

        // --- Activar animación ---
        animator.SetBool("isDodging", true);
        animator.SetFloat("DodgeDir", dodgeDir);

        // --- Duración real del clip actual ---
        float clipDuration = GetDodgeAnimDuration(dodgeDir);

        // --- Movimiento físico ---
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        float elapsed = 0f;
        float speed = (dodgeDistance / clipDuration) * dashSyncMultiplier;


        while (elapsed < clipDuration)
        {
            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // --- Restaurar físicas ---
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // --- Terminar animación ---
        animator.SetBool("isDodging", false);
        animator.SetFloat("DodgeDir", 0f);
        yield return new WaitForSeconds(0.5f);

        isDodging = false;
    }
    float GetDodgeAnimDuration(float dodgeDir)
    {
        // ⚙️ Asegurate que los nombres coincidan con los clips en tu Blend Tree
        switch ((int)dodgeDir)
        {
            case 0: return GetClipLength("DashForward");
            case 1: return GetClipLength("DashBackward");
            case 2: return GetClipLength("DashLeft");
            case 3: return GetClipLength("DashRight");
            default: return 0.6f;
        }
    }

    float GetClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName)
                return clip.length;

        return 0.6f; // fallback
    }

    float DirToIndexFloat(Vector3 dir)
    {
        Vector3 camF = Camera.main.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = Camera.main.transform.right; camR.y = 0; camR.Normalize();

        float dotF = Vector3.Dot(dir, camF);
        float dotR = Vector3.Dot(dir, camR);

        if (Mathf.Abs(dotF) >= Mathf.Abs(dotR))
            return (dotF >= 0f) ? 0f : 1f; // 0 = forward, 1 = back
        else
            return (dotR >= 0f) ? 3f : 2f; // 2 = left, 3 = right
    }


}
