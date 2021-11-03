using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public struct Result
{
    public string craftResult;
    public int craftAmount;
    public List<ItemAmount> itemsNeeded;
}
[Serializable]
public struct ItemAmount
{
    public string itemNeeded;
    [Range(1, 999)]
    public int amountNeeded;
}

public class CraftingStation : MonoBehaviour
{
    public List<Result> craft;
    [Space]
    [SerializeField] GameObject uipanel, contentHolder;
    CharacterStats character;
    Inventory inventory;
    [SerializeField] List<Item> itemsInInventory;
    [SerializeField] List<int> itemSlotForCraft, itemsNeedForCraft;
    Result craftThis;
    Result selectedCraft;

    public Image craftThisSprite;

    public List<RecipeHolder> slots;

    void GetSlots()
    {
        slots.Clear();
        if (contentHolder != null)
        {
            foreach (Transform item in contentHolder.transform)
            {
                slots.Add(item.GetComponent<RecipeHolder>());
            }
        }
        else
        {
            slots.Clear();
        }
        foreach (RecipeHolder holder in slots)
        {
            if (holder.recipe == null)
            {
                holder.gameObject.SetActive(false);
            }
            else
            {
                holder.gameObject.SetActive(true);
                holder.UpdateUi();
            }
        }
    }

    public CraftingRecipe CreateRecipe()
    {
        CraftingRecipe newRecipe = ScriptableObject.CreateInstance<CraftingRecipe>();
        newRecipe.SetUp(craftThis);
        return newRecipe;
    }

    private void Start()
    {
        itemsInInventory = new List<Item>();
        itemsNeedForCraft = new List<int>();
        itemSlotForCraft = new List<int>();
        craftThisSprite.gameObject.SetActive(false);
    }

    public void SelectRecipe(RecipeHolder i)
    {
        selectedCraft = i.recipe.craft;
        craftThisSprite.gameObject.SetActive(true);
        craftThisSprite.sprite = ItemList.SelectItem(selectedCraft.craftResult).sprite;
    }
    public void CanCraft()
    {
        //add into list for further use
        itemsInInventory.Clear();
        GetSlots();
        for (int i = 0; i < inventory.itemSlots.Length; i++)
        {
            if(inventory.itemSlots[i].item != null)
            {
                itemsInInventory.Add(inventory.itemSlots[i].item);
            }
        }
        for (int i = 0; i < craft.Count; i++)
        {
            bool gotItem = false;
            for (int z = 0; z < itemsInInventory.Count; z++)
            {
                for (int u = 0; u < craft[i].itemsNeeded.Count; u++)
                {
                    if (!gotItem)
                    {
                        if (itemsInInventory[z].itemName == craft[i].itemsNeeded[u].itemNeeded)
                        {
                            craftThis = craft[i];

                            slots[i].recipe = CreateRecipe();
                            gotItem = true;
                        }
                        else
                        {
                            slots[i].recipe = null;
                        }
                    }
                }
            }
        }
        foreach (RecipeHolder holder in slots)
        {
            if(holder.recipe == null)
            {
                holder.gameObject.SetActive(false);
            }
            else
            {
                holder.gameObject.SetActive(true);
                holder.UpdateUi();
            }
        }
    }
    public void Craft()
    {
        itemsNeedForCraft.Clear();
        itemSlotForCraft.Clear();
        craftThisSprite.gameObject.SetActive(false);
        //remove needed items
        if (selectedCraft.craftResult.Length > 0)
        {
            for (int i = 0; i < inventory.itemSlots.Length; i++)
            {
                if (inventory.itemSlots[i].item == null)
                {
                    continue;
                }
                for (int y = 0; y < selectedCraft.itemsNeeded.Count; y++)
                {
                    string neededNameItem = selectedCraft.itemsNeeded[y].itemNeeded;
                    int neededAmountItem = selectedCraft.itemsNeeded[y].amountNeeded;

                    if (inventory.itemSlots[i].item.itemName != neededNameItem)
                    {
                        continue;
                    }
                    else if (inventory.itemSlots[i].item.itemAmount >= neededAmountItem)
                    {
                        itemsNeedForCraft.Add(neededAmountItem);
                        itemSlotForCraft.Add(i);
                        continue;
                    }
                    else
                    {
                        print("not enough items!: " + neededNameItem + " " + neededAmountItem);
                        continue;
                    }
                }
            }
            FinishCrafting();
        }
    }
    void FinishCrafting()
    {
        for (int i = 0; i < itemSlotForCraft.Count; i++)
        {
            inventory.itemSlots[itemSlotForCraft[i]].item.itemAmount -= itemsNeedForCraft[i];
        }
        inventory.RefreshUI();
        character.CreateItem(ItemList.SelectItem(selectedCraft.craftResult).name, 1, ItemList.SelectItem(selectedCraft.craftResult).sprite, ItemList.SelectItem(selectedCraft.craftResult).type, ItemList.SelectItem(selectedCraft.craftResult).maxStackSize);
        inventory.RefreshUI();
        CanCraft();
    }

    public void OpenCratingInventory(CharacterStats charr, Inventory inv)
    {
        uipanel.gameObject.SetActive(true);
        character = charr;
        inventory = inv;

        CanCraft();
    }
    public void CloseChestInventory()
    {
        uipanel.gameObject.SetActive(false);
        character = null;
    }
    public void RecipeInfo(RecipeHolder _recipe)
    {
        character.RecipeInfo(_recipe);
    }
}
