using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SLIPERYROBOT_VR : MonoBehaviour
{
    [Header("Wheel Assignments (Assign ALL Primary WheelColliders)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;

    [Header("Movement Powers")]
    public float maxMotorForce = 2000f;
    public float brakeForce = 500f;
    public float rotationLimit = 0.5f;

    [Header("Physics")]
    public Rigidbody robotMainRigidbody;
    public float customLateralForceStrength = 2000f;
    public Rigidbody rb;
    RigidbodyConstraints rc = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    [Header("Visuals (Optional)")]
    public List<WheelCollider> allWheelCollidersForVisuals = new List<WheelCollider>();
    public List<Transform> allVisualWheels = new List<Transform>();
    public GameObject c; // Optional visual indicator for strafing

    public float strafeForce = 500f; // Strafing strength

    void FixedUpdate()
    {
        // Get left and right hand devices
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Read joystick values
        leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftStick);
        rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightStick);

        // Deadzone
        if (leftStick.magnitude < 0.1f) leftStick = Vector2.zero;
        if (rightStick.magnitude < 0.1f) rightStick = Vector2.zero;

        float leftY = -leftStick.y;   // left wheel forward/back
        float rightY = -rightStick.y; // right wheel forward/back

        // List of all wheels
        List<WheelCollider> wheels = new List<WheelCollider> { frontLeftWheel, frontRightWheel, backLeftWheel, backRightWheel };
        wheels.RemoveAll(w => w == null);

        // Reset torque and brake
        foreach (var w in wheels)
        {
            w.motorTorque = 0f;
            w.brakeTorque = brakeForce;
        }

        // Forward/backward & tank turn
        if (Mathf.Abs(leftY) > 0f || Mathf.Abs(rightY) > 0f)
        {
            rb.constraints = (Mathf.Abs(leftY) > 0f && Mathf.Abs(rightY) > 0f) ? rc : RigidbodyConstraints.None;

            foreach (var wheel in wheels)
            {
                if (wheel.transform.localPosition.x < 0) // left wheels
                    wheel.motorTorque = rightY * maxMotorForce * ((Mathf.Abs(leftY) > 0f && Mathf.Abs(leftY) > 0f) ? 1f : rotationLimit);
                else // right wheels
                    wheel.motorTorque = leftY * maxMotorForce * ((Mathf.Abs(leftY) > 0f && Mathf.Abs(rightY) > 0f) ? 1f : rotationLimit);

                wheel.brakeTorque = 0f;
            }
        }

        // --- Strafing with B buttons ---
        leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool leftB);
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rightB);

        float strafeDir = 0f;
        if (rightB) strafeDir -= 1f;   // Left B = strafe left
        if (leftB) strafeDir += 1f;  // Right B = strafe right

        if (strafeDir != 0f)
        {
            Vector3 strafeVector = transform.right * strafeDir * strafeForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + strafeVector);
            c?.SetActive(true); // optional visual indicator for strafing
        }
        else
        {
            c?.SetActive(false);
        }

        UpdateAllWheelVisuals();
    }

    void UpdateAllWheelVisuals()
    {
        if (allWheelCollidersForVisuals.Count != allVisualWheels.Count) return;

        for (int i = 0; i < allWheelCollidersForVisuals.Count; i++)
        {
            WheelCollider wc = allWheelCollidersForVisuals[i];
            Transform vw = allVisualWheels[i];
            if (wc == null || vw == null) continue;

            wc.GetWorldPose(out Vector3 pos, out Quaternion rot);
            vw.position = pos;
            vw.rotation = rot;
        }
    }
}
