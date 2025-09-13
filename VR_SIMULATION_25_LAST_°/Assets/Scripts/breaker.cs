using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breaker : MonoBehaviour
{public float reducefactor;
    public float timer;
    public RopeOptimizer rp;
   void Start()
        {
            StartCoroutine(DelayedAction());
        }
void FixedUpdate()
    {
        timer += Time.deltaTime;
        
    }
        IEnumerator DelayedAction()
        {
            
            yield return new WaitForSeconds(2); // Wait for 2 seconds
          
             Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.velocity *= reducefactor;
            rb.angularVelocity *=  0.2f;
        }
        yield return new WaitForSeconds(2);
        
        for (int i = 0; i <10; i++)
        {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.velocity *= reducefactor;
            rb.angularVelocity *= reducefactor;
        }
        yield return new WaitForSeconds(0.6f);
        }
        if (rp.isneartherope == false)
        {
            gameObject.SetActive(false);
        }

 
        }

}
