using UnityEngine;
using UnityEngine.XR;

public class SimpleIntake : MonoBehaviour
{
    [Header("Settings")]
    public float intakeForce = 10f;
    public Vector3 intakeDirection = Vector3.forward; // Adjust in Inspector

    [Header("Roller Visual")]
    public Transform roller;
    public float rollerSpeed = 360f;

    private bool intakeActive = false;
    private InputDevice leftController;

    private void Update()
    {
        // Make sure we always have a valid controller
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        // Check grip input
        if (leftController.isValid)
        {
            // Some devices use gripButton (boolean)
            if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                intakeActive = gripPressed;
            }

            // Some devices use grip (float 0..1)
            if (leftController.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                if (gripValue > 0.1f) // pressed slightly
                    intakeActive = true;
                else
                    intakeActive = false;
            }
        }

        // Rotate roller if intake is active
        if (intakeActive && roller != null)
        {
            roller.Rotate(Vector3.forward * rollerSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!intakeActive) return;

        if (other.CompareTag("BiodiversityUnit"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(transform.TransformDirection(intakeDirection) * intakeForce);
            }
        }
    }
}
