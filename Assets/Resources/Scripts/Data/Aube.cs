using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aube : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            Debug.Log("콜라이더1");
            if (collision.gameObject.TryGetComponent<HandHarvest>(out HandHarvest player))
            {
                Debug.Log("콜라이더2");
                //Manager.Instance.observer.UserPlayer.gamedata.playerFairyType += player.DeliveryAube();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {

    }

}
