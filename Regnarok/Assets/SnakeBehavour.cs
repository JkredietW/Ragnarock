using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SnakeBehavour : MonoBehaviour
{
	public List<GameObject> targets;
	public float damage;
	public float attackTime;
	public float attackCooldown;
	public float targetCooldown;
	public float damageCoolDownTime;
	public ParticleSystem ps;
	public GameObject player;
	private bool isAttacking;
	private bool cooldownIsOn;
	private bool hasAttackCooldown;
	private bool doingDamage;
	public Animator anim;
	private NavMeshAgent agent;
	private float speed;
	private bool canAttack;
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Invoke("KillMe", 300f);
		speed = agent.speed;
		Invoke("Started", 5);
	}
	void Update()
	{
		if (!cooldownIsOn)
		{
			if (targets.Count != 0)
			{
					if (targets[0] != null)
					{
						if (!isAttacking && !hasAttackCooldown&&canAttack)
						{
							StartCoroutine(Attack());
							transform.LookAt(targets[0].transform.position);
						}
						if (PhotonNetwork.IsMasterClient)
						{
						float dis = Vector3.Distance(transform.position, player.transform.position);
							if (dis > 4)
							{
								agent.destination = targets[0].transform.position;
								agent.speed = speed;
							}
						}
						else
						{
							agent.destination = transform.position;
							agent.speed = 0;
						}
					}
					else
					{
						FiltherEnemies();
					}
			}
			else
			{
				if (ps.transform.gameObject.activeSelf)
				{
					StopAttacking();
				}
				float dis = Vector3.Distance(transform.position, player.transform.position);
				if (PhotonNetwork.IsMasterClient)
				{
					if (dis > 4)
					{
						agent.destination = player.transform.position;
						agent.speed = speed;
					}
					else
					{
						agent.destination = transform.position;
						agent.speed = 0;
					}
				}
			}
			if (isAttacking)
			{
				RaycastHit hitInfo;
				if (

					Physics.Raycast(transform.position, transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(0, 2, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(0, 4, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(0, 6, 0), transform.forward, out hitInfo) ||

					Physics.Raycast(transform.position + new Vector3(-2, 2, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(-4, 4, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(-6, 6, 0), transform.forward, out hitInfo) ||

					Physics.Raycast(transform.position + new Vector3(2, 2, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(4, 4, 0), transform.forward, out hitInfo) ||
					Physics.Raycast(transform.position + new Vector3(6, 6, 0), transform.forward, out hitInfo)

					)
				{
					if (hitInfo.transform.GetComponent<EnemieHealth>())
					{
						if (!doingDamage)
						{
							StartCoroutine(DamageCoolDown());
							hitInfo.transform.GetComponent<EnemieHealth>().TakeDamage(damage, false, 0, 0, 0, Vector3.zero);
						}
					}
				}
			}
		}
	}
	public void Started()
	{
		canAttack = true;
	}
	public IEnumerator DamageCoolDown()
	{
		doingDamage = true;
		yield return new WaitForSeconds(damageCoolDownTime);
		doingDamage = false;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.GetComponent<EnemieHealth>())
		{
			targets.Add(other.gameObject);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.GetComponent<EnemieHealth>())
		{
			targets.Remove(other.gameObject);
		}
	}
	public void SetFalse()
	{
		cooldownIsOn = false;
	}
	public IEnumerator Attack()
	{
		FiltherEnemies();
		isAttacking = true;
		ps.Play();
		ps.transform.gameObject.SetActive(true);
		anim.SetBool("IsAttacking", true);
		yield return new WaitForSeconds(attackTime);
		StopAttacking();
		StartCoroutine(AttackCooldown());
	}
	public void StopAttacking()
	{
		ps.Stop();
		ps.transform.gameObject.SetActive(false);
		anim.SetBool("IsAttacking", false);
		isAttacking = false;
	}
	public IEnumerator AttackCooldown()
	{
		FiltherEnemies();
		hasAttackCooldown = true;
		yield return new WaitForSeconds(attackCooldown);
		hasAttackCooldown = false;
		FiltherEnemies();
	}
	public void KillMe()
	{
		PhotonNetwork.Destroy(gameObject);
	}
	public void FiltherEnemies()
	{
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == null)
			{
				targets.Remove(targets[i]);
			}
		}
	}
}