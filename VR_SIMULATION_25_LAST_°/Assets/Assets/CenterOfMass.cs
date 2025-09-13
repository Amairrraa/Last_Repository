using UnityEngine;

// This script ONLY calculates and displays the Center of Mass.
// It does NOT automatically apply it to the Rigidbody.
public class CoM_Calculator : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag your main Rigidbody component here.")]
    public Rigidbody rb;

    [Tooltip("The density of the material, used for mass calculation.")]
    public float materialDensity = 7850f; // e.g., kg/m³ for steel

    [Header("Output")]
    [Tooltip("The calculated Center of Mass in the Rigidbody's local space. This is a read-only value.")]
    public Vector3 calculatedCoM_Local;


    // This creates a button in the Inspector. Click it to run the calculation.
    [ContextMenu("Calculate Center of Mass")]
    public void Calculate()
    {
        if (!rb)
        {
            Debug.LogError("Rigidbody is not assigned! Please drag it into the 'rb' field in the Inspector.");
            return;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found on this object or its children.");
            return;
        }

        double totalMass = 0;
        Vector3 weightedPositionSum = Vector3.zero;

        foreach (var c in colliders)
        {
            if (c.isTrigger) continue;

            double volume = 0;
            Vector3 worldCenter = c.bounds.center;

            if (c is BoxCollider box)
            {
                Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);
                volume = size.x * size.y * size.z;
                worldCenter = box.transform.TransformPoint(box.center);
            }
            else if (c is SphereCollider sphere)
            {
                float radius = sphere.radius * GetMaxAbsScale(sphere.transform.lossyScale);
                volume = (4.0 / 3.0) * Mathf.PI * Mathf.Pow(radius, 3);
                worldCenter = sphere.transform.TransformPoint(sphere.center);
            }
            else if (c is CapsuleCollider capsule)
            {
                float radius = capsule.radius * GetMaxAbsScale(capsule.transform.lossyScale);
                float height = capsule.height * Mathf.Abs(capsule.transform.lossyScale.y);
                double cylinderVolume = Mathf.PI * radius * radius * (height - 2 * radius);
                double sphereVolume = (4.0 / 3.0) * Mathf.PI * Mathf.Pow(radius, 3);
                volume = cylinderVolume + sphereVolume;
                worldCenter = capsule.transform.TransformPoint(capsule.center);
            }
            else if (c is MeshCollider)
            {
                // This is a rough approximation for MeshColliders
                Vector3 size = c.bounds.size;
                volume = size.x * size.y * size.z;
            }

            double mass = volume * materialDensity;
            totalMass += mass;
            weightedPositionSum += worldCenter * (float)mass;
        }

        if (totalMass <= 0) return;

        // Calculate the final CoM in world space
        Vector3 worldCoM = weightedPositionSum / (float)totalMass;

        // Convert the world space CoM to the Rigidbody's local space
        calculatedCoM_Local = rb.transform.InverseTransformPoint(worldCoM);

        Debug.Log("Calculation complete. Calculated Local CoM is: " + calculatedCoM_Local);
    }

    private float GetMaxAbsScale(Vector3 scale)
    {
        return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
    }
}