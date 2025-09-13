using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheel Visuals")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    [Header("Driving Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;

    private bool controlsEnabled = false;

    private void OnEnable()
    {
        TutorialManager.OnGameStarted += EnableControls;
    }

    private void OnDisable()
    {
        TutorialManager.OnGameStarted -= EnableControls;
    }

    private void EnableControls()
    {
        controlsEnabled = true;
        Debug.Log("[ROBOT] Controls enabled.");
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled) return;

        HandleInput();
        UpdateWheelVisuals(frontLeftWheel, frontLeftVisual);
        UpdateWheelVisuals(frontRightWheel, frontRightVisual);
        UpdateWheelVisuals(rearLeftWheel, rearLeftVisual);
        UpdateWheelVisuals(rearRightWheel, rearRightVisual);
    }

    private void HandleInput()
    {
        float motor = 0f;
        float brake = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            motor = motorForce;
        else if (Input.GetKey(KeyCode.DownArrow))
            motor = -motorForce;

        if (Input.GetKey(KeyCode.Space))
            brake = brakeForce;

        ApplyWheel(frontLeftWheel, motor, brake);
        ApplyWheel(frontRightWheel, motor, brake);
        ApplyWheel(rearLeftWheel, motor, brake);
        ApplyWheel(rearRightWheel, motor, brake);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            frontLeftWheel.motorTorque = -motorForce;
            rearLeftWheel.motorTorque = -motorForce;
            frontRightWheel.motorTorque = motorForce;
            rearRightWheel.motorTorque = motorForce;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            frontLeftWheel.motorTorque = motorForce;
            rearLeftWheel.motorTorque = motorForce;
            frontRightWheel.motorTorque = -motorForce;
            rearRightWheel.motorTorque = -motorForce;
        }
    }

    private void ApplyWheel(WheelCollider wheel, float motor, float brake)
    {
        wheel.motorTorque = motor;
        wheel.brakeTorque = brake;
    }

    private void UpdateWheelVisuals(WheelCollider collider, Transform visual)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        visual.position = pos;
        visual.rotation = rot;
    }
}