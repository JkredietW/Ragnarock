using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel EquipmentPanel;
    public ItemSlot draggableItem;
    public Items[] itemList;//list of items
    public bool itemIsBeingDragged;
    PlayerController playercontroller;
    [Space]
    public TextMeshProUGUI nameOfItemText;
    public TextMeshProUGUI amountOfItemText;
    public TextMeshProUGUI typeOfItemText;

    //color
    [HideInInspector] public Color normalColor = Color.white;
    [HideInInspector] public Color disabledColor = new Color(1, 1, 1, 0);

    [Space]
    float endDamage, endAttackSpeed, endCritChance, endArmor, endHealth, endJumpAmount, executeBelow, revives, nimbusStacks, burnStacks, poisonStacks;
    public float BaseDamage, baseAttackSpeed, baseCritChance, baseArmor, baseHealth;
    float addedDamage, addedAttackSpeed, addedCritChance, addedArmor, addedHealth, addedHealthRegen, addedLifeSteal, addedBleedChance, addedHealthOnKill, addedMovementSpeed, addedJumps;
    float precentAddedDamage, precentAddedAttackSpeed, precentAddedCritChance, precentAddedArmor, precentAddedHealth;

    [Space]
    //xp
    public int level;
    public float xpAmountNeeded, xpAmount, xpIncreasment, xpGainedMultiplier = 1;
    public Slider xpBar;

    public GameObject bambooHat, vikingHat, wizardHat;

    public void GainXp(float _xpAmount)
    {
        xpAmount += _xpAmount * xpGainedMultiplier;
        xpBar.value = xpAmount;
        if (xpAmount > xpAmountNeeded)
        {
            level++;
            xpAmount -= xpAmountNeeded;
            xpAmountNeeded *= xpIncreasment;
            xpBar.maxValue = xpAmountNeeded;
            xpBar.value = xpAmount;
            CalculateOffensiveStats();
            CalculateDefensiveStats();
            GainXp(0);
        }
    }

    private void Awake()
    {
        playercontroller = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if(itemIsBeingDragged)
        {
            draggableItem.transform.position = Input.mousePosition;
        }
    }
    #region be granted with stats :D
    public void GiveStats_damageFlat(float value)
    {
        addedDamage = value;
    }
    public void GiveStats_damagePrecent(float value)
    {
        precentAddedDamage = value;
    }
    public void GiveStats_attackSpeedFlat(float value)
    {
        addedAttackSpeed = value;
    }
    public void GiveStats_attackSpeedPrecent(float value)
    {
        precentAddedAttackSpeed = value;
    }
    public void GiveStats_critChanceFlat(float value)
    {
        addedCritChance = value;
    }
    public void GiveStats_critChancePrecent(float value)
    {
        precentAddedCritChance = value;
    }
    public void GiveStats_addedHealth(float value)
    {
        addedHealth = value;
    }
    public void GiveStats_healthRegen(float value)
    {
        addedHealthRegen = value;
    }
    public void GiveStats_addedArmor(float value)
    {
        addedArmor = value;
    }
    public void GiveStats_addLifeSteal(float value)
    {
        addedLifeSteal = value;
    }
    public void GiveStats_bleedChance(float value)
    {
        addedBleedChance = value;
    }
    public void GiveStats_healthOnKill(float value)
    {
        addedHealthOnKill = value;
    }
    public void GiveStats_movementSpeed(float value)
    {
        addedMovementSpeed = value;
    }
    public void GiveStats_xpmulti(float value)
    {
        xpGainedMultiplier = value + 1;
    }
    public void GiveStats_healthPrecent(float value)
    {
        precentAddedHealth = value;
    }
    public void GiveStats_addedJumps(float value)
    {
        addedJumps = value;
    }
    public void GiveStats_execute(float value)
    {
        executeBelow = value / 100;
    }
    public void GiveStats_revives(float value)
    {
        revives = value;
    }
    public void GiveStats_nimbus(float value)
    {
        nimbusStacks = value;
    }
    public void GiveStats_burn(float value)
    {
        burnStacks = value;
    }
    public void GiveStats_poison(float value)
    {
        poisonStacks = value;
    }
    #endregion
    public void CalculateOffensiveStats()
    {
        //nog geen accesory stats

        if (GetComponent<PlayerController>().heldItem)
        {
            Item item = GetComponent<PlayerController>().heldItem;
            if (item.equipment == EquipmentType.axe || item.equipment == EquipmentType.pickaxe || item.equipment == EquipmentType.weapon)
            {
                endDamage = ((BaseDamage * (level * 0.1f + 1)) + addedDamage + item.damageBonus) * ((precentAddedDamage + item.damagePrecentBonus) / 100 + 1);
                endAttackSpeed = ((baseAttackSpeed * (level * 0.01f + 1)) + addedAttackSpeed + item.attackSpeedBonus) * ((precentAddedAttackSpeed + item.attackSpeedPrecentBonus) / 100 + 1);
                //endAttackSpeed = tempAttackSpeed / (tempAttackSpeed * tempAttackSpeed);
                endCritChance = ((baseCritChance * (level * 0.01f + 1)) + addedCritChance + item.critChanceBonus) * ((precentAddedCritChance + item.critChancePrecentBonus) / 100 + 1);
            }
            else
            {
                endDamage = 1 * (level * 0.1f + 1);
                endAttackSpeed = 1 * (level * 0.1f + 1);
                endCritChance = 5 * (level * 0.1f + 1);
            }
        }
        else
        {
            endDamage = 1 * (level * 0.1f + 1);
            endAttackSpeed = 1 * (level * 0.1f + 1);
            endCritChance = 5 * (level * 0.1f + 1);
            endJumpAmount = addedJumps + 1;
        }

        //give stats/ offensive
        playercontroller.RecieveStats(endDamage, endAttackSpeed, endCritChance, addedLifeSteal, addedBleedChance, addedHealthOnKill, addedMovementSpeed, (int)endJumpAmount, executeBelow, (int)nimbusStacks, (int)burnStacks, poisonStacks);
    }
    public void CalculateDefensiveStats()
    {
        //defensive
        endArmor = ((baseArmor * (level * 0.1f + 1)) + addedArmor) * (precentAddedArmor / 100 + 1);
        endHealth = ((baseHealth * (level * 0.1f + 1)) + addedHealth) * (precentAddedHealth / 100 + 1);

        GetComponent<Health>().RecieveStats(endHealth, endArmor, addedHealthRegen, (int)revives);
    }
    public void HoverItem(ItemSlot slot)
    {
        if (!itemIsBeingDragged)
        {
            if (slot.item == null)
            {
                nameOfItemText.text = string.Empty;
                amountOfItemText.text = string.Empty;
                typeOfItemText.text = string.Empty;
            }
            else
            {
                nameOfItemText.text = slot.item.itemName;
                amountOfItemText.text = slot.item.itemAmount.ToString() + " / " + slot.item.maxStack.ToString();
                if (slot.item.equipment == EquipmentType.none)
                {
                    typeOfItemText.text = "";
                    return;
                }
                typeOfItemText.text = slot.item.equipment.ToString();
            }
        }
    }
    public void EquipHat(ItemSlot slot)
    {
        GetComponent<PhotonView>().RPC("ChangeHat", RpcTarget.All, slot.item.itemName);
    }
    [PunRPC]
    public void ChangeHat(string _itemName)
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            return;
        }
        bambooHat.SetActive(false);
        wizardHat.SetActive(false);
        vikingHat.SetActive(false);
        if (_itemName == "BambooHat")
        {
            bambooHat.SetActive(true);
        }
        else if (_itemName == "WizardHat")
        {
            wizardHat.SetActive(true);
        }
        else if (_itemName == "VikingHat")
        {
            vikingHat.SetActive(true);
        }
    }
    public void MoveItem(ItemSlot itemslot)
    {
        if (Input.GetKey(KeyCode.Q))
        {
            inventory.DropItem(itemslot.item);
            itemslot.item = null;
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            if (!itemIsBeingDragged)
            {
                if(itemslot.item.itemAmount == 1)
                {
                    draggableItem.item = itemslot.item;
                    draggableItem.item.itemAmount = 1;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    itemslot.item = null;
                }
                else if(itemslot.item.itemAmount > 1)
                {
                    itemslot.item.itemAmount--;
                    draggableItem.item = itemslot.item;
                    draggableItem.item.itemAmount = 1;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                }
                itemIsBeingDragged = true;
            }
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            if (!itemIsBeingDragged)
            {
                if (itemslot.item.itemAmount > 1)
                {
                    float roundedDown = 0;
                    float roundedUp = 0;
                    int half = 0;
                    if (itemslot.item.itemAmount % 2 == 0)
                    {
                        half = itemslot.item.itemAmount / 2;
                    }
                    else if (itemslot.item.itemAmount % 2 == 1)
                    {
                        roundedDown = (float)itemslot.item.itemAmount / 2 - 0.5f;
                        roundedUp = (float)itemslot.item.itemAmount / 2 + 0.5f;
                    }
                    if (half > 0)
                    {
                        itemslot.item.itemAmount = half;
                        draggableItem.item = itemslot.item;
                        draggableItem.item.itemAmount = half;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    }
                    else
                    {
                        itemslot.item.itemAmount = (int)roundedUp;
                        draggableItem.item = itemslot.item;
                        draggableItem.item.itemAmount = (int)roundedDown;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    }
                    itemIsBeingDragged = true;
                }
            }
        }
        else
        {
            if (itemIsBeingDragged)
            {
                //empty slot
                if (itemslot.item == null)
                {
                    if (itemslot is EquipmentSlots)
                    {
                        EquipmentSlots quipSlot = itemslot as EquipmentSlots;
                        if (quipSlot.EquipmentType == draggableItem.item.equipment)
                        {
                            itemslot.item = draggableItem.item;
                            draggableItem.item = null;
                            draggableItem.gameObject.GetComponent<Image>().color = disabledColor;

                            //toggle
                            itemIsBeingDragged = !itemIsBeingDragged;
                        }
                    }
                    else
                    {
                        itemslot.item = draggableItem.item;
                        draggableItem.item = null;
                        draggableItem.gameObject.GetComponent<Image>().color = disabledColor;

                        //toggle
                        itemIsBeingDragged = !itemIsBeingDragged;
                    }
                }
                //slot with item in it
                else if (itemslot.item != null)
                {
                    //same item
                    if (itemslot.item.itemName == draggableItem.item.itemName)
                    {
                        //less than max stack size
                        if (itemslot.item.itemAmount + draggableItem.item.itemAmount <= itemslot.item.maxStack)
                        {
                            itemslot.item.itemAmount += draggableItem.item.itemAmount;
                            draggableItem.item = null;
                            draggableItem.gameObject.GetComponent<Image>().color = disabledColor;
                            itemIsBeingDragged = !itemIsBeingDragged;
                        }
                        //more than stack size
                        else
                        {
                            if (itemslot is EquipmentSlots)
                            {
                                EquipmentSlots quipSlot = itemslot as EquipmentSlots;
                                if (quipSlot.EquipmentType == draggableItem.item.equipment)
                                {
                                    draggableItem.item.itemAmount = itemslot.item.itemAmount + draggableItem.item.itemAmount - itemslot.item.maxStack;
                                    itemslot.item.itemAmount = itemslot.item.maxStack;
                                }
                            }
                            else
                            {
                                draggableItem.item.itemAmount = itemslot.item.itemAmount + draggableItem.item.itemAmount - itemslot.item.maxStack;
                                itemslot.item.itemAmount = itemslot.item.maxStack;
                            }
                        }
                    }
                    //different item
                    else
                    {
                        Item swappedItem = itemslot.item;
                        itemslot.item = draggableItem.item;
                        draggableItem.item = swappedItem;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                        draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;
                        //do not toggle here! it will break
                    }
                }
                else
                {
                    Debug.LogError("I need more pockets!");
                }
            }
            else
            {
                //no item in slot
                if (itemslot.item != null)
                {
                    draggableItem.item = itemslot.item;
                    itemslot.item = null;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;

                    //toggle
                    itemIsBeingDragged = !itemIsBeingDragged;
                }
            }
        }
        //refresh inventory ui/ chest does it himself
        if (itemslot.inv != null)
        {
            itemslot.inv.RefreshUI();
        }
        //update dragged item stats
        if (draggableItem.item != null)
        {
            if (draggableItem.item.itemAmount > 1)
            {
                draggableItem.stackAmountText.text = draggableItem.item.itemAmount.ToString();
            }
            else
            {
                draggableItem.stackAmountText.text = "";
            }
        }
        else
        {
            draggableItem.stackAmountText.text = "";
        }
        if (GetComponent<PlayerController>().lastCratingStation != null)
        {
            GetComponent<PlayerController>().lastCratingStation.CanCraft();
        }
        else
        {
            GetComponent<InventoryCraft>().CanCraft();
        }
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        if(type != EquipmentType.none)
        {
            newItem.damageBonus = ItemList.SelectItem(name).baseDamage;
            newItem.attackSpeedBonus = ItemList.SelectItem(name).baseAttackSpeed;
            newItem.critChanceBonus = ItemList.SelectItem(name).baseCritChance;
        }
        newItem.SetUpNewItem(name, amount, image, type, maxStack, ItemList.SelectItem(name).foodHealAmount, ItemList.SelectItem(name).smeltTime, ItemList.SelectItem(name).summonObject);

        inventory.AddItem(newItem);
    }
    public Item CreateItemForChest(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        if (type != EquipmentType.none)
        {
            newItem.damageBonus = ItemList.SelectItem(name).baseDamage;
            newItem.attackSpeedBonus = ItemList.SelectItem(name).baseAttackSpeed;
            newItem.critChanceBonus = ItemList.SelectItem(name).baseCritChance;
        }
        newItem.SetUpNewItem(name, amount, image, type, maxStack, ItemList.SelectItem(name).foodHealAmount, ItemList.SelectItem(name).smeltTime, ItemList.SelectItem(name).summonObject);
        return newItem;
    }
    [System.Serializable]
    public struct Items
    {
        public int id;
        public int amount;
    }

    public void RecipeInfo(RecipeHolder _recipe)
    {
        nameOfItemText.text = _recipe.recipe.resultName;
        amountOfItemText.text = _recipe.recipe.resultAmount.ToString();
        typeOfItemText.text = _recipe.recipe.typeOfResult.ToString();
    }
}
