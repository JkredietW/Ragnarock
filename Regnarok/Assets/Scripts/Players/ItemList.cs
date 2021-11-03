using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public struct ItemContent
{
    public string name;
    public GameObject worldItem;
    [Space]
    public Sprite sprite;
    public EquipmentType type;
    public int maxStackSize;
    [Space]
    [Range(1, 100)]
    public int baseDamage;
    [Range(0.5f, 5f)]
    public float baseAttackSpeed;
    [Range(5, 100)]
    public int baseCritChance;
    [Range(0, 100)]
    public int foodHealAmount;
    [Range(0, 600)]
    public int smeltTime;
    public string smeltResult;
    public GameObject summonObject;
}

public class ItemList : MonoBehaviour
{
    public ItemContent[] itemContents;
    public static List<ItemContent> staticItemContents;

    private void Awake()
    {
        staticItemContents = new List<ItemContent>(itemContents);
    }

    public static ItemContent SelectItem(string itemName)
    {
        for (int i = 0; i < staticItemContents.Count; i++)
        {
            if(staticItemContents[i].name == itemName)
            {
                return staticItemContents[i];
            }
        }
        Debug.LogError("Specified item not found!: " + itemName);
        return default;
    }
}
