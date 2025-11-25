using UnityEngine;

public class CharacterPreviewRotator : MonoBehaviour
{
    public Transform character;
    public float rotationSpeed = 5f;
    private float rotationY;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
            character.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }
}
