using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<InterfaceMethod.IItemData> userItemDatas = new List<InterfaceMethod.IItemData>()
    {
        new Potion()
        
    };


    public List<SpaceShipData> spaceShipDatas = new List<SpaceShipData>()
    {

    };

    public Dictionary<int ,CharacterData> chracterDatas = new Dictionary<int, CharacterData>()
    {
        {600, new CharacterData() },
        {601, new CharacterData() },
        {602, new CharacterData() },
        {603, new CharacterData() },
        {604, new CharacterData() },
        {605, new CharacterData() },
    };

}
