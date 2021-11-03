using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;

public class PlayerHealth : Health
{
    public bool isDead;
    public GameObject graveStone;
    public GameObject mainCam;
    public GameObject mesh;
    public List<GameObject> otherPlayersCam;
    public Slider HealthSlider;
    public float respawnTime=15;
    [Header("PoitionWater")]
    public float timeBetweenDamages;
    public float waterDamage;
    public bool takingWaterDamage;
    private int index;
    private GameManager gm;
    private void Start()
    {
        if (PV.IsMine)
        {
            gm = FindObjectOfType<GameManager>();   
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = health;
            StartCoroutine("HealthRegen");
            otherPlayersCam = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainCamera"));
            for (int i = 0; i < otherPlayersCam.Count; i++)
            {
                if (otherPlayersCam[i] == mainCam)
                {
                    otherPlayersCam.Remove(otherPlayersCam[i]);
                }
                else if (otherPlayersCam[i].transform.parent.transform.GetComponent<PlayerHealth>().health <= 0)
				{
                    otherPlayersCam.Remove(otherPlayersCam[i]);
                }
            }
            for (int i = 0; i < otherPlayersCam.Count; i++)
            {
				if (otherPlayersCam[i].transform.GetComponent<AudioListener>())
				{
                    Destroy(otherPlayersCam[i].transform.GetComponent<AudioListener>());
                }
            }
        }
    }
	private void Update()
	{
        if (PV.IsMine)
        {
            if (isDead)
            {
				if (gm == null)
				{
                    gm = FindObjectOfType<GameManager>();
                }
				else
				{
                    gm.CheckHp();
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
        GetComponent<PlayerHealth>().TakeDamage(waterDamage, false, 0, 0, 0,transform.position + new Vector3 (Random.Range(-1, 2), 0 , (Random.Range(-1, 2))));
    }
	public override void Health_Damage(float damageValue, bool bleed, int burn, float poison, float execute, Vector3 hitlocation)
    {
        base.Health_Damage(damageValue, bleed, burn, poison, execute, hitlocation);
        if(HealthSlider)
        {
            HealthSlider.value = health;
        }
    }
    public override void Health_Heal(float healValue)
    {
        base.Health_Heal(healValue);
        if(PV.IsMine)
        {
            HealthSlider.value = health;
        }
    }
    public override void RecieveStats(float _health, float _armor, float _healthRegen, int revives)
    {
        base.RecieveStats(_health, _armor, _healthRegen, revives);
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
        }
    }
	public override void Health_Dead()
	{
        if (!PV.IsMine)
        {
            return;
        }
        if (reviveAmount > 0)
        {
            Health_Heal(maxHealth);
            GetComponent<StackableItemScript>().RemoveItem("Revive");
        }
        else
        {
            StartCoroutine(Dead());
        }
    }
	IEnumerator HealthRegen()
    {
        Health_Heal(healthRegen);
        yield return new WaitForSeconds(1);
        StartCoroutine("HealthRegen");
	}
    public IEnumerator Dead()
	{
		GetComponent<PhotonView>().RPC("SetDeadBool", RpcTarget.All, true);
        gm = FindObjectOfType<GameManager>();
        gm.CheckHp();
        transform.position += new Vector3(0,100,0);
        //GetComponent<PhotonView>().RPC("SpawnGrave", RpcTarget.MasterClient);
        otherPlayersCam = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainCamera"));
        //GetComponent<PhotonView>().RPC("GetGrave", RpcTarget.All);
        for (int i = 0; i < otherPlayersCam.Count; i++)
		{
			if (otherPlayersCam[i] == mainCam)
			{
				otherPlayersCam.Remove(otherPlayersCam[i]);
			}
			else if (otherPlayersCam[i].transform.parent.transform.GetComponent<PlayerHealth>().health <= 0)
			{
				otherPlayersCam.Remove(otherPlayersCam[i]);
			}
		}
		for (int i = 0; i < otherPlayersCam.Count; i++)
		{
			if (otherPlayersCam[i].transform.GetComponent<AudioListener>())
			{
				Destroy(otherPlayersCam[i].transform.GetComponent<AudioListener>());
			}
		}
		mainCam.SetActive(false);
		GetComponent<PhotonView>().RPC("SetBody", RpcTarget.All, false);
        GetComponent<PhotonView>().RPC("SetDeadBool", RpcTarget.All, true);
        otherPlayersCam[index].GetComponent<Camera>().enabled = true;
        FindObjectOfType<GameManager>().CheckHp();
       //GetComponent<PhotonView>().RPC("GetGrave", RpcTarget.All);
        yield return new WaitForSeconds(respawnTime*FindObjectOfType<GameManager>().days/2+5);
        transform.position = gm.deathPos;
        gm.PlayRespawnSound();
        Respawn();
    }
    [PunRPC]
    public void GetGrave()
	{
        List<GraveStoneScript> templist = new List<GraveStoneScript>(FindObjectsOfType<GraveStoneScript>());
		for (int i = 0; i < templist.Count; i++)
		{
            if (templist[i].myPlayer == null)
			{
                templist[i].myPlayer = transform.gameObject;
            }
		}
    }
    [PunRPC]
    public void SetDeadBool(bool b)
	{
        isDead = b;
	}
    [PunRPC]
    public void SpawnGrave()
	{
        graveStone = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Grave"), transform.position, transform.rotation);
    }
	public void Respawn()
	{
        GetComponent<PhotonView>().RPC("SetDeadBool", RpcTarget.All, false);
        SincHeal(100);
        otherPlayersCam[index].GetComponent<Camera>().enabled = false;
        otherPlayersCam.Clear();
        mainCam.SetActive(true);
        GetComponent<PlayerController>().isDead = false;
        GetComponent<PhotonView>().RPC("SetBody", RpcTarget.All,true);
       // GetComponent<PhotonView>().RPC("DestroyGraveStone", RpcTarget.MasterClient);
        SincHeal(100);
    }
    [PunRPC]
    public void SetBody(bool b)
	{
		if (b)
		{
            mesh.SetActive(true);
        }
		else
		{
            mesh.SetActive(false);
        }
	}
    [PunRPC]
    public void DestroyGraveStone()
	{
        PhotonNetwork.Destroy(graveStone);
    }
}
