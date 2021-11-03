using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveStoneScript : ProbScript
{
    public GameObject myPlayer;
	public override void Interaction()
	{
		myPlayer.GetComponent<PlayerHealth>().Respawn();
	}
}
