using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoForward : MonoBehaviour
{
    public float moveForce = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * moveForce);
    }
}
