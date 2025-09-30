using UnityEngine;
using UnityEngine.XR;

public class LimitedRotatorVR : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.right; // Axis of rotation (local)
    public float rotationSpeed = 90f;            // Degrees per second

    [Header("Limits")]
    public float minAngle = -45f; // Lower limit (degrees)
    public float maxAngle = 45f;  // Upper limit (degrees)

    private float currentAngle = 0f;

    void Update()
    {
        // Get right-hand controller
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check "select" (trigger) button
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
        {
            float delta = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

            // Apply only allowed delta
            float appliedDelta = newAngle - currentAngle;
            transform.Rotate(rotationAxis * appliedDelta, Space.Self);

            // Update stored angle
            currentAngle = newAngle;
        }
    }
}
