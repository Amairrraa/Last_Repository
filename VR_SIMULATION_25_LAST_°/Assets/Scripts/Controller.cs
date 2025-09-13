using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public float moveSpeed = 5f;   // forward/backward speed
    public float turnSpeed = 100f; // rotation speed (degrees per second)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    
    }

    void FixedUpdate()
    {
        // forward/back input (W/S or Up/Down)
        float moveInput = Input.GetAxis("Vertical");

        // turn input (A/D or Left/Right)
        float turnInput = Input.GetAxis("Horizontal");

        // move in the forward direction
        Vector3 move = transform.forward * moveInput * moveSpeed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // rotate around Y axis
        if (turnInput != 0)
        {
            Quaternion turn = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turn);
    }
}}