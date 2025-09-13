using UnityEngine;

public class AcceleratorTrigger : MonoBehaviour
{
    private Transform activeAccelerator = null;
    private BiodiversityDispenser activeDispenser = null;

    [Header("Rotation Settings")]
    [Tooltip("Target rotation speed in RPM (rotations per minute)")]
    public float targetRPM = 250f;

    private float rotationSpeed; // Calculated degrees per second
    private float rotationTimer = 0f;

    void Start()
    {
        // Convert RPM to degrees per second: 1 rotation = 360 degrees
        rotationSpeed = targetRPM * 360f / 60f;
    }

    void Update()
    {
        bool rotating = activeAccelerator != null && Input.GetKey(KeyCode.R);

        if (rotating)
        {
            activeAccelerator.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
            rotationTimer += Time.deltaTime;

            Debug.Log($"[ACCELERATOR] Rotating. Time: {rotationTimer:F2}s");

            if (activeDispenser != null)
            {
                activeDispenser.ResetDowngradeTimer();

                if (rotationTimer >= 10f)
                {
                    activeDispenser.SetDispenserState(BiodiversityDispenser.SpawnState.High);
                }
                else if (rotationTimer >= 5f)
                {
                    activeDispenser.SetDispenserState(BiodiversityDispenser.SpawnState.Medium);
                }
            }
        }
        else if (rotationTimer > 0f)
        {
            Debug.Log("[ACCELERATOR] Rotation stopped. Timer reset.");
            rotationTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AcceleratorZone1") || other.CompareTag("AcceleratorZone2"))
        {
            AcceleratorZone zone = other.GetComponent<AcceleratorZone>();
            if (zone != null)
            {
                activeAccelerator = zone.acceleratorWheel;
                activeDispenser = zone.linkedDispenser;
                Debug.Log("[ACCELERATOR] Entered accelerator zone. Ready to rotate.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AcceleratorZone1") || other.CompareTag("AcceleratorZone2"))
        {
            Debug.Log("[ACCELERATOR] Exited accelerator zone.");
            activeAccelerator = null;
            activeDispenser = null;
        }
    }
}
