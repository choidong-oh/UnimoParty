using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipData : InterfaceMethod.TableData
{ 
    int _index;
    string _name;
    int _inventorySlotCount;
    float _shipMoveSpeed;
    string _shipModelName;
    int _shipCost;
    public bool _isbuy;
    List<ItemData> shipInventory = new List<ItemData>();

    public int INDEX
    {
        get { return _index; }
        set { _index = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    
    public int InventorySlotCount
    {
        get { return _inventorySlotCount; }
        set { _inventorySlotCount = value; }
    }

    public float ShipMoveSpeed
    {
        get { return _shipMoveSpeed; }
        set { _shipMoveSpeed = value; }
    }

    public string ShipModelName
    {
        get { return _shipModelName; }
        set { _shipModelName = value; }
    }

    public int ShipCost
    {
        get { return _shipCost; }
        set { _shipCost = value; }
    }
}
