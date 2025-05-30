using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<InterfaceMethod.IItemData> userItemDatas = new List<InterfaceMethod.IItemData>()
    {
        new Potion()
        
    };

    public List<SpaceShipData> spaceShipDatas = new List<SpaceShipData>();
}
