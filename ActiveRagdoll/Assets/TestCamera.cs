using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -6);
    public float sensitivityX = 200f;
    public float sensitivityY = 100f;
    public float minY = -20f;
    public float maxY = 60f;

    private float rotX;
    private float rotY;

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            Vector3 dir = transform.position - target.position;
            Quaternion lookRot = Quaternion.LookRotation(-dir);
            rotX = lookRot.eulerAngles.x;
            rotY = lookRot.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // Entrada de mouse cruda
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minY, maxY);

        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 desiredPos = target.position + rotation * offset;

        // Suavizado opcional
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
