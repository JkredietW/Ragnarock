using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCraft : MonoBehaviour
{
    public List<Result> craft;
    [Space]
    [SerializeField] GameObject contentHolder;
    [SerializeField] Inventory inventory;
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
            if (inventory.itemSlots[i].item != null)
            {
                itemsInInventory.Add(inventory.itemSlots[i].item);
            }
        }
        if(itemsInInventory.Count > 0)
        {
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
    public void Craft()
    {
        craftThisSprite.gameObject.SetActive(false);
        //remove needed items
        itemSlotForCraft.Clear();
        itemsNeedForCraft.Clear();
        if (selectedCraft.craftAmount > 0)
        {
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
                if (itemSlotForCraft.Count != selectedCraft.itemsNeeded.Count)
                {
                    return;
                }
                FinishCrafting();
            }
        }
    }
    void FinishCrafting()
    {
        for (int i = 0; i < itemSlotForCraft.Count; i++)
        {
            inventory.itemSlots[itemSlotForCraft[i]].item.itemAmount -= itemsNeedForCraft[i];
        }
        inventory.RefreshUI();
        GetComponent<CharacterStats>().CreateItem(ItemList.SelectItem(selectedCraft.craftResult).name, 1, ItemList.SelectItem(selectedCraft.craftResult).sprite, ItemList.SelectItem(selectedCraft.craftResult).type, ItemList.SelectItem(selectedCraft.craftResult).maxStackSize);
        inventory.RefreshUI();
        selectedCraft = default;
        CanCraft();
    }
}
