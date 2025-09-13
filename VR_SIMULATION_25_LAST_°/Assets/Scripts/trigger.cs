using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour
{
    public bool start = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BALL")
        {
            start = true;
        }
    }
}
