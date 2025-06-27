using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BarricadeBox : MonoBehaviourPunCallbacks
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(wait());
        }



    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC("Explode", RpcTarget.All, true);

        yield return new WaitForSeconds(1);
        photonView.RPC("Explode", RpcTarget.All, false);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void Explode(bool isFreeze)
    {
        JoystickController PlayerMove;
        HeadDash headDash;
        if ((PlayerMove = GetComponentInChildren<JoystickController>()) != null)
        {
            PlayerMove.Freeze(isFreeze);
            Debug.Log("플레이어 움직임 어름");
        }

        if ((headDash = GetComponentInChildren<HeadDash>()) != null)
        {
            headDash.Freeze(isFreeze);
            Debug.Log("플레이어 대시 어름");
        }

        if (TryGetComponent<HandHarvest>(out HandHarvest PlayerHarvest))
        {
            PlayerHarvest.Freeze(isFreeze);

            Debug.Log("플레이어 채집 어름");
        }
    }





}
