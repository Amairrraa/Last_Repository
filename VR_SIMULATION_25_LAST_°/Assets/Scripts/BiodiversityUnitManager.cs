using UnityEngine;

public class BiodiversityUnitManager : MonoBehaviour
{
    [SerializeField] private Rigidbody Rb;
    private bool IsGhost;

    public void Init(Vector3 velocity, bool isGhost)
    {
        IsGhost = isGhost;
        Rb.velocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;
        Rb.AddForce(velocity, ForceMode.Impulse);
        Debug.Log((isGhost ? "[GHOST]" : "[REAL]") + " Ball initialized with velocity: " + velocity);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (IsGhost) return;
        Debug.Log("Ball collided with " + col.gameObject.name);
    }

    private void OnDestroy()
    {
        Debug.LogWarning((IsGhost ? "[GHOST]" : "[REAL]") + " Ball destroyed: " + name);
    }
}
