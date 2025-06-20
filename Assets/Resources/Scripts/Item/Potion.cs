using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour , InterfaceMethod.IItemData
{
    ItemData _itemdata;

    public ItemData ItemData 
    { 
        get
        { return _itemdata;}
        set
        {_itemdata = value;}
    }


}
