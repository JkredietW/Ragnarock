using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.IO;

public class EnemieHealth : Health
{
	public int coinDrop;
	public float xpAmount;
	public Renderer[] toDisolve;
	public GameObject healthbar;
	public MeshCollider col;
	private GameManager gm;
	private StateManager sm;
	private PhotonView pv;

	public Vector3 coinDropOffset;
	public Vector3 itemDropOffset;
	[Header("PoitionWater")]
	public float timeBetweenDamages;
	public float waterDamage;
	public bool takingWaterDamage;
	public AudioSource burningAudio;
	private float value;
	private bool disolveCharachter;
	public void Start()
	{
		gm = FindObjectOfType<GameManager>();
		sm = GetComponent<StateManager>();
		pv = GetComponent<PhotonView>();
	}
	public override void Health_Dead()
	{
		transform.position -= new Vector3(0, 0.5f, 0);
		sm.isDead = true;
		col.enabled = true;
		GetComponent<NavMeshAgent>().enabled = false;
		GetComponent<Rigidbody>().useGravity = true;
		sm.ResetAnim();
		sm.anim.SetBool("IsDying", true);
		Destroy(gameObject.GetComponent<NavMeshAgent>());
		Destroy(gameObject.GetComponent<NavMeshObstacle>());
		healthbar.SetActive(false);
		
	}
	private void Update()
	{
		if (disolveCharachter)
		{
			value += 0.2f * Time.deltaTime;
			DisolveMe();
			if (value > 1)
			{
				value = 1;
				PhotonNetwork.Destroy(gameObject);
				disolveCharachter = false;
			}
		}
	}
	public void RollItem()
	{
        foreach (DropItems item in dropItemList)
        {
			float roll = Random.Range(0, 101);
			if(roll < item.ChanceOfDrop)
            {
				GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", item.nameOfItem), transform.position + itemDropOffset, Quaternion.identity);
				if(item.amountOfItem > 1)
                {
					droppedItem.GetComponent<WorldItem>().itemAmount = item.amountOfItem;
				}
			}
        }
	}
	public void DamagePoitionWater()
	{
		if (!takingWaterDamage)
		{
			StartCoroutine(TakeWaterDamage());
		}
	}
	public IEnumerator TakeWaterDamage()
	{
		takingWaterDamage = true;
		yield return new WaitForSeconds(timeBetweenDamages);
		takingWaterDamage = false;
		GetComponent<PlayerHealth>().TakeDamage(waterDamage, false, 0, 0, 0, transform.position + new Vector3(Random.Range(-1, 2), 0, (Random.Range(-1, 2))));
	}
	[PunRPC]
	public void DropMoney()
	{
		coinDropOffset = new Vector3(0, 2, 0);
		float temcoins = coinDrop * (gm.days + 1) * gm.goldMultiplier;
		coinDrop = (int)temcoins;
		FindObjectOfType<GameManager>().DropItems("GoldenCoin", lastHitLocation + coinDropOffset, Quaternion.identity, coinDrop, -1);
	}
	[PunRPC]
	public void GiveXp()
    {
		CharacterStats[] players = FindObjectsOfType<CharacterStats>();
        foreach (CharacterStats player in players)
        {
			if(player.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
            {
				player.GainXp(xpAmount);
            }
		}
    }
	public void GivePlayerStuff()
	{
		if(PhotonNetwork.IsMasterClient)
        {
			RollItem();
			pv.RPC("DropMoney", RpcTarget.MasterClient);
			pv.RPC("GiveXp", RpcTarget.All);
			PhotonNetwork.Destroy(gameObject);
		}
	}
	public void ActivateDisolve()
	{
		disolveCharachter = true;
	}
	public void DisolveMe()
	{
		if (!burningAudio.isPlaying)
		{
			burningAudio.Play();
		}
		for (int i = 0; i < toDisolve.Length; i++)
		{
			toDisolve[i].material.SetFloat("_Dissolve", value);
		}
	}
}