using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ItemPickUp : MonoBehaviour
{
    public int itemSerialNumber;
    [SerializeField] Vector3 dropOffset;
    [SerializeField] List<StructDropItemsList> dropItems;

    bool justOnce;
    [Serializable]
    public struct StructDropItemsList
    {
        public string dropItemName;
        public Vector2 dropAmounts;
    }

    //destory
    public virtual void DropItems()
    {
        if(justOnce)
        {
            return;
        }
        justOnce = true;
        for (int i = 0; i < dropItems.Count; i++)
        {
            FindObjectOfType<GameManager>().DropItems(dropItems[i].dropItemName, transform.position + dropOffset, Quaternion.identity, UnityEngine.Random.Range((int)dropItems[i].dropAmounts.x, (int)dropItems[i].dropAmounts.y), itemSerialNumber);
        }
    }
}
