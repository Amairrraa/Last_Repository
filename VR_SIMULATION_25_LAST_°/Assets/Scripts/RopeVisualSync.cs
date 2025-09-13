using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisualSync : MonoBehaviour
{
    public Transform[] bones;            // The bones of the rigged rope
    public Transform[] ropeSegments;     // Physics segment transforms

    void LateUpdate()
    {
        for (int i = 0; i < bones.Length && i < ropeSegments.Length; i++)
        {
            bones[i].position = ropeSegments[i].position;
            bones[i].rotation = ropeSegments[i].rotation;
        }
    }
}

