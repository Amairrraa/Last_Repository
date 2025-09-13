using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeLineRenderer : MonoBehaviour
{
    public Transform[] ropeSegments;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (ropeSegments.Length > 0)
            line.positionCount = ropeSegments.Length;
    }

    void Update()
    {
        for (int i = 0; i < ropeSegments.Length; i++)
        {
            line.SetPosition(i, ropeSegments[i].position);
        }
    }
}
