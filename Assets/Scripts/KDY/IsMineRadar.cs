using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IsMineRadar : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject mylocatableObject;
    void Start()
    {
        if (photonView.IsMine)
        {
            mylocatableObject.SetActive(false);
        }
    }
}
