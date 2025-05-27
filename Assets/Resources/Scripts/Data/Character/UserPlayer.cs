using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : MonoBehaviour
{
    FairyType fairyType;
    public DataCenter gamedata = new DataCenter(100, 0, PlayerState.None);

    void Start()
    {
        Manager.Instance.observer.UserPlayer = this;
    }

}
