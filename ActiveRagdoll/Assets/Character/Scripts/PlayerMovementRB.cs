
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
    public int comboStep = 0;          
    public float comboResetTime = 1f;  
    private float lastAttackTime = 0f;
    private bool queuedAttack = false; 

    public WeaponHitbox weapon;
    private Vector3 moveInput;
    private bool isRunning;

    [Header("Dodge")]
    public KeyCode dodgeKey = KeyCode.LeftControl;
    public float dodgeDistance = 4f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 0.6f;
    private bool isDodging = false;
    private float lastDodgeTime = -999f;
    [Range(0.8f, 1.5f)]
    public float dashSyncMultiplier = 1.15f; 


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical");    
        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (animator != null)
        {
            animator.SetFloat("VelX", horizontal);
            animator.SetFloat("VelY", isRunning ? vertical * 2f : vertical);
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        if (animator != null)
            animator.SetBool("isJumping", !isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (Input.GetMouseButtonDown(0) && !isAttacking)
            LightAttack();
        else if (Input.GetMouseButtonDown(1) && !isAttacking)
            HeavyAttack();

        if (isAttacking)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            bool inAttackState = st.IsName("Sword And Shield Slash") || st.IsName("Attack2") || st.IsName("AttackPower");

            if (inAttackState && st.normalizedTime >= 0.8f && !animator.IsInTransition(0))
                EndAttack();
        }

        if (Input.GetKeyDown(KeyCode.R)) EndAttack();

        if (Input.GetKeyDown(dodgeKey))
            TryDodge();
    }

  

    void FixedUpdate()
    {
        if (isDodging) return;

        // Dirección hacia la cámara
        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

        // Movimiento relativo a la cámara
        Vector3 camRight = Camera.main.transform.right; camRight.y = 0;
        Vector3 moveDir = (camForward * moveInput.z + camRight * moveInput.x).normalized;
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
        if (moveDir.magnitude > 0.1f && isGrounded)
        {
            float speed;

            if (isAttacking)
                speed = (isRunning ? runSpeed : walkSpeed) * 0.35f; 
            else
                speed = isRunning ? runSpeed : walkSpeed;

            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
        }
    }

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

    void TryDodge()
    {
        if (isDodging) return;
        if (Time.time - lastDodgeTime < dodgeCooldown) return;
        if (!isGrounded) return;
        if (isAttacking) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        if (dir.sqrMagnitude < 0.01f) dir = Vector3.back;

        Vector3 camF = Camera.main.transform.forward; camF.y = 0;
        Vector3 camR = Camera.main.transform.right; camR.y = 0;
        Vector3 moveDir = (camF * dir.z + camR * dir.x).normalized;

        StartCoroutine(DodgeRoutine(moveDir));
    }

    IEnumerator DodgeRoutine(Vector3 dir)
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        float dodgeDir = DirToIndexFloat(dir);

        // --- Activar animación ---
        animator.SetBool("isDodging", true);
        animator.SetFloat("DodgeDir", dodgeDir);

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

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animator.SetBool("isDodging", false);
        animator.SetFloat("DodgeDir", 0f);
        yield return new WaitForSeconds(0.5f);

        isDodging = false;
    }
    float GetDodgeAnimDuration(float dodgeDir)
    {
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

        return 0.6f; 
    }

    float DirToIndexFloat(Vector3 dir)
    {
        Vector3 camF = Camera.main.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = Camera.main.transform.right; camR.y = 0; camR.Normalize();

        float dotF = Vector3.Dot(dir, camF);
        float dotR = Vector3.Dot(dir, camR);

        if (Mathf.Abs(dotF) >= Mathf.Abs(dotR))
            return (dotF >= 0f) ? 0f : 1f;
        else
            return (dotR >= 0f) ? 3f : 2f;
    }


}
