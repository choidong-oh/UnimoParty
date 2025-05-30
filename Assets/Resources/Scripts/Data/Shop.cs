using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    

    private void Start()
    {
        GetItemData();
    }

    void GetItemData()
    {
        var tempdatas = Manager.Instance.dataLoader.data;
    }

    void ShopItemList()
    {

    }

    void SelectItem()
    {

    }

}
