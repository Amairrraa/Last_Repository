using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller2 : MonoBehaviour
{
    public float motorForce = 1500f;
    public float brakeForce = 3000f;

    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;

    private void FixedUpdate()
    {
        float leftPower = 0f;
        float rightPower = 0f;

        // Forward / Backward
        if (Input.GetKey(KeyCode.W))
        {
            leftPower = 1f;
            rightPower = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            leftPower = -1f;
            rightPower = -1f;
        }

        // Turning
        if (Input.GetKey(KeyCode.A))
        {
            leftPower -= 1f;
            rightPower += 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            leftPower += 1f;
            rightPower -= 1f;
        }

        // Normalize values to [-1, 1]
        leftPower = Mathf.Clamp(leftPower, -1f, 1f);
        rightPower = Mathf.Clamp(rightPower, -1f, 1f);

        // Apply motor torque
        frontLeftWheel.motorTorque = leftPower * motorForce;
        rearLeftWheel.motorTorque = leftPower * motorForce;

        frontRightWheel.motorTorque = rightPower * motorForce;
        rearRightWheel.motorTorque = rightPower * motorForce;

        // Apply brakes when no input
        if (leftPower == 0f && rightPower == 0f)
        {
            ApplyBrakes(brakeForce);
        }
        else
        {
            ApplyBrakes(0f); // release brakes
        }

        UpdateWheels();
    }

    void ApplyBrakes(float force)
    {
        frontLeftWheel.brakeTorque = force;
        rearLeftWheel.brakeTorque = force;
        frontRightWheel.brakeTorque = force;
        rearRightWheel.brakeTorque = force;
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftTransform);
        UpdateSingleWheel(frontRightWheel, frontRightTransform);
        UpdateSingleWheel(rearLeftWheel, rearLeftTransform);
        UpdateSingleWheel(rearRightWheel, rearRightTransform);
    }

    void UpdateSingleWheel(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        trans.position = pos;
        trans.rotation = rot;
    }
}
