using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : MonoBehaviour, InterfaceMethod.TableData
{
    public int INDEX { get; set; }
    FairyType fairyType;
    public DataCenter gamedata = new DataCenter(100, 0);
    List<Item> _items;

    void Start()
    {
        Manager.Instance.observer.UserPlayer = this;
    }

    public void AddItem(Item InitItem)
    {
        _items.Add(InitItem);
    }


}
