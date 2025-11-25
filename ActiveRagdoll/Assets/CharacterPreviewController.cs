using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform characterRoot; // El modelo a rotar
    public Camera previewCamera;

    [Header("Rotación")]
    public float rotationSpeed = 120f;
    public bool invertHorizontal = false;

    private bool dragging = false;
    private float currentRotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (characterRoot != null)
            currentRotationY = characterRoot.eulerAngles.y;
    }

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            dragging = true;

        if (Input.GetMouseButtonUp(0))
            dragging = false;

        if (!dragging) return;

        float mouseDelta = Input.GetAxis("Mouse X");

        if (invertHorizontal)
            mouseDelta = -mouseDelta;

        currentRotationY += mouseDelta * rotationSpeed * Time.deltaTime;

        if (characterRoot != null)
        {
            characterRoot.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
        }
    }
}
