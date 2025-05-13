using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : MonoBehaviour, InterfaceMethod.TableData
{
    public int INDEX { get; set; }
    FairyType fairyType;
    public DataCenter gamedata = new DataCenter(100, 0 );


    void Start()
    {
        Manager.Instance.observer.UserPlayer = this;
    }

}
