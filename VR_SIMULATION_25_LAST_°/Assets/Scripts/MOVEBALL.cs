using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOVEBALL : MonoBehaviour
{
    public float sbeed;
    public float rotate;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector3(0f, 5f * sbeed, 0f); // Move right at 5 units/second
    }
}
