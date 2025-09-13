using UnityEngine;

public class RopeAttach : MonoBehaviour
{
    // This will be the tag of the object you want to fuse with
    public string targetTag = "AttachPoint";

    // Option: make rope child of the target
    public bool makeChild = true;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collided object has the right tag
        if (collision.gameObject.CompareTag(targetTag))
        {
            if (makeChild)
            {
                // Make rope child of target
                transform.SetParent(collision.transform);
                Debug.Log("Rope is now child of " + collision.gameObject.name);
            }
            else
            {
                // Freeze rope in place (fusion-like)
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true; // stops rope movement
                }
                Debug.Log("Rope fused with " + collision.gameObject.name);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Same behavior if you prefer Trigger instead of Collision
        if (other.CompareTag(targetTag))
        {
            if (makeChild)
            {
                transform.SetParent(other.transform);
                Debug.Log("Rope is now child of " + other.gameObject.name);
            }
            else
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                Debug.Log("Rope fused with " + other.gameObject.name);
            }
        }
    }
}
