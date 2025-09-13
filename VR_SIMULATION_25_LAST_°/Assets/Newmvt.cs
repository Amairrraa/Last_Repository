using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class VRSlipperyRobot : MonoBehaviour
{
    [Header("Wheel Colliders (assign)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;

    [Header("Middle Wheel Visual (no collider)")]
    public Transform centerVisual;

    [Header("Wheel Visuals (optional)")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform backLeftVisual;
    public Transform backRightVisual;

    [Header("Physics & Tuning")]
    public Rigidbody rb;
    public float maxMotorForce = 1500f;
    public float torqueSmoothing = 8f;
    public float brakeForce = 8000f;
    public float lateralStrength = 1200f;   // side strafe force
    public float rotationTorque = 1200f;
    public float rotationSmoothing = 6f;
    public float wheelVisualRotationSpeed = 800f;
    [Range(0f, 0.4f)]
    public float deadzone = 0.18f;
    public bool invertY = false;

    // Internal smoothed values
    float targetFL, targetFR, targetBL, targetBR;
    float currentFL, currentFR, currentBL, currentBR;
    float targetYawTorque, currentYawTorque;

    // VR inputs
    private InputDevice leftHand;
    private InputDevice rightHand;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        ConfigureDefaultSidewaysFriction(frontLeftWheel);
        ConfigureDefaultSidewaysFriction(frontRightWheel);
        ConfigureDefaultSidewaysFriction(backLeftWheel);
        ConfigureDefaultSidewaysFriction(backRightWheel);
    }

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
        float delta = Time.fixedDeltaTime;

        // --- Get VR joystick input ---
        Vector2 leftAxis = Vector2.zero;
        Vector2 rightAxis = Vector2.zero;
        if (leftHand.isValid)
            leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftAxis);
        if (rightHand.isValid)
            rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightAxis);

        float leftY = invertY ? -leftAxis.y : leftAxis.y;
        float rightY = invertY ? -rightAxis.y : rightAxis.y;
        float strafeX = rightAxis.x;  // horizontal axis for strafing

        // Apply deadzone
        leftY = Mathf.Abs(leftY) < deadzone ? 0f : leftY;
        rightY = Mathf.Abs(rightY) < deadzone ? 0f : rightY;
        strafeX = Mathf.Abs(strafeX) < deadzone ? 0f : strafeX;

        // --- Wheel targets ---
        targetFL = leftY * maxMotorForce;
        targetBL = leftY * maxMotorForce;
        targetFR = rightY * maxMotorForce;
        targetBR = rightY * maxMotorForce;

        // --- Rotation (tank-style turning) ---
        targetYawTorque = (rightY - leftY) * rotationTorque;
        currentYawTorque = Mathf.Lerp(currentYawTorque, targetYawTorque, 1 - Mathf.Exp(-rotationSmoothing * delta));
        if (Mathf.Abs(currentYawTorque) > 0.01f)
            rb.AddTorque(Vector3.up * currentYawTorque, ForceMode.Force);

        // --- Braking when idle ---
        bool anyInput = Mathf.Abs(leftY) > 0f || Mathf.Abs(rightY) > 0f || Mathf.Abs(strafeX) > 0f;
        float appliedBrake = anyInput ? 0f : brakeForce;

        // --- Smooth motor torque ---
        currentFL = Mathf.Lerp(currentFL, targetFL, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentFR = Mathf.Lerp(currentFR, targetFR, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentBL = Mathf.Lerp(currentBL, targetBL, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentBR = Mathf.Lerp(currentBR, targetBR, 1 - Mathf.Exp(-torqueSmoothing * delta));

        ApplyMotorAndBrakes(frontLeftWheel, currentFL, appliedBrake);
        ApplyMotorAndBrakes(frontRightWheel, currentFR, appliedBrake);
        ApplyMotorAndBrakes(backLeftWheel, currentBL, appliedBrake);
        ApplyMotorAndBrakes(backRightWheel, currentBR, appliedBrake);

        // --- Middle wheel strafing ---
        float lateralForce = strafeX * lateralStrength;
        if (Mathf.Abs(lateralForce) > 0.01f)
            rb.AddForce(transform.right * lateralForce, ForceMode.Force);

        // --- Update visuals ---
        UpdateWheelVisual(frontLeftWheel, frontLeftVisual, currentFL);
        UpdateWheelVisual(frontRightWheel, frontRightVisual, currentFR);
        UpdateWheelVisual(backLeftWheel, backLeftVisual, currentBL);
        UpdateWheelVisual(backRightWheel, backRightVisual, currentBR);

        if (centerVisual != null)
        {
            float rotationAmount = strafeX * lateralStrength * delta;
            centerVisual.Rotate(Vector3.forward, rotationAmount, Space.Self);
        }
    }

    void ApplyMotorAndBrakes(WheelCollider col, float torqueVal, float brake)
    {
        if (col == null) return;
        col.motorTorque = torqueVal;
        col.brakeTorque = brake;
        col.steerAngle = 0f;
    }

    void UpdateWheelVisual(WheelCollider col, Transform visual, float torqueEstimate)
    {
        if (visual == null) return;
        if (col != null)
        {
            Vector3 pos;
            Quaternion rot;
            col.GetWorldPose(out pos, out rot);
            visual.position = pos;
            visual.rotation = rot;

            float spin = torqueEstimate * Time.fixedDeltaTime;
            visual.Rotate(Vector3.right, spin * wheelVisualRotationSpeed, Space.Self);
        }
    }

    void ConfigureDefaultSidewaysFriction(WheelCollider col)
    {
        if (col == null) return;
        WheelFrictionCurve side = col.sidewaysFriction;
        side.extremumSlip = 0.2f;
        side.extremumValue = 1f;
        side.asymptoteSlip = 0.5f;
        side.asymptoteValue = 0.75f;
        side.stiffness = 1f;
        col.sidewaysFriction = side;
    }

    void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rb.worldCenterOfMass, 0.06f);
        }
    }
}
