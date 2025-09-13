using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeOptimizer : MonoBehaviour
{
    public bool isneartherope;
    public GameObject rope;
    public GameObject hiddencollider;
    public Rigidbody rb1;
    public Rigidbody rb2;

    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rope.SetActive(true);
            hiddencollider.SetActive(false);
            Rigidbody[] rigidbodies = rope.GetComponentsInChildren<Rigidbody>();
            isneartherope = true;

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            rb1.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
             rb2.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isneartherope = false;
        hiddencollider.SetActive(true);
        // Rigidbody[] rigidbodies = rope.GetComponentsInChildren<Rigidbody>();
        // foreach (Rigidbody rb in rigidbodies)
        // {
              rb1.collisionDetectionMode = CollisionDetectionMode.Discrete;
             rb2.collisionDetectionMode = CollisionDetectionMode.Discrete;
        //}

    }
    
}