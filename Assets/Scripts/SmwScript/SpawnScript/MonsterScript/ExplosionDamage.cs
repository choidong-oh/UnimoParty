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
        if (other.CompareTag("Player"))
        {
            Manager.Instance.observer.HitPlayer(damage);
        }
    }
}
