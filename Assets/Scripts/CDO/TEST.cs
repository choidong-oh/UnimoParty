using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TEST : MonoBehaviour
{
    [SerializeField] GameObject Boomprefab;
    [SerializeField] Transform TestBoomTrans;


    //[ContextMenu("¾óÀ½")]

    private void Start()
    {
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.Instantiate("Boomprefab", TestBoomTrans.position,Quaternion.identity);
    }







    
}


