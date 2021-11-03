using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public struct items
{
    public string itemName;
    public int amount;
    [TextArea]
    public string decription;
    public float value;
    public Sprite sprite;
}

public class StackableItemScript : MonoBehaviour
{
	public items[] itemlist;
	private bool cooldownBool;
	private void OnTriggerEnter(Collider other)
	{
        //destory for multiplayer
		if (other.transform.GetComponent<StackAbleItem>())
		{
			if (!cooldownBool)
			{
				AddItem(other.transform.GetComponent<StackAbleItem>().id);
				other.transform.GetComponent<StackAbleItem>().Collision();
			}
		}
	}
	public void AddItem(int index)
	{
		cooldownBool = true;
		if (ChanceToGetDubbel())
		{
			itemlist[index].amount += 2;
		}
		else
		{
			itemlist[index].amount++;
		}
        //hier stat calculation
        GiveStatsToStats();

        Invoke("ResetCooldown", 0.1f);
	}
    void GiveStatsToStats()
    {
        CharacterStats stats = GetComponent<CharacterStats>();
        for (int i = 0; i < itemlist.Length; i++)
        {
            if(itemlist[i].amount > 0)
            {
                if(itemlist[i].itemName == "EnergyDrink")
                {
                    stats.GiveStats_movementSpeed(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Shield")
                {
                    stats.GiveStats_addedArmor(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "IronKnuckle")
                {
                    stats.GiveStats_damageFlat(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "HeartContainer")
                {
                    stats.GiveStats_addedHealth(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "ShadowOrb")
                {
                    stats.GiveStats_healthOnKill(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Crown")
                {
                    stats.GiveStats_xpmulti(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Knife")
                {
                    stats.GiveStats_bleedChance(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Clock")
                {
                    stats.GiveStats_attackSpeedFlat(itemlist[i].amount * itemlist[i].value);
                }
                else if (itemlist[i].itemName == "Scouter")
                {
                    stats.GiveStats_critChanceFlat(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "GoldPouch")
                {
                    FindObjectOfType<GameManager>().GiveStats_goldmulti(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Plaster")
                {
                    stats.GiveStats_healthRegen(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Tooth")
                {
                    stats.GiveStats_addLifeSteal(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "BigHeartContainer")
                {
                    stats.GiveStats_healthPrecent(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "WingShoes")
                {
                    stats.GiveStats_addedJumps(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Guillotine")
                {
                    stats.GiveStats_execute(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Revive")
                {
                    stats.GiveStats_revives(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Nimbus")
                {
                    stats.GiveStats_nimbus(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Matches")
                {
                    stats.GiveStats_burn(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Skull")
                {
                    stats.GiveStats_poison(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
            }
        }

        ItemsToInventory();
        stats.CalculateOffensiveStats();
        stats.CalculateDefensiveStats();
    }
    public void RemoveItem(string nameOfItem)
    {
        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].itemName == nameOfItem)
            {
                itemlist[i].amount -= 1;
                return;
            }
        }
    }
    void ItemsToInventory()
    {
        Inventory inv = GetComponent<Inventory>();
        for (int i = 0; i < itemlist.Length; i++)
        {
            if (itemlist[i].amount > 0)
            {
                inv.GiveStackAbleItem(itemlist[i]);
            }
        }
    }
	public void ResetCooldown()
	{
		cooldownBool = false;
	}
	public bool ChanceToGetDubbel()
	{
		if (Random.Range(0.00f, 100.00f) <= 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
