using UnityEngine;

public class CameraFollowCursor : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 200f;   // How fast the camera rotates
    public float clampAngle = 80f;     // Max up/down rotation

    private float rotX; // Rotation around X (vertical look)
    private float rotY; // Rotation around Y (horizontal look)

    void Start()
    {
        // Lock and hide the cursor in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize starting rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    void Update()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust rotation based on input
        rotY += mouseX * sensitivity * Time.deltaTime;
        rotX += mouseY * sensitivity * Time.deltaTime;

        // Clamp vertical rotation
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        // Apply rotation
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);
        transform.rotation = localRotation;
    }
}
