using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class OvenStation : MonoBehaviour
{
    [SerializeField] GameObject uipanel;
    CharacterStats character;
    Inventory inventory;

    [SerializeField] ItemSlot smeltSlot, fuelSlot, finishedSlot;

    float fuelTime, smeltTime, smeltProgress;
    bool canSmelt;

    [SerializeField] Slider fuelSlider, progressSlider;

    public void OpenCratingInventory(CharacterStats charr, Inventory inv)
    {
        uipanel.gameObject.SetActive(true);
        character = charr;
        inventory = inv;
    }
    public void CloseChestInventory()
    {
        character = null;
        uipanel.gameObject.SetActive(false);
    }
    
    public void CheckSlots()
    {
        //check if can smelt
        if(smeltSlot.item == null)
        {
            canSmelt = false;
            progressSlider.value = 0;
            return;
        }
        if (finishedSlot.item != null)
        {
            if (finishedSlot.item.itemName != ItemList.SelectItem(smeltSlot.item.itemName).smeltResult)
            {
                canSmelt = false;
                return;
            }
        }
        //check for fuel
        if (fuelTime == 0)
        {
            if (!NeedFuel())
            {
                canSmelt = false;
                return; //stops here: no more fuel
            }
        }
        BeginSmelt();
    }
    void BeginSmelt()
    {
        UpdateSlots();
        canSmelt = true;
    }
    void FinishSmelt()
    {
        ItemContent meltedItem = ItemList.SelectItem(ItemList.SelectItem(smeltSlot.item.itemName).smeltResult);
        CreateItem(meltedItem.name, 1 + GetFinishedSlotAmount(), meltedItem.sprite, EquipmentType.none, meltedItem.maxStackSize);
        smeltSlot.item.itemAmount--;
        if (smeltSlot.item.itemAmount == 0)
        {
            smeltSlot.item = null;
        }
        CheckSlots();
        UpdateSlots();
        SincSlots(smeltSlot);
        SincSlots(fuelSlot);
        SincSlots(finishedSlot);
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type, maxStack, ItemList.SelectItem(name).foodHealAmount, ItemList.SelectItem(name).smeltTime, ItemList.SelectItem(name).summonObject);

        finishedSlot.item = newItem;
        UpdateSlots();
    }
    int GetFinishedSlotAmount()
    {
        if(finishedSlot.item != null)
        {
            return finishedSlot.item.itemAmount;
        }
        else
        {
            return 0;
        }
    }
    void UpdateSlots()
    {
        if(smeltSlot.item)
        {
            smeltSlot.stackAmountText.text = smeltSlot.item.itemAmount.ToString();
        }
        else
        {
            smeltSlot.stackAmountText.text = "";
        }
        if (fuelSlot.item)
        {
            fuelSlot.stackAmountText.text = fuelSlot.item.itemAmount.ToString();
        }
        else
        {
            fuelSlot.stackAmountText.text = "";
        }
        if (finishedSlot.item)
        {
            finishedSlot.stackAmountText.text = finishedSlot.item.itemAmount.ToString();
        }
        else
        {
            finishedSlot.stackAmountText.text = "";
        }
    }
    private void Update()
    {
        if (canSmelt)
        {
            if (fuelTime > 0)
            {
                if (smeltSlot.item == null)
                {
                    smeltProgress = 0;
                    canSmelt = false;
                    return;
                }
                if (Time.time > smeltTime)
                {
                    smeltTime = Time.time + 0.5f;
                    fuelTime -= 0.5f;
                    fuelSlider.value = fuelTime;
                    smeltProgress += 0.5f;
                    progressSlider.value = smeltProgress;
                    if (smeltProgress == 10)
                    {
                        smeltProgress = 0;
                        progressSlider.value = smeltProgress;
                        FinishSmelt();
                    }
                }
            }
            else
            {
                if (!NeedFuel())
                {
                    smeltProgress = 0;
                    canSmelt = false;
                    return; //stops here: no more fuel
                }
            }
        }
        else
        {
            if(fuelTime > 0)
            {
                if (Time.time > smeltTime)
                {
                    smeltTime = Time.time + 0.5f;
                    fuelTime -= 0.5f;
                    fuelSlider.value = fuelTime;
                }
            }
        }
    }


    bool NeedFuel()
    {
        if (fuelSlot.item == null)
        {
            return false;
        }
        fuelSlot.item.itemAmount--;
        fuelTime = fuelSlot.item.smeltTime;
        fuelSlider.maxValue = fuelTime;
        fuelSlider.value = fuelTime;
        if (fuelSlot.item.itemAmount == 0)
        {
            fuelSlot.item = null;
        }
        UpdateSlots();
        return true;
    }

    public void MoveItem(ItemSlot slot)
    {
        character.MoveItem(slot);
        UpdateSlots();
        SincSlots(slot);
    }
    void SincSlots(ItemSlot slot)
    {
        GameManager gam = FindObjectOfType<GameManager>();
        int objectId = GetComponent<PlaceAbleItemId>().placeabelItemID;
        if (slot == smeltSlot)
        {
            if (smeltSlot.item == null)
            {
                gam.SincSlots(0, default, 0, objectId);
            }
            else
            {
                gam.SincSlots(0, smeltSlot.item.itemName, smeltSlot.item.itemAmount, objectId);
            }
        }
        else if (slot == fuelSlot)
        {
            if (fuelSlot.item == null)
            {
                gam.SincSlots(1, default, 0, objectId);
            }
            else
            {
                gam.SincSlots(1, fuelSlot.item.itemName, fuelSlot.item.itemAmount, objectId);
            }
        }
        else if (slot == finishedSlot)
        {
            if (finishedSlot.item == null)
            {
                gam.SincSlots(2, default, 0, objectId);
            }
            else
            {
                gam.SincSlots(2, finishedSlot.item.itemName, finishedSlot.item.itemAmount, objectId);
            }
        }
        else
        {
            print("furnace sinc broke man");
        }
    }
    public void GetItemInSlot(int slotNumber, string givenItem, int amount)
    {
        if (slotNumber == 0)
        {
            if (amount == 0)
            {
                smeltSlot.item = null;
                return;
            }
            if (smeltSlot.item == null)
            {
                smeltSlot.item = FindObjectOfType<CharacterStats>().CreateItemForChest(givenItem, amount, ItemList.SelectItem(givenItem).sprite, ItemList.SelectItem(givenItem).type, ItemList.SelectItem(givenItem).maxStackSize); ;
            }
        }
        else if (slotNumber == 1)
        {
            if (amount == 0)
            {
                fuelSlot.item = null;
                return;
            }
            if (fuelSlot.item == null)
            {
                fuelSlot.item = FindObjectOfType<CharacterStats>().CreateItemForChest(givenItem, amount, ItemList.SelectItem(givenItem).sprite, ItemList.SelectItem(givenItem).type, ItemList.SelectItem(givenItem).maxStackSize); ;
            }
        }
        else if (slotNumber == 2)
        {
            if (amount == 0)
            {
                finishedSlot.item = null;
                return;
            }
            if (finishedSlot.item == null)
            {
                finishedSlot.item = FindObjectOfType<CharacterStats>().CreateItemForChest(givenItem, amount, ItemList.SelectItem(givenItem).sprite, ItemList.SelectItem(givenItem).type, ItemList.SelectItem(givenItem).maxStackSize); ;
            }
        }
    }
}