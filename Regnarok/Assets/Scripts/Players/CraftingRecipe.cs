using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public Result craft;

    public string resultName;
    public Sprite resultSprite;
    public int resultAmount;

    [Space]

    public string resourseName1;
    public string resourseName2;
    public Sprite resourceSprite1, resourceSprite2;
    public int resourceAmount1, resourceAmount2;

    [HideInInspector] public EquipmentType typeOfResult;

    public void SetUp(Result crafty)
    {
        craft = crafty;
        typeOfResult = ItemList.SelectItem(craft.craftResult).type;
        ItemContent content = ItemList.SelectItem(craft.craftResult);

        //result
        resultName = content.name;
        resultSprite = content.sprite;
        resultAmount = craft.craftAmount; //for now 1 craft at the time

        content = ItemList.SelectItem(craft.itemsNeeded[0].itemNeeded);
        //needed 1
        resourseName1 = content.name;
        resourceSprite1 = content.sprite;
        resourceAmount1 = craft.itemsNeeded[0].amountNeeded;

        if (craft.itemsNeeded.Count > 1)
        {
            content = ItemList.SelectItem(craft.itemsNeeded[1].itemNeeded);

            resourseName2 = content.name;
            resourceSprite2 = content.sprite;
            resourceAmount2 = craft.itemsNeeded[1].amountNeeded;
        }
    }
}
