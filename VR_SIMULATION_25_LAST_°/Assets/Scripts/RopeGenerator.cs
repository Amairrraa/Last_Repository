using System.Collections.Generic; // At the top
using System.Collections;
using UnityEngine;
public class RopeGenerator : MonoBehaviour
{
    [Header("Line Smoothing")]
    [Range(2, 100)]
    public int smoothingResolution = 10; // Points between each rope segment

    [Header("Rope Settings")]
    public int segmentCount = 20;
    public float segmentLength = 0.2f;
    public float segmentRadius = 0.05f;
    public Material ropeMaterial;

    [Header("Physics Settings")]
    public float ropeMass = 0.1f;
    public bool useConfigurableJoint = true;

    [Header("Visual Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    private List<Transform> segmentTransforms = new List<Transform>();
    private LineRenderer lineRenderer;

    void Start()
    {
        GenerateRope();
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.positionCount = segmentTransforms.Count;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        lineRenderer.receiveShadows = true;
    }

    void Update()
    {
        if (lineRenderer && segmentTransforms.Count > 1)
        {
            List<Vector3> smoothedPoints = GetSmoothedRopePoints(segmentTransforms, smoothingResolution);
            lineRenderer.positionCount = smoothedPoints.Count;
            lineRenderer.SetPositions(smoothedPoints.ToArray());
        }
    }


    void GenerateRope()
    {
        Vector3 segmentDirection = -transform.up;
        Rigidbody previousRb = null;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            segment.name = "RopeSegment_" + i;
            segment.transform.localScale = new Vector3(segmentRadius, segmentLength / 2f, segmentRadius);
            segment.transform.position = transform.position + segmentDirection * segmentLength * i;
            segment.transform.rotation = Quaternion.identity;

            if (ropeMaterial)
                segment.GetComponent<Renderer>().material = ropeMaterial;

            Rigidbody rb = segment.AddComponent<Rigidbody>();
            rb.mass = ropeMass;
            rb.angularDrag = 0.05f;

            if (previousRb != null)
            {
                if (useConfigurableJoint)
                {
                    ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                    joint.connectedBody = previousRb;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.anchor = new Vector3(0, 0.5f, 0);
                    joint.connectedAnchor = new Vector3(0, -0.5f, 0);
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;
                }
                else
                {
                    CharacterJoint joint = segment.AddComponent<CharacterJoint>();
                    joint.connectedBody = previousRb;
                }
            }
            else
            {
                rb.isKinematic = true; // Anchor top
            }

            segment.layer = LayerMask.NameToLayer("Rope");
            previousRb = rb;

            segmentTransforms.Add(segment.transform); // ?? store transform
        }
    }
    List<Vector3> GetSmoothedRopePoints(List<Transform> points, int resolution)
    {
        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1].position : points[i].position;
            Vector3 p1 = points[i].position;
            Vector3 p2 = points[i + 1].position;
            Vector3 p3 = (i + 2 < points.Count) ? points[i + 2].position : p2;

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 newPos = CatmullRom(p0, p1, p2, p3, t);
                smoothPoints.Add(newPos);
            }
        }

        // Add final point
        smoothPoints.Add(points[points.Count - 1].position);

        return smoothPoints;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

}
