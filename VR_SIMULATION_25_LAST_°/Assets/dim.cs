using UnityEngine;

public class ShowBounds : MonoBehaviour
{
    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log(name + " size: " + renderer.bounds.size);
        }
    }
}
