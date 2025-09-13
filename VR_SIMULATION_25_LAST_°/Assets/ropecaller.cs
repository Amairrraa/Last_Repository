using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropecaller : MonoBehaviour
{
    public GameObject r1;
    public GameObject r2;
    public GameObject r3;
    public GameObject r4;
    public GameObject r5;
    public GameObject r6;
    // Start is called before the first frame update
     private void Awake() {
        r1.SetActive(true);
        r2.SetActive(true);
        r3.SetActive(true);
        r4.SetActive(true);
        r5.SetActive(true);
        r6.SetActive(true);
        
    }
}

   
