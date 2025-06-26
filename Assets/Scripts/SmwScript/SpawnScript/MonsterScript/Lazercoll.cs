//레이콕 레이져 
using Photon.Pun;
using UnityEngine;

public class Lazercoll : MonoBehaviourPunCallbacks
{
    int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            damage = 1;
            Manager.Instance.observer.HitPlayer(damage);
        }

    }

}
