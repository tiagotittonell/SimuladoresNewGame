using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target; // El personaje
    public Vector3 offset = new Vector3(0, 3, -6);

    [Header("Rotación con mouse")]
    public float sensitivityX = 200f;
    public float sensitivityY = 100f;
    public float minY = -20f;
    public float maxY = 60f;

    private float rotX; // rotación vertical acumulada
    private float rotY; // rotación horizontal acumulada

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Input de mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minY, maxY);

        // Rotación de la cámara
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);

        // Posición de la cámara con offset
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = desiredPosition;

        // Mirar siempre al jugador
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
