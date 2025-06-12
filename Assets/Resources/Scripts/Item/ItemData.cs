using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemName
{
    Potion, FreezeBoom, Barricade, end
}

public class ItemData : InterfaceMethod.TableData
{
    public int INDEX { get; set; }
    public string _name;
    public int _itemProbability_top;
    public int _itemProbability_mid;
    public int _itemProbability_bot;
    public int _itemConTime;
    public int _itemHeal;
    public int _itemShootSpeed;
    public float _itemExplRange;
    public int _itemMaxRange;
    public int _itemIceTime;
    public int _itemAlarmTime;
    public ItemName type;
    float _itemRange;
    int _itemCount;
    int _itemCost;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int ItemProbabilityTop
    {
        get { return _itemProbability_top; }
        set { _itemProbability_top = value; }
    }

    public int ItemProbabilityMid
    {
        get { return _itemProbability_mid; }
        set { _itemProbability_mid = value; }
    }

    public int ItemProbabilityBot
    {
        get { return _itemProbability_bot; }
        set { _itemProbability_bot = value; }
    }

    public int ItemConTime
    {
        get { return _itemConTime; }
        set { _itemConTime = value; }
    }

    public int ItemHeal
    {
        get { return _itemHeal; }
        set { _itemHeal = value; }
    }

    public int ItemShootSpeed
    {
        get { return _itemShootSpeed; }
        set { _itemShootSpeed = value; }
    }

    public float ItemExplRange
    {
        get { return _itemExplRange; }
        set { _itemExplRange = value; }
    }

    public int ItemMaxRange
    {
        get { return _itemMaxRange; }
        set { _itemMaxRange = value; }
    }

    public int ItemIceTime
    {
        get { return _itemIceTime; }
        set { _itemIceTime = value; }
    }

    public int ItemAlarmTime
    {
        get { return _itemAlarmTime; }
        set { _itemAlarmTime = value; }
    }

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
