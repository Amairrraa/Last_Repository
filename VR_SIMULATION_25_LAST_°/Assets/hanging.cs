using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation

    void Update()
    {
        // Rotate Right when pressing R
        if (Input.GetKey(KeyCode.R))
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }

        // Rotate Left when pressing T
        if (Input.GetKey(KeyCode.T))
        {
            transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
        }
    }
}
