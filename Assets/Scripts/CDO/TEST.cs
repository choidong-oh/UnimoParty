using Photon.Pun;
using UnityEngine;

public class TEST : MonoBehaviourPunCallbacks, IItemUse
{
    public void Use(Transform firepos, int power)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        transform.parent = null;
        rb.useGravity = true;
        Collider cd = gameObject.GetComponent<CapsuleCollider>();
        cd.isTrigger = false;
    }
}


