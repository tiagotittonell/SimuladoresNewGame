using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    public Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Distancia / Zoom")]
    public float distance = 6f;
    public float minDistance = 2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 3f;

    [Header("Shake (impacto)")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.15f;
    public float shakeRoughness = 15f;
    private float shakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    [Header("Rotación con mouse")]
    public float sensitivityX = 200f;
    public float sensitivityY = 100f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Suavizado")]
    public float rotSmoothTime = 0.05f;
    public float posSmoothTime = 0.05f;

    [Header("Colisión")]
    public LayerMask collisionMask = ~0;
    public float collisionRadius = 0.2f;

    [Header("Cursor")]
    public bool lockCursor = true;

    float yaw;
    float pitch;
    float yawVel;
    float pitchVel;
    Vector3 posVel;

    void Start()
    {
        TryFindTarget();

        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: no se encontró un jugador inicial.");
            return;
        }

        Vector3 e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            TryFindTarget();
            if (target == null) return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.unscaledDeltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, yaw, ref yawVel, rotSmoothTime);
        float smoothPitch = Mathf.SmoothDampAngle(transform.eulerAngles.x, pitch, ref pitchVel, rotSmoothTime);
        Quaternion rot = Quaternion.Euler(smoothPitch, smoothYaw, 0f);

        Vector3 pivot = target.position + pivotOffset;

        Vector3 desiredPos = pivot - rot * Vector3.forward * distance;

        if (Physics.SphereCast(pivot, collisionRadius, (desiredPos - pivot).normalized,
                               out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float safeDist = Mathf.Clamp(hit.distance - 0.05f, minDistance, distance);
            desiredPos = pivot - rot * Vector3.forward * safeDist;
        }

        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, desiredPos, ref posVel, posSmoothTime);

        ApplyShake();
        smoothPos += shakeOffset;

        transform.position = smoothPos;
        transform.rotation = rot;
    }

    void TryFindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    public void ShakeCamera(float intensity = 1f)
    {
        shakeTimer = shakeDuration;
        shakeMagnitude = 0.15f * intensity;
    }

    void ApplyShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float shake = Mathf.Clamp01(shakeTimer / shakeDuration);
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeRoughness, 0f) - 0.5f) * 2f * shakeMagnitude * shake;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeRoughness) - 0.5f) * 2f * shakeMagnitude * shake;
            float offsetZ = (Mathf.PerlinNoise(Time.time * shakeRoughness, Time.time * 1.5f) - 0.5f) * 2f * shakeMagnitude * shake;
            shakeOffset = new Vector3(offsetX, offsetY, offsetZ);
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }
}
