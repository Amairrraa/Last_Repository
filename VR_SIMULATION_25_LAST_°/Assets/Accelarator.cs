using UnityEngine;
using UnityEngine.XR;

public class WheelRotatorVR : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.right; // Default X axis
    public float rotationSpeed = 100f;

    void Update()
    {
        // Get right-hand controller
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Check grip button (treated as a binary press)
        if (rightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed) && gripPressed)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
