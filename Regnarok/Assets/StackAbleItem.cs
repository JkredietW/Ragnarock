using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StackAbleItem : MonoBehaviour
{
	public int id;
	public string itemName;
	public void Collision()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
