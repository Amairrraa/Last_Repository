using UnityEngine;

public class MeasureModel : MonoBehaviour
{
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            Bounds bounds = mf.sharedMesh.bounds;

            // Bounds are in local space, so scale them
            Vector3 size = Vector3.Scale(bounds.size, transform.localScale);

            Debug.Log("Model size: " + size);
            Debug.Log("Width: " + size.x + " | Height: " + size.y + " | Depth: " + size.z);
        }
    }
}
