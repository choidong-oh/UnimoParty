using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    int damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            Manager.Instance.observer.HitPlayer(damage + 1);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
        }
    }

    [PunRPC]
    void HitPlayerRPC(int dmg)
    {
        Manager.Instance.observer.HitPlayer(dmg);
    }
}
