using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [HideInInspector] public float health;
    public GameObject damageNumber;
    protected Vector3 lastHitLocation;

    public PhotonView PV;

    public float armor;

    protected float healthRegen;

    int bleedTicks, burnTicks, poisonTicks;
    float bleedDamage, burnDamage, poisonDamage;

    [HideInInspector]public float reviveAmount;
    public List<DropItems> dropItemList;
    public AudioSource audioSource;
    public GameObject bloodSpat;

    //particles
    public GameObject fireEffect;
    public GameObject poisonEffect;

    [System.Serializable]
    public struct DropItems
    {
        public string nameOfItem;
        public int amountOfItem;
        public float ChanceOfDrop;
    }

    protected virtual void Awake()
    {
        if(GetComponent<PhotonView>())
        {
            PV = GetComponent<PhotonView>();
        }
        health = maxHealth;
        burnDamage = maxHealth / 40;
        audioSource = GetComponent<AudioSource>();
    }
    public virtual void Health_Damage(float damageValue, bool bleed, int burn, float poison, float execute, Vector3 hitlocation)
    {
        audioSource.Play();
        GameObject blood = Instantiate(bloodSpat, hitlocation, Quaternion.identity);
        Destroy(blood, 1);
        lastHitLocation = hitlocation;
        if (execute > 0)
        {
            //if crit not execute
            //color of damage number different
        }
        else
        {
            //here normal color
        }

        damageValue = Mathf.Clamp(damageValue - armor, 1, maxHealth);
        if (health > 0)
        {
            health = Mathf.Clamp(health - damageValue, 0, maxHealth);
            if (damageNumber != null)
            {
                GameObject damageNum = Instantiate(damageNumber, hitlocation, Quaternion.identity);
                damageNum.GetComponent<DamageNumbers>().damageAmount = damageValue;
            }
            if (health == 0)
            {
                Health_Dead();
            }
            if (execute > 0 && health < maxHealth * execute)
            {
                Health_Dead();
            }
        }
        if (bleed)
        {
            if (bleedDamage == 0)
            {
                StartCoroutine(Bleed());
            }
            bleedTicks += 5;
            if (bleedDamage < damageValue * 0.25f)
            {
                bleedDamage = damageValue * 0.25f;
            }
        }
        if (burn > 0)
        {
            burnTicks += burn * 2;
            if (burnTicks > 0)
            {
                fireEffect.SetActive(true);
                StartCoroutine(Bleed());
            }
        }
        if (poison > 0)
        {
            poisonEffect.SetActive(true);
            poisonTicks = 10;
            poisonDamage += poison;
            StartCoroutine(Poison());
        }
    }
    public virtual void Health_Heal(float healValue)
    {
        if (PV.IsMine)
        {
            if (health < maxHealth)
            {
                health = Mathf.Clamp(health + healValue, 0, maxHealth);
            }
        }
    }
    public virtual void Health_Dead()
    {
        print("used base function");
        PhotonNetwork.Destroy(gameObject);
    }

    //stats
    public virtual void GiveKiller(GameObject killer)
    {
        //nothing here yet
    }
    public virtual void RecieveStats(float _health, float _armor, float _healthRegen, int revives)
    {
        maxHealth = _health;
        armor = _armor;
        healthRegen = _healthRegen;
        reviveAmount = revives;
    }

    protected virtual IEnumerator Bleed()
    {
        float roll = Random.Range(0.4f, 0.7f);
        yield return new WaitForSeconds(roll);
        if(bleedTicks > 0)
        {
            bleedTicks--;
            Health_Damage(bleedDamage, false, 0, 0, 0, transform.position);
            StartCoroutine(Bleed());
        }
        else
        {
            bleedDamage = 0;
        }
    }
    protected virtual IEnumerator Burn()
    {
        float roll = Random.Range(0.9f, 1.2f);
        yield return new WaitForSeconds(roll);
        if (burnTicks > 0)
        {
            burnTicks--;
            Health_Damage(burnDamage, false, 0, 0, 0, transform.position);
            StartCoroutine(Burn());
        }
        else
        {
            fireEffect.SetActive(false);
        }
    }
    protected virtual IEnumerator Poison()
    {
        float roll = Random.Range(0.4f, 0.7f);
        yield return new WaitForSeconds(roll);
        if (poisonTicks > 0)
        {
            poisonTicks--;
            Health_Damage(poisonDamage, false, 0, 0, 0, transform.position);
            StartCoroutine(Poison());
        }
        else
        {
            poisonDamage = 0;
            poisonEffect.SetActive(false);
        }
    }

    #region sinc
    //damage
    public void TakeDamage(float damageValue, bool _bleed, int burnticks, float poisonTicks, float execute, Vector3 _hitlocation)
    {
        PV.RPC("SincHealthOnMAster", RpcTarget.MasterClient, damageValue, _bleed, burnticks, poisonTicks, execute, _hitlocation);
    }
    //heal
    public void TakeHeal(float damageValue)
    {
        PV.RPC("SincHealOnMAster", RpcTarget.MasterClient, damageValue);
    }

    //damage
    [PunRPC]
    public void SincHealthOnMAster(float _health, bool _bleed, int burnticks, float poisonTicks, float execute, Vector3 _hitlocation)
    {
         PV.RPC("SincHealth", RpcTarget.All, _health, _bleed, burnticks, poisonTicks, execute, _hitlocation);
    }
    [PunRPC]
    public void SincHealth(float _health, bool _bleed, int burnticks, float poisonTicks, float execute, Vector3 _hitlocation)
    {
        Health_Damage(_health, _bleed, burnticks, poisonTicks, execute, _hitlocation);
    }
    //heal
    [PunRPC]
    public void SincHealOnMAster(float _health)
    {
        PV.RPC("SincHeal", RpcTarget.All, _health);
    }
    [PunRPC]
    public void SincHeal(float _health)
    {
        Health_Heal(_health);
    }
    #endregion
}
