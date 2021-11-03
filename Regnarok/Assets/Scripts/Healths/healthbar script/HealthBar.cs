using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBar: MonoBehaviour
{
	public Slider healtSlider;
	public GameObject healtbarObject;
	public List<GameObject> players;
	public GameObject myPlayer;
	public void GetMyPlayer()
	{
		players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].GetComponent<PhotonView>().IsMine)
			{
				myPlayer = players[i];
			}
		}
	}
}
