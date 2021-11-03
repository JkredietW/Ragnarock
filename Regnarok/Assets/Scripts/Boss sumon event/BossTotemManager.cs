using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BossTotemManager : MonoBehaviour
{
    public List<GameObject> bosTotems;
	public GameManager gm;
    public int amountOffBosses;
	public LayerMask groundLayer;
	public int activatedTotems;
	private bool bosSpawned;
	void Update()
	{
		if (bosTotems.Count > 0)
		{
			for (int i = 0; i < bosTotems.Count; i++)
			{
				if (bosTotems[i] == null)
				{
					bosTotems.Remove(bosTotems[i]);
				}
			}
		}
		if (!bosSpawned)
		{
			if (FindObjectOfType<GameManager>().playerObjectList.Count > 0)
			{ 
				if (bosTotems.Count <= 0)
				{
					if (activatedTotems >= amountOffBosses)
					{
						SpawnBoss();
					}
				}
			}
		}
    }
	public void SpawnBoss()
	{
		bosSpawned = true;
		Ray ray = new Ray(GetPos(), -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			if (hitInfo.transform.tag == "Mesh")
			{
				gm.GetComponent<PhotonView>().RPC("SpawnPartical", RpcTarget.MasterClient, hitInfo.point);
				new WaitForSeconds(0.5f);
				gm.SpawnEndBoss(hitInfo.point);
			}
		}
	}
	public Vector3 GetPos()
	{
		Vector3 pos = new Vector3();
		pos.x += Random.Range(-6.00f, 6.00f);
		pos.z += Random.Range(-6.00f, 6.00f);

		pos.y = 100;
		return pos;
	}
}
