using UnityEngine;


public class TEST : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform firePoint;


    private void Start()
    {
        ThrowGrenade1();
    }

    Rigidbody rb;
    Vector3 throwDirection;
    void ThrowGrenade1()
    {
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        rb = grenade.GetComponent<Rigidbody>();
        throwDirection = firePoint.transform.forward;
        rb.AddForce(throwDirection * 10,ForceMode.VelocityChange);
    }






}


