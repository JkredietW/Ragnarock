using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public List<Item> items;
    public List<GameObject> stackAbleItemList;
    [SerializeField] Transform itemsParent, hotbarParent;
    public ItemSlot[] itemSlots, hotBarSlots;
    [Space]
    [SerializeField] GameObject inventoryPanel, craftPanel, escMenu, escContent, optionsContent;
    [SerializeField] GameObject hotbarIndecator;
    [SerializeField] int allHotbarSlots = 6;

    [SerializeField] GameObject handHolder;
    GameObject handObject;

    bool inventoryEnabled;
    public int hotbarLocation;
    private PhotonView pv;

    public int goldCoinsInPocket;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI dayText;

    CharacterStats character;
    PlayerController controller;
    InventoryCraft craftingthingy;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6
     };


    public void GiveStackAbleItem(items item)
    {
        string itemName = item.itemName;
        for (int i = 0; i < stackAbleItemList.Count; i++)
        {
            if(stackAbleItemList[i].name == itemName)
            {
                stackAbleItemList[i].GetComponentInChildren<Image>().sprite = item.sprite;
                stackAbleItemList[i].GetComponentInChildren<TextMeshProUGUI>().text = item.amount.ToString();
                stackAbleItemList[i].SetActive(true);
            }
        }
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        character = GetComponent<CharacterStats>();
        controller = GetComponent<PlayerController>();
        inventoryPanel.SetActive(false);
        craftPanel.SetActive(false);
        craftingthingy = GetComponent<InventoryCraft>();
        if (pv.IsMine)
        {
            return;
        }
        else
        {
            inventoryPanel.transform.parent.gameObject.SetActive(false);
            enabled = false;
        }
    }
    private void Start()
    {
        Invoke("FixHotbarLocation", 0.1f);
    }
    void FixHotbarLocation()
    {
        SelectItemInHotBar(0);
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].slotID = i;
            itemSlots[i].inv = this;
        }
        for (int i = 0; i < hotBarSlots.Length; i++)
        {
            hotBarSlots[i].slotID = i + 25;
            hotBarSlots[i].inv = this;
        }
    }
    private void Update() //<----------------------------- update
    {
        OpenInventory();
        ScrollHotbar();
    }
    public void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            items[i] = itemSlots[i].item;
            if (itemSlots[i].item != null)
            {
                //if 0 or less remove
                if(itemSlots[i].item.itemAmount < 1)
                {
                    itemSlots[i].stackAmountText.text = "";
                    itemSlots[i].item = null;
                    continue;
                }
                if (itemSlots[i].item.itemAmount > 1)
                {
                    itemSlots[i].stackAmountText.text = itemSlots[i].item.itemAmount.ToString();
                }
                else
                {
                    itemSlots[i].stackAmountText.text = "";
                }
            }
            else
            {
                itemSlots[i].stackAmountText.text = "";
            }
        }
        i = 0;
        for (; i < hotBarSlots.Length; i++)
        {
            if (hotBarSlots[i].item != null)
            {
                if (hotBarSlots[i].item.itemAmount > 1)
                {
                    hotBarSlots[i].stackAmountText.text = hotBarSlots[i].item.itemAmount.ToString();
                }
                else
                {
                    hotBarSlots[i].stackAmountText.text = "";
                }
            }
            else
            {
                hotBarSlots[i].stackAmountText.text = "";
            }
        }
        SelectItemInHotBar(hotbarLocation);
    }
    //for world items

    void AddCoin(int amount)
    {
        controller.CoinSound();
        goldCoinsInPocket += amount;
        coinsText.text = "Golden coins : " + goldCoinsInPocket;
    }
    public void RemoveCoin(int amount)
    {
        goldCoinsInPocket -= amount;
        coinsText.text = "Golden coins : " + goldCoinsInPocket;
    }
    public void AddItem(Item item)
    {
        //tutorial stuff
        if(item.itemName == "Stick")
        {
            GetComponent<Tutorial>().FirstObjective(item.itemAmount, 0);
        }
        else if (item.itemName == "Flint")
        {
            GetComponent<Tutorial>().FirstObjective(0, item.itemAmount);
        }
        else if(item.itemName == "FlintAxe")
        {
            GetComponent<Tutorial>().SecondObjective(1);
        }
        else if (item.itemName == "OakWood")
        {
            GetComponent<Tutorial>().ThirdObjective(item.itemAmount);
        }
        else if (item.itemName == "CraftingStation")
        {
            GetComponent<Tutorial>().FourthObjective(item.itemAmount);
        }

        if (item.equipment == EquipmentType.coin)
        {
            AddCoin(item.itemAmount);
            return;
        }
        if (!IsFull())
        {
            controller.ItemSound();
            for (int i = 0; i < itemSlots.Length; i++)
            {
                //empty slot
                if (itemSlots[i].item == null)
                {
                    itemSlots[i].item = item;
                    //add item to list of inv
                    RefreshUI();
                    return;
                }
                //slot with item in it
                else if (itemSlots[i].item != null)
                {
                    //same item
                    if (itemSlots[i].item.itemName == item.itemName)
                    {
                        //less than max stack size
                        if (itemSlots[i].item.itemAmount + item.itemAmount <= itemSlots[i].item.maxStack)
                        {
                            itemSlots[i].item.itemAmount += item.itemAmount;
                            //add item to list of inv
                            RefreshUI();
                            return;
                        }
                        //more than stack size
                        else
                        {
                            continue;
                        }
                    }
                    //different item
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    Debug.LogError("I need more pockets!");
                    DropItem(item);
                    //add item to list of inv
                    RefreshUI();
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("I need more pockets!");
            DropItem(item);
            //add item to list of inv
            RefreshUI();
            return;
        }
    }
    //delete items only from list
    public bool RemoveItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item)
            {
                itemSlots[i].item = null;
                RefreshUI();
                return true;
            }
        }
        return false;
    }
    public bool IsFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null)
            {
                return false;
            }
        }
        return true;
    }

    void ScrollHotbar()
    {
        if (!controller.placementCheck)
        {
            //scroll in hotbar
            if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
            {
                hotbarLocation -= (int)Input.mouseScrollDelta.y;
                if (hotbarLocation > allHotbarSlots - 1)
                {
                    hotbarLocation = 0;
                }
                else if (hotbarLocation < 0)
                {
                    hotbarLocation = allHotbarSlots - 1;
                }
                SelectItemInHotBar(hotbarLocation);
            }
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    hotbarLocation = i;
                    SelectItemInHotBar(hotbarLocation);
                }
            }
        }
    }
    void SelectItemInHotBar(int _location)
    {
        hotbarIndecator.transform.position = hotBarSlots[_location].transform.position;
        if(hotBarSlots[_location].item != null)
        {
            controller.heldItem = hotBarSlots[_location].item;
            GetComponent<PhotonView>().RPC("ShowItemInHand", RpcTarget.All, controller.heldItem.itemName);
        }
        else
        {
            GetComponent<PhotonView>().RPC("ShowItemInHand", RpcTarget.All, "EmptyItem");
        }
        character.CalculateOffensiveStats();
    }
    [PunRPC]
    void ShowItemInHand(string nameOfItem)
    {
        for (int i = 0; i < handHolder.transform.childCount; i++)
        {
            if(handHolder.transform.GetChild(i).name == nameOfItem)
            {
                handHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                handHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            OpenActualInventory(false);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenEscMenu();
        }
    }
    public void OpenActualInventory(bool closeCraft)
    {
        SelectItemInHotBar(hotbarLocation);
        inventoryEnabled = !inventoryEnabled;
        inventoryPanel.SetActive(inventoryEnabled);
        craftingthingy.CanCraft();
        if (!closeCraft)
        {
            craftPanel.SetActive(inventoryEnabled);
        }
        GetComponent<PlayerController>().LockCamera();
        PlayerController con = GetComponent<PlayerController>();
        if (con.lastChest != null)
        {
            con.lastChest.CloseChestInventory();
            con.lastChest = null;
        }
        if (con.lastCratingStation != null)
        {
            con.lastCratingStation.CloseChestInventory();
            con.lastCratingStation = null;
        }
        if (con.lastOvenStation != null)
        {
            con.lastOvenStation.CloseChestInventory();
            con.lastOvenStation = null;
        }
        if (!inventoryEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (character.draggableItem.item != null)
            {
                DropItem(character.draggableItem.item);
                character.draggableItem.item = null;
                character.draggableItem.stackAmountText.text = "";
                character.draggableItem.gameObject.GetComponent<Image>().color = character.disabledColor;
                character.itemIsBeingDragged = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        Cursor.visible = inventoryEnabled;
    }
    public void DropItem(Item item)
    {
        if (item != null)
        {
            //master client needs to do this!
            GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", item.itemName), transform.position + transform.forward, Quaternion.identity);
            droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
            droppedItem.GetComponent<WorldItem>().SetUp(item.itemName, item.itemAmount, ItemList.SelectItem(item.itemName).sprite, ItemList.SelectItem(item.itemName).type, ItemList.SelectItem(item.itemName).maxStackSize);
            SelectItemInHotBar(hotbarLocation);
            //sinc hier stacks of item

        }
    }
    public void OpenEscMenu()
    {
        escMenu.SetActive(!escMenu.activeSelf);
        controller.LockCamera();
        if (escMenu.activeSelf)
        {
            FakeOptions(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            if(inventoryEnabled)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    public void ResumeGame()
    {
        escMenu.SetActive(false);
        controller.LockCamera();
    }
    public void FakeOptions(bool onOff)
    {
        optionsContent.SetActive(onOff);
        escContent.SetActive(!onOff);
    }
    public void ToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void GiveDaysToMe(string daysAmount)
    {
        dayText.text = "Day : " + daysAmount;
    }
}
