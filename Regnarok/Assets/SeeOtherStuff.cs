using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SeeOtherStuff : MonoBehaviour
{
	public List<SeeOtherStuff> players;
	public List<Totem> bosTotems;
	private bool active;
	private bool totemActived;
	private void Start()
	{
		players = new List<SeeOtherStuff>(FindObjectsOfType<SeeOtherStuff>());
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].transform.GetComponent<PhotonView>().IsMine)
			{
				players.Remove(players[i]);
			}
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			players.Clear();

			players = new List<SeeOtherStuff>(FindObjectsOfType<SeeOtherStuff>());
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i]==this)
				{
					players.Remove(players[i]);
				}
			}
			
			for (int i = 0; i < players.Count; i++)
			{
				TriggerPlayer(i);
			}
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			bosTotems.Clear();

			bosTotems = new List<Totem>(FindObjectsOfType<Totem>());
			for (int i = 0; i < bosTotems.Count; i++)
			{
				if (!bosTotems[i].isBoss)
				{
					bosTotems.Remove(bosTotems[i]);
				}
			}
			TriggerTotem();
		}
	}
	public void TriggerTotem()
	{
		if (GetComponent<PhotonView>().IsMine)
		{
			totemActived = !totemActived;
			for (int i = 0; i < bosTotems.Count; i++)
			{
				if (bosTotems[i] != null)
				{
					bosTotems[i].transform.gameObject.GetComponent<Outline>().enabled = totemActived;
				}
			}
		}
	}
	public void TriggerPlayer(int i)
	{
		active = !active;

		players[i].GetComponent<Outline>().enabled = active;
	}
}
