using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;
    public float gravity = -13f;
    public float jumpHeight = 1f;

    private CharacterController controller;
    private Vector3 velocity;
    private Transform cam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform; // Reference to the camera
    }

    void Update()
    {
        // --- MOVEMENT ---
        float horizontal = Input.GetAxis("Horizontal"); // Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // Up/Down arrows

        // Move relative to the camera's forward/right
        Vector3 move = cam.forward * vertical + cam.right * horizontal;
        move.y = 0f; // Prevent moving up/down with camera tilt

        controller.Move(move.normalized * speed * Time.deltaTime);

        // --- GRAVITY ---
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // --- JUMP (hold Space = continuous jumping) ---
        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
