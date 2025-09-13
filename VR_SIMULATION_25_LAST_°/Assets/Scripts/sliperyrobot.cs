using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlipperyRobotWithMiddleWheelZ : MonoBehaviour
{
    [Header("Wheel Colliders (assign)")]
    public WheelCollider frontLeftWheel;   // Z
    public WheelCollider frontRightWheel;  // E
    public WheelCollider backLeftWheel;    // S
    public WheelCollider backRightWheel;   // D

    [Header("Middle Wheel Visual (no collider)")]
    public Transform centerVisual;

    [Header("Wheel Visuals (optional)")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform backLeftVisual;
    public Transform backRightVisual;

    [Header("Physics & tuning")]
    public Rigidbody rb;
    public float maxMotorForce = 1500f;       // torque for main wheels
    public float torqueSmoothing = 8f;
    public float brakeForce = 8000f;
    public float lateralStrength = 1200f;     // force for middle wheel
    public float rotationTorque = 1200f;      // for turning
    public float rotationSmoothing = 6f;
    public float wheelVisualRotationSpeed = 800f;

    // internal smoothed targets
    float targetFL, targetFR, targetBL, targetBR;
    float currentFL, currentFR, currentBL, currentBR;
    float targetYawTorque, currentYawTorque;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        ConfigureDefaultSidewaysFriction(frontLeftWheel);
        ConfigureDefaultSidewaysFriction(frontRightWheel);
        ConfigureDefaultSidewaysFriction(backLeftWheel);
        ConfigureDefaultSidewaysFriction(backRightWheel);
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        // --- Rotation (Arrow Keys) ---
        float turnInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow)) turnInput = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow)) turnInput = -1f;

        targetYawTorque = turnInput * rotationTorque;
        currentYawTorque = Mathf.Lerp(currentYawTorque, targetYawTorque, 1 - Mathf.Exp(-rotationSmoothing * delta));
        if (Mathf.Abs(currentYawTorque) > 0.01f)
            rb.AddTorque(Vector3.up * currentYawTorque, ForceMode.Force);

        // --- Individual wheel keys ---
        bool keyZ = Input.GetKey(KeyCode.S); // front left
        bool keyE = Input.GetKey(KeyCode.D); // front right
        bool keyS = Input.GetKey(KeyCode.W); // back left
        bool keyD = Input.GetKey(KeyCode.E); // back right

        // Middle wheel strafing keys
        float strafeDir = 0f;
        if (Input.GetKey(KeyCode.I)) strafeDir = 1f;    // strafe right
        if (Input.GetKey(KeyCode.P)) strafeDir = -1f;   // strafe left

        // Reset targets
        targetFL = targetFR = targetBL = targetBR = 0f;

        if (keyZ) targetFL = maxMotorForce;
        if (keyE) targetFR = maxMotorForce;
        if (keyS) targetBL = -maxMotorForce;
        if (keyD) targetBR = -maxMotorForce;

        // Brake if no input
        bool anyInput = keyZ || keyE || keyS || keyD || strafeDir != 0f || turnInput != 0f;
        float appliedBrake = anyInput ? 0f : brakeForce;

        // Smooth torque
        currentFL = Mathf.Lerp(currentFL, targetFL, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentFR = Mathf.Lerp(currentFR, targetFR, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentBL = Mathf.Lerp(currentBL, targetBL, 1 - Mathf.Exp(-torqueSmoothing * delta));
        currentBR = Mathf.Lerp(currentBR, targetBR, 1 - Mathf.Exp(-torqueSmoothing * delta));

        ApplyMotorAndBrakes(frontLeftWheel, currentFL, appliedBrake);
        ApplyMotorAndBrakes(frontRightWheel, currentFR, appliedBrake);
        ApplyMotorAndBrakes(backLeftWheel, currentBL, appliedBrake);
        ApplyMotorAndBrakes(backRightWheel, currentBR, appliedBrake);

        // --- Middle wheel lateral force ---
        float lateralForce = strafeDir * lateralStrength;
        if (Mathf.Abs(lateralForce) > 0.01f)
            rb.AddForce(transform.right * lateralForce, ForceMode.Force);

        // --- Update visuals ---
        UpdateWheelVisual(frontLeftWheel, frontLeftVisual, currentFL);
        UpdateWheelVisual(frontRightWheel, frontRightVisual, currentFR);
        UpdateWheelVisual(backLeftWheel, backLeftVisual, currentBL);
        UpdateWheelVisual(backRightWheel, backRightVisual, currentBR);

        // Middle wheel visual rotation along Z axis
        if (centerVisual != null)
        {
            float rotationAmount = strafeDir * lateralStrength * delta;
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


