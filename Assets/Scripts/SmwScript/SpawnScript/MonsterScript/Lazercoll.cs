using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazercoll : MonoBehaviour
{
    int damage; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            damage = 1;
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
        }

    }

}
