using System.Collections.Generic;
using UnityEngine;

public class CollectorManager : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] controlPoints;
    [SerializeField] private float PathDuration = 3f;
    [SerializeField] private bool PathFaceForward = true;

    [Header("Collector Settings")]
    [SerializeField] private GameObject[] collectorParts;
    [SerializeField] private Vector3 collectorRotationAxis = Vector3.up;
    [SerializeField] private float collectorRotationSpeed = 50f;
    [SerializeField] private bool automaticMode = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.C;

    [Header("Detection Settings")]
    [SerializeField] private Collider detectionTrigger;

    [Header("Drop / Safety")]
    [Tooltip("Safe vertical offset above last path point to prevent overlap.")]
    [SerializeField] private float finalDropOffset = 0.2f; // 20 cm

    [Header("Debugging")]
    [SerializeField] private bool debugLogs = true;

    private bool isCollectorSpinning = false;

    private class Carried
    {
        public BiodiversityUnitManager manager;
        public Rigidbody rb;
        public Collider[] colliders;
        public bool[] originalIsTrigger;
        public float pathTime;
    }

    private readonly List<Carried> carriedBalls = new List<Carried>();

    private void Start()
    {
        if (controlPoints == null || controlPoints.Length < 2)
            Debug.LogError("CollectorManager: Please assign at least 2 control points!");

        if (detectionTrigger == null)
            Debug.LogError("CollectorManager: Please assign a trigger collider in Detection Settings!");
        else
        {
            if (!detectionTrigger.isTrigger)
            {
                Debug.LogWarning("CollectorManager: detectionTrigger is not set as trigger — forcing it to trigger.");
                detectionTrigger.isTrigger = true;
            }

            var forwarder = detectionTrigger.GetComponent<TriggerForwarder>();
            if (forwarder == null) forwarder = detectionTrigger.gameObject.AddComponent<TriggerForwarder>();
            forwarder.manager = this;
        }

        if (automaticMode) isCollectorSpinning = true;
    }

    private void Update()
    {
        HandleCollectorRotation();
    }

    private void FixedUpdate()
    {
        for (int i = carriedBalls.Count - 1; i >= 0; i--)
        {
            var c = carriedBalls[i];
            if (c == null || c.manager == null || c.rb == null)
            {
                carriedBalls.RemoveAt(i);
                continue;
            }

            c.pathTime += Time.fixedDeltaTime / Mathf.Max(0.0001f, PathDuration);
            float t = Mathf.Clamp01(c.pathTime);

            Vector3 pos = GetCatmullRomClamped(t);
            c.rb.MovePosition(pos);

            if (PathFaceForward)
            {
                float lookAhead = 0.01f;
                float t2 = Mathf.Min(t + lookAhead, 1f);
                Vector3 pos2 = GetCatmullRomClamped(t2);
                Vector3 dir = (pos2 - pos).normalized;
                if (dir != Vector3.zero) c.rb.MoveRotation(Quaternion.LookRotation(dir));
            }

            if (t >= 1f)
            {
                carriedBalls.RemoveAt(i);
                RestorePhysics(c);
            }
        }
    }

    #region Rotation
    private void HandleCollectorRotation()
    {
        if (collectorParts == null || collectorParts.Length == 0) return;

        if (automaticMode)
        {
            foreach (var part in collectorParts)
                if (part != null) part.transform.Rotate(collectorRotationAxis * collectorRotationSpeed * Time.deltaTime, Space.Self);
            isCollectorSpinning = true;
        }
        else
        {
            if (Input.GetKeyDown(toggleKey)) isCollectorSpinning = !isCollectorSpinning;
            if (isCollectorSpinning)
                foreach (var part in collectorParts)
                    if (part != null) part.transform.Rotate(collectorRotationAxis * collectorRotationSpeed * Time.deltaTime, Space.Self);
        }
    }
    #endregion

    #region Detection
    public void HandleDetectionEnter(Collider other)
    {
        if (!isCollectorSpinning) return;
        if (!other.CompareTag("BiodiversityUnit")) return;

        var unitManager = other.GetComponent<BiodiversityUnitManager>() ?? other.GetComponentInParent<BiodiversityUnitManager>();
        if (unitManager == null) return;

        StartCarry(unitManager);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleDetectionEnter(other);
    }
    #endregion

    #region Carry logic
    private void StartCarry(BiodiversityUnitManager unit)
    {
        if (unit == null || controlPoints.Length < 2) return;

        Rigidbody rb = unit.GetComponent<Rigidbody>();
        if (rb == null) return;

        Collider[] cols = unit.GetComponentsInChildren<Collider>();
        bool[] original = new bool[cols.Length];
        for (int i = 0; i < cols.Length; i++)
        {
            original[i] = cols[i].isTrigger;
            cols[i].isTrigger = true;
        }

        var c = new Carried
        {
            manager = unit,
            rb = rb,
            colliders = cols,
            originalIsTrigger = original,
            pathTime = 0f
        };

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        carriedBalls.Add(c);

        if (debugLogs) Debug.Log($"CollectorManager: Started carrying '{unit.name}'.");
    }

    private void RestorePhysics(Carried c)
    {
        if (c == null || c.rb == null) return;

        // Move slightly above last path point to avoid overlapping colliders
        Vector3 finalPos = GetCatmullRomClamped(1f) + Vector3.up * finalDropOffset;
        c.rb.position = finalPos;

        // Restore colliders
        if (c.colliders != null && c.originalIsTrigger != null)
        {
            for (int i = 0; i < c.colliders.Length; i++)
                if (c.colliders[i] != null)
                    c.colliders[i].isTrigger = c.originalIsTrigger[i];
        }

        // Restore Rigidbody
        c.rb.isKinematic = false;
        c.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        c.rb.velocity = Vector3.zero;
        c.rb.angularVelocity = Vector3.zero;
        c.rb.WakeUp();

        if (debugLogs) Debug.Log($"CollectorManager: Ball '{c.manager.name}' physics restored.");
    }
    #endregion

    #region Catmull-Rom spline
    private Vector3 GetCatmullRomClamped(float tNorm)
    {
        int count = controlPoints.Length;
        if (count == 2) return Vector3.Lerp(controlPoints[0].position, controlPoints[1].position, tNorm);

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
    #endregion
}

public class TriggerForwarder : MonoBehaviour
{
    [HideInInspector] public CollectorManager manager;
    private void OnTriggerEnter(Collider other)
    {
        manager?.HandleDetectionEnter(other);
    }
}
