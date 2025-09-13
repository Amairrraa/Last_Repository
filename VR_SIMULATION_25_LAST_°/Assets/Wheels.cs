using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class OmniWheelVRRobot4W : MonoBehaviour
{
    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeft, backLeft;
    public WheelCollider frontRight, backRight;

    [Header("Wheel Visuals")]
    public Transform vFrontLeft, vBackLeft;
    public Transform vFrontRight, vBackRight;

    [Header("Drive Settings")]
    public float maxForce = 1500f;       // forward/back torque
    [Range(0f, 0.4f)]
    public float deadzone = 0.18f;
    public bool invertY = false;

    private InputDevice leftHand;
    private InputDevice rightHand;

    void TryInitDevices()
    {
        if (!leftHand.isValid)
        {
            var list = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, list);
            if (list.Count > 0) leftHand = list[0];
        }
        if (!rightHand.isValid)
        {
            var list = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, list);
            if (list.Count > 0) rightHand = list[0];
        }
    }

    void FixedUpdate()
    {
        TryInitDevices();

        Vector2 leftAxis = Vector2.zero;
        Vector2 rightAxis = Vector2.zero;

        if (leftHand.isValid)
            leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftAxis);
        if (rightHand.isValid)
            rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightAxis);

        float leftY = invertY ? -leftAxis.y : leftAxis.y;
        float rightY = invertY ? -rightAxis.y : rightAxis.y;

        // Apply deadzone
        leftY = Mathf.Abs(leftY) < deadzone ? 0f : leftY;
        rightY = Mathf.Abs(rightY) < deadzone ? 0f : rightY;

        // --- Calculate wheel torques ---
        float leftTorque = leftY * maxForce;
        float rightTorque = rightY * maxForce;

        ApplyTorque(new WheelCollider[] { frontLeft, backLeft }, leftTorque);
        ApplyTorque(new WheelCollider[] { frontRight, backRight }, rightTorque);

        // --- Update visuals ---
        UpdateWheelVisual(frontLeft, vFrontLeft);
        UpdateWheelVisual(backLeft, vBackLeft);
        UpdateWheelVisual(frontRight, vFrontRight);
        UpdateWheelVisual(backRight, vBackRight);
    }

    void ApplyTorque(WheelCollider[] wheels, float torque)
    {
        if (wheels == null) return;

        foreach (var w in wheels)
        {
            if (w)
            {
                w.motorTorque = torque;
                w.brakeTorque = 0f;
                w.steerAngle = 0f;
            }
        }
    }

    void UpdateWheelVisual(WheelCollider wc, Transform visual)
    {
        if (!wc || !visual) return;
        wc.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visual.position = pos;
        visual.rotation = rot;
    }

    void OnDrawGizmos()
    {
        if (!rb) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rb.worldCenterOfMass, 0.1f);
    }
}
