using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
public class StateManager : MonoBehaviour
{
	public State curentState;
	public List<GameObject> players;
	public GameObject hitboxPos;
	public GameObject target;
	public AttackState[] attackStates;
	public AttackState[] rangedStates;
	public AttackState healingState;
	public Animator anim;
	public Vector3 delayedPos;
	public float targetUpdateTime;
	public float targetDelay;
	public float triggerRange;
	public float attackRange;
	public float idleRange=2f;
	public float attackMovementSpeed;
	public float movementSpeed;
	public float AttrotationSpeed;
	public int currentAttack;
	public bool doAttack;
	public bool spawned;
	public bool isDead;
	[Header("ranged")]
	public bool hasRangedAtt;
	public bool trowCoolDown;
	[Header("mage")]
	public bool hasMageAttack;
	public float healingCoolDownTime;
	public bool isHealing;
	public bool stopWalking;
	public GameObject laser;
	public GameObject healingEffect;
	public AudioSource laserAudio;
	[Header("Audio")]
	public float screamTime;
	public AudioSource walkingSound;
	public AudioSource screamSound;
	private bool gettingTarget;
	private bool hitboxActive;
	private bool doingDamage;
	private bool scream;
	private Health hp;
	private Collider[] hitColliders;
	private float hitboxRadius;
	private void Start()
	{
		players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		target = players[0];
		if (!PhotonNetwork.IsMasterClient)
		{
			GetComponent<NavMeshAgent>().enabled = false;
		}
		screamTime = Random.Range(4, screamTime);
		screamSound.Play();
	}
	private void Update()
	{
		if (isDead)
		{
			return;
		}
		if (transform.position.y >= 90)
		{
			PhotonNetwork.Destroy(gameObject);
		}
		if (!stopWalking)
		{
			if (!walkingSound.isPlaying)
			{
				if (GetComponent<NavMeshAgent>().destination != transform.position)
				{
					walkingSound.Play();
				}
			}
		}
		else
		{
			if (walkingSound.isPlaying)
			{
				walkingSound.Stop();
			}
		}
		if (hitboxActive)
		{
			hitColliders=Physics.OverlapSphere(hitboxPos.transform.position, hitboxRadius);
			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i].transform.tag == "Player")
				{
					if (!doingDamage)
					{
						StartCoroutine(DoDamge(i));
					}
				}
			}
		}
		RunStateMachine();
		if (!gettingTarget|| target.GetComponent<PlayerHealth>().isDead)
		{
			StartCoroutine(GetTarget());
		}
		if (!scream)
		{
			StartCoroutine(Scream());
		}
		new WaitForSeconds(targetDelay);
		delayedPos= target.transform.position;
	}
	public IEnumerator DoDamge(int i)
	{
		doingDamage = true;
		hitColliders[i].transform.GetComponent<PlayerHealth>().TakeDamage(attackStates[currentAttack].damage,false,0,0,0, hitColliders[i].transform.position);
		yield return new WaitForSeconds(5);
		doingDamage = false;
	}
	private void RunStateMachine()
	{
		State nextState = curentState?.RunCurrentState();

		if(nextState != null)
		{
			SwitchOnNextState(nextState);
		}
	}
	private void SwitchOnNextState(State nextState)
	{
		curentState = nextState;
	}
	public IEnumerator Scream()
	{
		scream = true;
		screamSound.Play();
		yield return new WaitForSeconds(screamTime);
		scream = false;
	}
	public IEnumerator GetTarget()
	{
		gettingTarget = true;
		for (int i = 0; i < players.Count; i++)
		{			
			float dis = Vector3.Distance(transform.position, players[i].transform.position);
			float targetDis = Vector3.Distance(transform.position, target.transform.position);
			if (dis < targetDis)
			{
				if (players[i] == null)
				{
					players.Remove(players[i]);
				}
				if (players[i] != null)
				{
					if (target != players[i])
					{
						if (players[i].activeSelf)
						{
							if (players[i].GetComponent<PlayerHealth>().health > 0 && players[i].activeSelf)
							{
								target = players[i];
							}
						}
					}
				}
			}
		}
		yield return new WaitForSeconds(targetUpdateTime);
		gettingTarget = false;
	}
	public void ResetAnim()
	{
		for (int i = 0; i < attackStates.Length; i++)
		{
			anim.SetBool(attackStates[i].animationName, false);
		}
		anim.SetBool("IsWalking", false);		
	}
	public void SetHitBoxActive(float radius)
	{
		hitboxRadius = radius;
		hitboxActive = true;
	}
	public void SetHitBoxFalse()
	{
		hitboxActive = false;
		hitColliders = null;
	}
	public void Spawned()
	{
		spawned = true;
	}
	public void CoolDownTrow(float timeToWait)
	{
		trowCoolDown = true;
		Invoke("TurnOn", timeToWait);
		print(timeToWait);
	}
	public void HealingCoolDown()
	{
		Invoke("TurnOffCoolDown", healingCoolDownTime);
		isHealing = true;
	}
	public void TurnOffCoolDown()
	{
		isHealing = false;
	}
	public void TurnOn()
	{
		trowCoolDown = false;
	}
	public void ToggelHealEffect(int i)
	{
		if (i ==1)
		{
			GetComponent<PhotonView>().RPC("ToggelHealEffectSynced", RpcTarget.All, true);
		}
		else
		{
			GetComponent<PhotonView>().RPC("ToggelHealEffectSynced", RpcTarget.All, false);
		}
	}
	[PunRPC]
	public void ToggelHealEffectSynced(bool b)
	{
		if (b)
		{
			healingEffect.SetActive(true);
			healingEffect.GetComponent<ParticleSystem>().Play();
		}
		else
		{
			healingEffect.SetActive(false);
			healingEffect.GetComponent<ParticleSystem>().Stop();
		}
	}
	public void HealMe()
	{
		GetComponent<EnemieHealth>().SincHeal(GetComponent<EnemieHealth>().health += 40);
	}
	public void KillMe()
	{
		PhotonNetwork.Destroy(gameObject);
	}
	public void TogleLaserMagic(int i)
	{
		if (i == 1)
		{
			GetComponent<PhotonView>().RPC("ToggleSynceLaserMagic", RpcTarget.All, true);
		}
		else
		{
			GetComponent<PhotonView>().RPC("ToggleSynceLaserMagic", RpcTarget.All, false);
		}
	}
	[PunRPC]
	public void ToggleSynceLaserMagic(bool b)
	{
		if (b)
		{
			laser.SetActive(true);
			laser.GetComponent<ParticleSystem>().Play();
			laser.transform.LookAt(target.transform.position);
			laserAudio.Play();
		}
		else
		{
			laserAudio.Stop();
			laser.SetActive(false);
			laser.GetComponent<ParticleSystem>().Stop();
		}
	}
}