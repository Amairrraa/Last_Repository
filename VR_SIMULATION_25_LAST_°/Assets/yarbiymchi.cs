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
    private bool previousGripState = false; // track grip press state

    private void Update()
    {
        // Ensure controller is valid
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        if (leftController.isValid)
        {
            // Read grip button as bool
            if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                // Detect rising edge (button just pressed down)
                if (gripPressed && !previousGripState)
                {
                    intakeActive = !intakeActive; // toggle intake
                    Debug.Log("Intake " + (intakeActive ? "ON" : "OFF"));
                }

                previousGripState = gripPressed;
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
