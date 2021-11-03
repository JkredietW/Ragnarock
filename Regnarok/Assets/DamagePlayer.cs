using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (!other.GetComponent<PlayerHealth>().takingWaterDamage)
			{
				other.GetComponent<PlayerHealth>().DamagePoitionWater();
			}
		}
	}
}
