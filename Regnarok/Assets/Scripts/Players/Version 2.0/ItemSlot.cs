using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    public int slotID;
    public Inventory inv;
    public ChestInventory chestInv;

    public TextMeshProUGUI stackAmountText;

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);

    private Item _item;
    private void Start()
    {
        if (GetComponentInChildren<TextMeshProUGUI>())
        {
            stackAmountText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    public Item item
    {
        get { return _item; }
        set {
            _item = value;

            if (_item == null)
            {
                image.color = disabledColor;
            }
            else
            {
                image.sprite = _item.icon;
                image.color = normalColor;
            }
        }
    }
}