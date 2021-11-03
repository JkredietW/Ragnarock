using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
	public string animationName;
	public StateManager sm;
	public NavMeshAgent agent;
	public TriggerState trigger;
	public IdleState idle;
	public float damage;
	public override State RunCurrentState()
	{
		return this;
	}
}
