using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMove : MonoBehaviour
{
    [SerializeField]
    Transform player;
    

    void Update()
    {
        gameObject.transform.position = player.position;
    }
}
