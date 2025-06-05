using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class TEST : MonoBehaviourPunCallbacks
{
    public Transform firepos;
    [SerializeField] Transform rightController;
    [SerializeField] int grenadePower = 1;

    GameObject grenade;

    private void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(wait());
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        //SearchFirepos();
        //StartCoroutine(dsds());
    }

  
    void ThrowGrenade1()
    {
        grenade = PhotonNetwork.Instantiate("Boomprefab", firepos.position, firepos.rotation);


        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Vector3 throwDirection = firepos.transform.forward+ firepos.transform.up;
        rb.AddForce(throwDirection* grenadePower, ForceMode.VelocityChange);
    }

    IEnumerator dsds()
    {
        grenade = PhotonNetwork.Instantiate("Boomprefab", firepos.position, firepos.rotation);
        yield return new WaitUntil(() => grenade != null);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Vector3 throwDirection = firepos.transform.forward + firepos.transform.up;
        rb.AddForce(throwDirection * grenadePower, ForceMode.VelocityChange);
    }



    void SearchFirepos()
    {
        var modelPrefab= rightController.gameObject.GetComponentInChildren<ActionBasedController>().modelPrefab;

        firepos = modelPrefab.GetChild(0).transform;

    }

  

}


