using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemName
{
    Potion, end
}

public enum ItemType
{

}

public class ItemData : InterfaceMethod.TableData
{
    public int INDEX { get; set; }
    public ItemName type;
    float _itemRange;
    int _itemCount;
    int _itemCost;

    public float ItemRange
    {
        get { return _itemRange; }
        set { _itemRange = value; }
    }

    public int ItemCount
    {
        get { return _itemCount; }
        set { _itemCount = value; }
    }

    public int ItemCost
    {
        get { return _itemCost; }
        set { _itemCost = value; }
    }
}
