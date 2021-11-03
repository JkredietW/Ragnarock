using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class Totem : MonoBehaviour
{
	public LayerMask groundLayer;
	public int id;
	public bool isBoss;	
	public bool activated;
	public bool gaveItem;
	public int amountOfEnemies;
	public float timeToCheck;
	public Vector3 itemSpawnOffset = new Vector3(0, 1.5f, 0);
	public Vector3 spawnOffset = new Vector3(0, 1, 0);
	public GameObject[] torches;
	public  ItemListScript itemlist;
	public string spawnParticleName;
	public List<GameObject> enemies;
	public AudioSource activedSound;
	private bool isChecking;
	private bool allEnemiesDied;
	private int type;
	private GameManager gm;
    private void Awake()
    {
		gm = FindObjectOfType<GameManager>();
	}
    private void Start()
	{
		type = Random.Range(1, 4);
		amountOfEnemies *= type;
	}
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (!gaveItem)
			{
				if (activated)
				{
					if (!isChecking)
					{
						if (!allEnemiesDied)
						{
							StartCoroutine(CheckEnemies());
						}
					}
				}
			}
		}
	}
	public IEnumerator CheckEnemies()
	{	
		isChecking = true;
		for (int i = 0; i < enemies.Count; i++)
		{
			if (enemies[i] == null)
			{
				enemies.RemoveAt(i);
			}
			else
			{
				enemies[i].transform.GetComponent<Outline>().enabled = true;
			}
		}
		if (enemies.Count<=0)
		{
			allEnemiesDied = true;
			if (isBoss)
			{
				gm.AddActivatedTotemBoss();
				Destroy(gameObject);
			}
			else
			{
				if (!gaveItem)
				{
					GiveItem();
				}
			}
		}
		for (int i = 0; i < torches.Length; i++)
		{
			if (i > enemies.Count)
			{
				torches[i].SetActive(false);
			}
		}
		yield return new WaitForSeconds(timeToCheck);
		isChecking = false;
	}
	public void GiveItem()
	{
		gaveItem = true;
		int rarity = RaretyChance();
		Ray ray = new Ray(GetPos(), -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			gm.SpawnItem(hitInfo.point, rarity);
		}
		gm.DestroyTotem(id);
	}
	public void Interact()
	{
		if (!activated)
		{
			if (!activedSound.isPlaying)
			{
				activedSound.Play();
			}
			gm = FindObjectOfType<GameManager>();
			if (isBoss)
			{
				SpawnBoss();
			}
			else
			{
				for (int i = 0; i < amountOfEnemies; i++)
				{
					SpawnEnemies();
				}
			}
			gm.ActivatTotem(id,isBoss);
		}
	}  
	public Vector3 GetPos()
	{
		Vector3 pos = transform.position;
		pos.x += Random.Range(-6.00f, 6.00f);
		pos.z += Random.Range(-6.00f, 6.00f);

		pos.y = 100;
		return pos;
	}
	public void SpawnBoss()
	{
		Ray ray = new Ray(GetPos(), -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			if (hitInfo.transform.tag == "Mesh")
			{
				gm.GetComponent<PhotonView>().RPC("SpawnPartical", RpcTarget.MasterClient, hitInfo.point);
				new WaitForSeconds(0.5f);
				gm.SpawnBoss( hitInfo.point + spawnOffset, id);
			}
		}
	}
	void SpawnEnemies()
	{
		int randomNum = Random.Range(0, gm.enemielist.enemieList.Count);
		Ray ray = new Ray(GetPos(), -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			if (hitInfo.transform.tag == "Mesh")
			{
				gm.GetComponent<PhotonView>().RPC("SpawnPartical", RpcTarget.MasterClient, hitInfo.point);
				new WaitForSeconds(0.5f);
				gm.SpawnEnemies(randomNum, hitInfo.point + spawnOffset,id);
			}
			else
			{
				amountOfEnemies++;
			}
		}
	}
	[PunRPC]
	public void SpawnPartical(Vector3 spawnPos)
	{
		GameObject tempObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SpawnPartical"), spawnPos, Quaternion.identity);
		new WaitForSeconds(0.6f);
		PhotonNetwork.Destroy(tempObject);
	}
	public int RaretyChance()
	{
		float randomNum = Random.Range(0.00f, 100.00f);
		int itemRarity = 0;

		if (type == 1)
		{
			if (randomNum <= 68.5f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 68.5f && randomNum <= 93.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 93.5f && randomNum <= 98.5f)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 98.5f && randomNum <= 99.5f)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 99.5f)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 2)
		{
			if (randomNum <= 27.5f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 27.5f && randomNum <= 82.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 82.5f && randomNum <= 92.5f)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 92.5f && randomNum <= 97.5f)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 97.5f)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 3)
		{
			if (randomNum <= 24.2f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 24.2f && randomNum <= 60.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 60.5f && randomNum <= 80)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 80 && randomNum <= 94)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 94)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 4)
		{
			if (randomNum <= 5)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 5 && randomNum <= 25)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 25 && randomNum <= 65)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 65 && randomNum <= 85)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 85)
			{//mythic
				itemRarity = 4;
			}
		}
		return itemRarity;
	}
}