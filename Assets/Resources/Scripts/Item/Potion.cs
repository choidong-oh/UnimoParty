using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    ItemData _itemdata = new ItemData();

    private void Start()
    {
        InitPostionData();
    }

    void InitPostionData()
    {
        _itemdata = (ItemData)Manager.Instance.dataLoader.data["ItemData"][0];
    }
}
