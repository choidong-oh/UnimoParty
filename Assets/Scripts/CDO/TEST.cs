using UnityEngine;


public class TEST : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform firePoint;
    Rigidbody rb;
    Vector3 throwDirection;
    [SerializeField] int grenadePower = 10;

    private void Start()
    {
        ThrowGrenade1();
    }

    void ThrowGrenade1()
    {
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        rb = grenade.GetComponent<Rigidbody>();
        throwDirection = firePoint.transform.forward;
        rb.AddForce(throwDirection* grenadePower, ForceMode.VelocityChange);
    }

  

}


