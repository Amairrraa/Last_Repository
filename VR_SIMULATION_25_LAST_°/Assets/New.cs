using UnityEngine;

public class CollectorManagerx : MonoBehaviour
{
    [Header("Path Settings")]
    [Tooltip("Control points for the spline. First = entry, last = drop point above storage.")]
    public Transform[] controlPoints;

    [SerializeField] private float PathDuration = 3f;
    [SerializeField] private bool PathFaceForward = true;

    [Header("Detection Settings")]
    [Tooltip("Prefab containing a trigger collider for ball detection")]
    [SerializeField] private GameObject DetectionColliderPrefab;

    [Header("Collector Settings")]
    [Tooltip("Independent rotating collector prefab")]
    [SerializeField] private GameObject CollectorPrefab;
    [SerializeField] private Vector3 collectorRotationAxis = Vector3.up;
    [SerializeField] private float collectorRotationSpeed = 50f;

    [Tooltip("If true, collector rotates automatically. If false, it needs button control.")]
    [SerializeField] private bool automaticMode = true;

    [Tooltip("Key used to toggle spinning in Control Mode.")]
    [SerializeField] private KeyCode toggleKey = KeyCode.C;

    // Internal state
    private GameObject collectorInstance;
    private bool isCollectorSpinning = false; // used in Control Mode

    // Path state
    private float pathTime;
    private bool isFollowingPath = false;
    private BiodiversityUnitManager movingBall;

    private void Start()
    {
        // Spawn detection collider prefab (if provided)
        if (DetectionColliderPrefab != null)
        {
            GameObject detector = Instantiate(DetectionColliderPrefab, transform);
            Collider c = detector.GetComponent<Collider>();
            if (c == null || !c.isTrigger)
                Debug.LogWarning("DetectionColliderPrefab should have a trigger collider!");
        }

        // Safety check for path
        if (controlPoints == null || controlPoints.Length < 2)
        {
            Debug.LogError("CollectorManager: Please assign at least 2 control points!");
        }

        // Spawn independent collector prefab
        if (CollectorPrefab != null)
        {
            collectorInstance = Instantiate(CollectorPrefab, transform);
        }

        // If automatic, it starts spinning right away
        if (automaticMode) isCollectorSpinning = true;
    }

    private void Update()
    {
        HandleCollectorRotation();
        HandlePathFollowing();
    }

    private void HandleCollectorRotation()
    {
        if (collectorInstance == null) return;

        if (automaticMode)
        {
            // Always spinning
            collectorInstance.transform.Rotate(collectorRotationAxis * collectorRotationSpeed * Time.deltaTime, Space.Self);
            isCollectorSpinning = true;
        }
        else
        {
            // Control Mode – toggle spin with button
            if (Input.GetKeyDown(toggleKey))
            {
                isCollectorSpinning = !isCollectorSpinning;
            }

            if (isCollectorSpinning)
            {
                collectorInstance.transform.Rotate(collectorRotationAxis * collectorRotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ? Do nothing if collector is not spinning
        if (!isCollectorSpinning) return;

        if (other.CompareTag("BiodiversityUnit"))
        {
            BiodiversityUnitManager ballManager = other.GetComponent<BiodiversityUnitManager>();
            if (ballManager == null) return;

            StartBallPath(ballManager);
        }
    }

    private void StartBallPath(BiodiversityUnitManager ball)
    {
        if (controlPoints == null || controlPoints.Length < 2) return;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // follow path first

        movingBall = ball;
        pathTime = 0f;
        isFollowingPath = true;
    }

    private void HandlePathFollowing()
    {
        if (!isFollowingPath || movingBall == null) return;

        pathTime += Time.deltaTime / Mathf.Max(0.0001f, PathDuration);
        float t = Mathf.Clamp01(pathTime);

        Vector3 pos = GetCatmullRomClamped(t);
        movingBall.transform.position = pos;

        if (PathFaceForward)
        {
            float lookAhead = 0.001f;
            float t2 = Mathf.Min(t + lookAhead, 1f);
            Vector3 pos2 = GetCatmullRomClamped(t2);
            Vector3 dir = (pos2 - pos).normalized;
            if (dir != Vector3.zero) movingBall.transform.rotation = Quaternion.LookRotation(dir);
        }

        if (t >= 1f)
        {
            FinishPath();
        }
    }

    private void FinishPath()
    {
        isFollowingPath = false;

        // Now the ball should act naturally with physics
        Rigidbody rb = movingBall.GetComponent<Rigidbody>();
        rb.isKinematic = false; // gravity + physics on

        movingBall = null;
    }

    // Catmull–Rom spline with clamped ends
    private Vector3 GetCatmullRomClamped(float tNorm)
    {
        int count = controlPoints.Length;
        if (count == 2)
        {
            return Vector3.Lerp(controlPoints[0].position, controlPoints[1].position, tNorm);
        }

        float totalSegments = count - 1;
        float tScaled = tNorm * totalSegments;
        int i = Mathf.FloorToInt(tScaled);
        if (i >= count - 1) i = count - 2;
        float u = tScaled - i;

        Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)].position;
        Vector3 p1 = controlPoints[i].position;
        Vector3 p2 = controlPoints[i + 1].position;
        Vector3 p3 = controlPoints[Mathf.Min(i + 2, count - 1)].position;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * u +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * (u * u) +
            (-p0 + 3f * p1 - 3f * p2 + p3) * (u * u * u)
        );
    }
}