using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AcceleratorTrigger : MonoBehaviour
{
    private Transform activeAccelerator = null;
    private BiodiversityDispenser activeDispenser = null;

    [Header("Rotation Settings")]
    [Tooltip("Target rotation speed in RPM (rotations per minute)")]
    public float targetRPM = 250f;

    private float rotationSpeed; // degrees per second
    private float rotationTimer = 0f;

    private InputDevice rightHandDevice;

    void Start()
    {
        // Convert RPM to degrees per second
        rotationSpeed = targetRPM * 360f / 60f;
        TryGetRightHandDevice();
    }

    void Update()
    {
        // If device becomes invalid (controller disconnected), try to reacquire it.
        if (!rightHandDevice.isValid)
            TryGetRightHandDevice();

        bool rotating = activeAccelerator != null && IsRightGripPressed();

        if (rotating)
        {
            activeAccelerator.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
            rotationTimer += Time.deltaTime;

            Debug.Log($"[ACCELERATOR] Rotating. Time: {rotationTimer:F2}s");

            if (activeDispenser != null)
            {
                activeDispenser.ResetDowngradeTimer();

                if (rotationTimer >= 10f)
                    activeDispenser.SetDispenserState(BiodiversityDispenser.SpawnState.High);
                else if (rotationTimer >= 5f)
                    activeDispenser.SetDispenserState(BiodiversityDispenser.SpawnState.Medium);
            }
        }
        else if (rotationTimer > 0f)
        {
            Debug.Log("[ACCELERATOR] Rotation stopped. Timer reset.");
            rotationTimer = 0f;
        }
    }

    private void TryGetRightHandDevice()
    {
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
            rightHandDevice = rightHandDevices[0];
    }

    private bool IsRightGripPressed()
    {
        if (!rightHandDevice.isValid) return false;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool pressed))
            return pressed;
        return false;
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
