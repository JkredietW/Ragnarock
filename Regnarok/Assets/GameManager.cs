using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{

    [Header("Scaling")]
    public bool isDoingNight;
    public float timeForNightToEnd;
    public float scalingAmount;
    public float scalingIncreaseAmount;
    public int days;
    [Header("Player Spawn")]
    public GameObject playerObject;
	public LayerMask groundLayer;
    public float spawnHeightOffset;
    public float spawnRadius;
    public float spawnHeight;
    private EnemySpawner es;
    public PlayerManager playerManager;
    public ItemListScript itemlist;
    public EnemyList enemielist;
    public string miniBossName;
    public string endBossName;

    public List<GameObject> playerObjectList;
    public Vector3 deathPos;
    [Space]
    public float goldMultiplier =1;

    [Header("aduio")]
    public AudioSource dayAudio;
    public AudioSource nightAudio;
    public AudioSource isNight;
    public AudioSource playerSpawnSound;

    public AudioMixer audioMaster;

    public TextMeshProUGUI quoteText;

    [TextArea]
    public List<string> quotes;

    public GameObject endboss;
    private bool endBossSpawned;
    private bool spawnedPortal;
    public GameObject cameraOff;
    int objectInstanceID;
    public void GiveStats_goldmulti(float value)
    {
        goldMultiplier = value + 1;
    }
    private void Start()
	{
        es = GetComponent<EnemySpawner>();
        GetRandomQuote();
    }
	private void Update()
	{
        if (PhotonNetwork.IsMasterClient)
        {
            if (endBossSpawned)
            {
                if (endboss == null)
                {
                    if (!spawnedPortal)
                    {
                        SpawnThePortal();
                    }
                }
            }
        }
	}
    public void SpawnThePortal()
	{
		if (PhotonNetwork.IsMasterClient)
		{
            spawnedPortal = true;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Portal"), deathPos+new Vector3(0,1,0), Quaternion.identity);
        }
	}
	public IEnumerator IsNight()
	{
		dayAudio.Stop();
		nightAudio.Play();
        isNight.Play();
		isDoingNight = true;
        days++;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            item.GetComponent<Inventory>().GiveDaysToMe(days.ToString());
        }
        es.ClearPlayers();
        es.GetPlayers();
        es.SpawnEnemies(ScalingLeJorn());
        yield return new WaitForSeconds(timeForNightToEnd);
        scalingAmount = 1;
        isDoingNight = false;
    }
    public float ScalingLeJorn()
	{
        return scalingAmount = scalingIncreaseAmount * days;
	}
    public void AddActivatedTotemBoss()
	{
        GetComponent<PhotonView>().RPC("AddActivatedTotemBossSync", RpcTarget.All);
    }
    [PunRPC]
    public void AddActivatedTotemBossSync()
	{
        FindObjectOfType<BossTotemManager>().activatedTotems++;
	}
    public void SpawnPlayers()
	{
        Destroy(cameraOff);
        quoteText.gameObject.SetActive(false);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
            Vector3 spawnpos = new Vector3(Random.Range(spawnRadius, -spawnRadius), spawnHeight, Random.Range(-spawnRadius, spawnRadius));


            Ray ray = new Ray(spawnpos, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                spawnpos.y = hitInfo.point.y+spawnHeightOffset;
            }
            deathPos = spawnpos;
            if (playerManager.pv.Owner == PhotonNetwork.PlayerList[i])
            {
                playerManager.SpawnPlayer(spawnpos);
                PlayRespawnSound();
            }
        }
	}
    public void PlayRespawnSound()
	{
        playerSpawnSound.Play();
    }
    public void GiveXpFromHitableObject(float amount)
    {
        GetComponent<PhotonView>().RPC("GiveXp", RpcTarget.All, amount);
    }
    [PunRPC]
    public void GiveXp(float amount)
    {
        CharacterStats[] players = FindObjectsOfType<CharacterStats>();
        foreach (CharacterStats player in players)
        {
            if (player.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
            {
                player.GainXp(amount);
            }
        }
    }
    public void OpenChest(int id)
	{
        GetComponent<PhotonView>().RPC("OpenChestInWorld", RpcTarget.All,id);
    }
    private int amountOffDeads;
    public void CheckHp()
	{
        amountOffDeads = 0;
        for (int i = 0; i < playerObjectList.Count; i++)
		{
			if (playerObjectList[i].GetComponent<PlayerHealth>().isDead)
			{
                amountOffDeads++;
			}
		}
        if(amountOffDeads>= playerObjectList.Count)
		{
            GetComponent<PhotonView>().RPC("DisconectAll", RpcTarget.All);
        }
	}
    public void DestroyTotem(int i)
	{
        GetComponent<PhotonView>().RPC("DestroyOnMaster", RpcTarget.All,i);
    }
    [PunRPC]
    public void DestroyOnMaster(int i_i)
	{
        List<Totem> templist = new List<Totem>(FindObjectsOfType<Totem>());
		for (int i = 0; i < templist.Count; i++)
		{
			if (templist[i].isBoss)
			{
                templist.Remove(templist[i]);
			}
			if (templist[i].id == i_i)
			{
                Destroy(templist[i].transform.gameObject);
			}
		}
	}
    [PunRPC]
    public void DisconectAll()
	{
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void SpawnItem(Vector3 spawnPos, int type)
	{
        GetComponent<PhotonView>().RPC("SpawnItemSynced", RpcTarget.MasterClient,  spawnPos, type);
    }
    [PunRPC]
    public void SpawnItemSynced(Vector3 spawnPos,int type)
	{
		switch (type)
		{
            case 0:
                int randomComon = Random.Range(0, itemlist.common.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.common[randomComon].name), spawnPos, Quaternion.identity);
                break;
            case 1:
                int randomRare = Random.Range(0, itemlist.rare.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.rare[randomRare].name), spawnPos, Quaternion.identity);
                break;
            case 2:
                int randomEpic = Random.Range(0, itemlist.epic.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.epic[randomEpic].name), spawnPos, Quaternion.identity);
                break;
            case 3:
                int randomLegendary = Random.Range(0, itemlist.legendary.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.legendary[randomLegendary].name), spawnPos, Quaternion.identity);
                break;
            case 4:
                int randomMythic = Random.Range(0, itemlist.mythic.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.mythic[randomMythic].name), spawnPos, Quaternion.identity);
                break;
        }
    }
    #region destroyWorldItems
    public void DropItems(string droppedItemName, Vector3 position, Quaternion rotation, int amount, int serialNumber)
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient, droppedItemName, position, rotation, amount, serialNumber);
    }
    [PunRPC]
    public void DestroyWorldItem(string droppedItemName, Vector3 position, Quaternion rotation, int amount, int serialNumber)
    {
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", droppedItemName), position, rotation);
        droppedItem.GetComponent<WorldItem>().SetUp(ItemList.SelectItem(droppedItemName).name, amount, ItemList.SelectItem(droppedItemName).sprite, ItemList.SelectItem(droppedItemName).type, ItemList.SelectItem(droppedItemName).maxStackSize);
        if (serialNumber > -1)
        {
            GetComponent<PhotonView>().RPC("RemoveItemFromWorld", RpcTarget.All, serialNumber);
        }
        GetComponent<PhotonView>().RPC("SyncStackAmount", RpcTarget.All, droppedItemName, amount, droppedItem.GetComponent<PhotonView>().ViewID);
    }
    [PunRPC]
    public void SyncStackAmount(string droppedItemName, int amount, int objectInstanceID)
    {
        WorldItem[] worldItems = FindObjectsOfType<WorldItem>();
        foreach (WorldItem worldItem in worldItems)
        {
            if(worldItem.GetComponent<PhotonView>().ViewID == objectInstanceID)
            {
                worldItem.gameObject.GetComponent<WorldItem>().SetUp(ItemList.SelectItem(droppedItemName).name, amount, ItemList.SelectItem(droppedItemName).sprite, ItemList.SelectItem(droppedItemName).type, ItemList.SelectItem(droppedItemName).maxStackSize);
            }
        }
    }
    public void SpawnEndBoss(Vector3 spawnPos)
    {
        GetComponent<PhotonView>().RPC("SpawnEndBossSyncted", RpcTarget.MasterClient, spawnPos);
    }
    [PunRPC]
    public void SpawnEndBossSyncted(Vector3 spawnPos)
    {
        GameObject spawnedEnemie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", endBossName), spawnPos+new Vector3(0,0.5f,0), Quaternion.identity);
        spawnedEnemie.GetComponent<Outline>().enabled = true;
        endboss = spawnedEnemie;
        endBossSpawned = true;
    }
    public void SpawnBoss(Vector3 spawnPos, int id)
	{
        GetComponent<PhotonView>().RPC("SpawnBossSyncted", RpcTarget.MasterClient, spawnPos, id);
    }
    [PunRPC]
    public void SpawnBossSyncted(Vector3 spawnPos, int id)
    {
        GameObject spawnedEnemie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", miniBossName ), spawnPos, Quaternion.identity);
        spawnedEnemie.GetComponent<Outline>().enabled = true;
        Totem[] totems = FindObjectsOfType<Totem>();
        for (int i = 0; i < totems.Length; i++)
        {
            if (totems[i].id == id)
            {
                totems[i].enemies.Add(spawnedEnemie);
            }
        }
    }
    public void SpawnEnemies(int i_i, Vector3 spawnPos, int id)
    {
        GetComponent<PhotonView>().RPC("SpawnEnemiesSyncted", RpcTarget.MasterClient, i_i, spawnPos,id);
    }
        [PunRPC]
    public void SpawnEnemiesSyncted(int i_i, Vector3 spawnPos, int id)
    {
        GameObject spawnedEnemie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", enemielist.enemieList[i_i]), spawnPos, Quaternion.identity);
        spawnedEnemie.GetComponent<Outline>().enabled = true;
        Totem[] totems = FindObjectsOfType<Totem>();
        for (int i = 0; i < totems.Length; i++)
        {
            if (totems[i].id == id)
            {
                totems[i].enemies.Add(spawnedEnemie);
            }
        }
    }
    public void ActivatTotem(int id,bool isboss)
	{
        GetComponent<PhotonView>().RPC("ActivatedTotemSync", RpcTarget.All, id,isboss);
    }
    [PunRPC]
    public void ActivatedTotemSync(int id,bool isboss)
    {
        Totem[] totems = FindObjectsOfType<Totem>();
        for (int i = 0; i < totems.Length; i++)
        {
            if (totems[i].id == id)
            {
				if (totems[i].isBoss == isboss)
				{
                    totems[i].activated = true;
				}
            }
        }
    }
    [PunRPC]
    public void OpenChestInWorld(int id)
    {
        ChestScript[] objectsFound = FindObjectsOfType<ChestScript>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].chestId == id)
            {
                objectsFound[i].anim.SetBool("OpenChest", true);
            }
        }
    }
    [PunRPC]
    public void RemoveItemFromWorld(int serialNumber)
    {
        HitableObject[] objectsFound = FindObjectsOfType<HitableObject>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].itemSerialNumber == serialNumber)
            {
                Destroy(objectsFound[i].gameObject);
            }
        }
        ItemPickUp[] objectsFoundPickUp = FindObjectsOfType<ItemPickUp>();
        for (int i = 0; i < objectsFoundPickUp.Length; i++)
        {
            if (objectsFoundPickUp[i].itemSerialNumber == serialNumber)
            {
                Destroy(objectsFoundPickUp[i].gameObject);
            }
        }
    }
    #endregion

    public bool IsThisMasterClient()
    {
        if(GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	#region everyoneDeath
    public void EveryOneDead()
	{
        GetComponent<PhotonView>().RPC("SyncPanelLoss", RpcTarget.All);
    }
    [PunRPC]
    public void SyncPanelLoss()
	{
        
	}
	#endregion

	#region sincHealth
	public void SincHealthOfHitableObject(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        GetComponent<PhotonView>().RPC("SincHealthOnMaster", RpcTarget.MasterClient, _serialNumber, _healthAmount, _type, _hitlocation);
    }
    [PunRPC]
    public void SincHealthOnMaster(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        GetComponent<PhotonView>().RPC("SetHealth", RpcTarget.All, _serialNumber, _healthAmount, _type, _hitlocation);
    }
    [PunRPC]
    public void SetHealth(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        HitableObject[] objectsFound = FindObjectsOfType<HitableObject>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].itemSerialNumber == _serialNumber)
            {
                objectsFound[i].HitByPlayer(_healthAmount, _type, _hitlocation);
                return;
            }
        }
    }
    #endregion
    #region sinc funcace slots
    public void SincSlots(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        GetComponent<PhotonView>().RPC("SincSlotsOmMaster", RpcTarget.MasterClient, slotNumber, givenItem, amount, originFurnace);
    }
    [PunRPC]
    public void SincSlotsOmMaster(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        GetComponent<PhotonView>().RPC("Rpc_sincSlotsFurnace", RpcTarget.Others, slotNumber, givenItem, amount, originFurnace);
    }
    [PunRPC]
    public void Rpc_sincSlotsFurnace(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].placeabelItemID == originFurnace)
            {
                objectsFound[i].GetComponent<OvenStation>().GetItemInSlot(slotNumber, givenItem, amount);
                return;
            }
        }
    }
    #endregion
    #region sinc chestincentory
    public void SincChestOnMaster(int slotId, string itemId, int itemAmount, int originChest)
    {
        GetComponent<PhotonView>().RPC("Rpc_SincChestOnMaster", RpcTarget.MasterClient, slotId, itemId, itemAmount, originChest);
    }
    [PunRPC]
    public void Rpc_SincChestOnMaster(int slotId, string itemId, int itemAmount, int originChest)
    {
        GetComponent<PhotonView>().RPC("Rpc_SincChestOnClients", RpcTarget.Others, slotId, itemId, itemAmount, originChest);
    }
    [PunRPC]
    public void Rpc_SincChestOnClients(int slotId, string itemId, int itemAmount, int originChest)
    {
        PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].placeabelItemID == originChest)
            {
                objectsFound[i].GetComponent<ChestInventory>().SincSlots(slotId, itemId, itemAmount);
                return;
            }
        }
    }
    #endregion

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void GetRandomQuote()
    {
        if (quoteText.isActiveAndEnabled)
        {
            int roll = Random.Range(0, quotes.Count);
            quoteText.text = quotes[roll];

            Invoke("GetRandomQuote", 5);
        }
    }
}
