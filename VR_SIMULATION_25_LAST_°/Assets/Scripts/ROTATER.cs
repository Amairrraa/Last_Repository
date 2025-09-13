using UnityEngine;
using UnityEngine.XR;

public class HangingRotater : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.right; // X-axis rotation
    public float rotationSpeed = 50f;

    [Header("XR Settings")]
    public XRNode positiveController = XRNode.RightHand; // Right-hand controller
    public XRNode negativeController = XRNode.LeftHand;  // Left-hand controller

    public InputFeatureUsage<bool> positiveButton = CommonUsages.primaryButton;   // A button
    public InputFeatureUsage<bool> negativeButton = CommonUsages.primaryButton;   // X button

    void Update()
    {
        // Get devices
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(positiveController);
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(negativeController);

        bool positivePressed = false;
        bool negativePressed = false;

        if (rightDevice.isValid)
            rightDevice.TryGetFeatureValue(positiveButton, out positivePressed);

        if (leftDevice.isValid)
            leftDevice.TryGetFeatureValue(negativeButton, out negativePressed);

        // Rotate in positive direction
        if (positivePressed)
        {
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        }

        // Rotate in negative direction
        if (negativePressed)
        {
            transform.Rotate(-rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
