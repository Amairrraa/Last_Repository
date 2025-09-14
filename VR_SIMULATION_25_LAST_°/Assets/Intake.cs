using System.Collections;
using UnityEngine;

public class CollectorManager : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] pathPoints;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Ball Detection Settings")]
    public Collider intakeTrigger;  // Assign in Inspector

    [Header("Controls")]
    public KeyCode intakeKey = KeyCode.E;

    private bool intakeActive = false;

    private void OnEnable()
    {
        if (intakeTrigger != null)
        {
            intakeTrigger.isTrigger = true; // make sure it’s a trigger
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(intakeKey))
        {
            intakeActive = !intakeActive;
            Debug.Log("Intake " + (intakeActive ? "ON" : "OFF"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!intakeActive) return;

        if (other.CompareTag("BiodiversityUnit"))
        {
            Debug.Log("Ball detected: " + other.name);
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true; // 🔴 Disable physics
                StartCoroutine(MoveBallAlongPath(other.gameObject, rb));
            }
        }
    }

    private IEnumerator MoveBallAlongPath(GameObject ball, Rigidbody rb)
    {
        foreach (Transform point in pathPoints)
        {
            while (Vector3.Distance(ball.transform.position, point.position) > 0.05f)
            {
                ball.transform.position = Vector3.MoveTowards(
                    ball.transform.position,
                    point.position,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        // ✅ Restore physics at the end
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
