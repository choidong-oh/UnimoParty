using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    Rigidbody rb;


   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            rb = GetComponent<Rigidbody>();
            Debug.Log("sds");
            rb.isKinematic = true;
        }

    }


}
