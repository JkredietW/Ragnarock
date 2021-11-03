using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviour
{
    public int slotNumber;
    public GameObject item;
    public int slotType = 0;

    [SerializeField] Inventory inv;
    //[SerializeField] ChestInventory chestInv;

    public void GiveSlotnumber()
    {
        if(inv != null)
        {
            //inv.GiveMouseLocationForInventory(slotNumber);
        }
        else
        {
            //chestInv.GiveMouseLocationForInventory(slotNumber);
        }
    }
    public void GiveItemToSlot()
    {
        if (inv != null)
        {
            //inv.itemBeingDragged = false;
            //inv.AddItemToInventoryList(-1, -1, true, slotNumber);
        }
        else
        {
            //chestInv.AddItemToInventoryList(-1, -1, true, slotNumber);
        }
    }
}
