using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpecialBosScript : MonoBehaviour
{
	public bool has2Stages;
	public GameObject objectTorated;
	public AttackState[] newStates;
	public StateManager st;
	private bool inSecondStage;
	private float speed;
	public void Start()
	{
		speed = GetComponent<NavMeshAgent>().speed;
	}
	public void RotateObj(float amount)
	{
		objectTorated.transform.rotation = Quaternion.Euler(objectTorated.transform.eulerAngles.x, amount, objectTorated.transform.eulerAngles.z);
	}
	private void Update()
	{
		if (has2Stages)
		{
			if (!inSecondStage)
			{
				if (GetComponent<EnemieHealth>().health < GetComponent<EnemieHealth>().maxHealth / 5)
				{
					StartCoroutine(EnterSecondstage());
				}
			}
		}
	}
	public IEnumerator EnterSecondstage()
	{
		inSecondStage = true;
		yield return new WaitForSeconds(0.5f);
		GetComponent<Animator>().SetBool("Stage2", true);
		st.attackStates = newStates;
	}
	public void StopDes()
	{
		st.spawned = false;
		GetComponent<NavMeshAgent>().destination = transform.position;
		GetComponent<NavMeshAgent>().speed = 0; ;
	}
	public void DontStop()
	{
		st.spawned = true;
		GetComponent<NavMeshAgent>().speed = speed;
	}
}
