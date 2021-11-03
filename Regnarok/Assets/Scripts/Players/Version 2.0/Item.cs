using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(1, 100)]
    public int itemAmount, maxStack;

    [Space]
    [SerializeField] public int foodLifeRestore;
    [Space]
    [HideInInspector] public float damageBonus;
    [HideInInspector] public float armorBonus;
    [HideInInspector] public float attackSpeedBonus;
    [HideInInspector] public float critChanceBonus;
    [HideInInspector] public float healthBonus;
    [Space]
    [HideInInspector] public float damagePrecentBonus;
    [HideInInspector] public float armorPrecentBonus;
    [HideInInspector] public float attackSpeedPrecentBonus;
    [HideInInspector] public float critChancePrecentBonus;
    [HideInInspector] public float healthPrecentBonus;
    [Space]
    public EquipmentType equipment;
    [Space]
    [HideInInspector] public float smeltTime;
    [HideInInspector] public GameObject summonObject;
    public string smeltResult;

    public virtual void SetUpNewItem(string _itemName, int _itemAmount, Sprite _icon, EquipmentType _type, int _maxStack, int _heal, float _smeltTime, GameObject _summonObject)
    {
        itemName = _itemName;
        itemAmount = _itemAmount;
        icon = _icon;
        maxStack = _maxStack;
        equipment = _type;
        foodLifeRestore = _heal;
        smeltTime = _smeltTime;
        summonObject = _summonObject;
    }
}
