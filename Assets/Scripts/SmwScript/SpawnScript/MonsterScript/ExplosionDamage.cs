//폭발 데미지
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviourPunCallbacks
{
    int damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            var otherPV = other.GetComponent<PhotonView>();
            if (otherPV != null && otherPV.Owner != null)
            {
                // 데미지 전용 RPC
                photonView.RPC("HitPlayerRPC", otherPV.Owner, damage + 1);
            }
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
        }
    }

    [PunRPC]
    void HitPlayerRPC(int dmg)
    {
        Manager.Instance.observer.HitPlayer(dmg);
    }
}
