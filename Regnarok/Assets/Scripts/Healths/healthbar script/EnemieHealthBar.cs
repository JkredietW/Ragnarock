using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieHealthBar : HealthBar
{
	public EnemieHealth myHealth;
	private void Start()
	{
		GetMyPlayer();
		healtSlider.maxValue = myHealth.maxHealth;
		healtSlider.minValue = 0;

		healtSlider.value = myHealth.health;
	}
	private void Update()
	{
		healtSlider.value = myHealth.health;
		healtbarObject.transform.LookAt(myPlayer.transform);
	}
}
