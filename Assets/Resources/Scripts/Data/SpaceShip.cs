using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public GameObject Model;
    public SpaceShipData SpaceShipData = new SpaceShipData();
    public List<GameObject> ItemSlot;

    private void Start()
    {
        EnableItemSlot();
    }

    void EnableItemSlot()
    {
        for(int i = 0; i < SpaceShipData.InventorySlotCount; i++)
        {
            ItemSlot[i].gameObject.SetActive(true);
        }
    }

}
