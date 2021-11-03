using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class WorldItem : MonoBehaviour
{
    public string itemName;
    public Sprite itemImage;
    public int itemAmount, maxStack;

    public bool mayBePickedUp;

    public EquipmentType equipment;
    public int itemID;

    [Space]
    [HideInInspector] public int damageBonus;
    [HideInInspector] public int armorBonus;
    [HideInInspector] public int attackSpeedBonus;
    [HideInInspector] public int critChanceBonus;
    [HideInInspector] public int healthBonus;
    [Space]
    [HideInInspector] public int damagePrecentBonus;
    [HideInInspector] public int armorPrecentBonus;
    [HideInInspector] public int attackSpeedPrecentBonus;
    [HideInInspector] public int critChancePrecentBonus;
    [HideInInspector] public int healthPrecentBonus;
    private void Start()
    {
        mayBePickedUp = false;
        Invoke("Cooldown", 0.1f);
    }
    public void Cooldown()
    {
        mayBePickedUp = true;
    }
    public void SetUp(string name, int amount, Sprite image, EquipmentType type, int _maxStack)
    {
        itemName = name;
        itemAmount = amount;
        itemImage = image;
        equipment = type;
        maxStack = _maxStack;
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (mayBePickedUp)
        {
            if (other.CompareTag("Player"))
            {
                mayBePickedUp = false;
                Invoke("Cooldown", 1f);
                if (other.GetComponent<Inventory>().IsFull())
                {
                    return;
                }
                other.GetComponent<CharacterStats>().CreateItem(itemName, itemAmount, itemImage, equipment, maxStack);
                if (GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient)
                {
                    GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                }
                GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
                Invoke("SecLater", 0.1f);
            }
        }
    }
    void SecLater()
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
    }
    [PunRPC]
    public void DestroyWorldItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
