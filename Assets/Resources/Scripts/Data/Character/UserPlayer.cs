using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : MonoBehaviour
{
    public FairyType fairyType;
    public DataCenter gamedata = new DataCenter(100,0,5000,PlayerState.None);
    SpaceShipData SpaceShip;

    void Start()
    {
        Manager.Instance.observer.UserPlayer = this;
    }
}
