using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CONT : MonoBehaviour
{
    //public float rotationSpeed = 100f;
    public float Speed = 10f;
    Rigidbody RB;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {


           //transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
        //transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Speed, this.transform.position.z);
        RB.AddForce(0f,  Speed, 0f);


    }
}
