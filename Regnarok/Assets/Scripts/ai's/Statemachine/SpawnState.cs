using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnState : State
{
    public StateManager sm;
    public State idleState;
    [Space(5)]
    public Rigidbody rb;
    public Animator anim;
    public NavMeshAgent agent;
    public GameObject mesh;
    public float jumpSpeed;
    public bool hasWeapon;
    public GameObject weapon;
    public GameObject secondWeapon;
    public bool hasHat;
    public GameObject hat;
    private bool isSpawning;
	public override State RunCurrentState()
	{
		if (isSpawning)
		{
            return this;
		}
		else
		{
            return idleState;
        }
	}
    public void SpawnForce()
    {
        rb.AddForce(Vector3.up * jumpSpeed * 3);
        isSpawning = true;
        mesh.SetActive(false);
		if (hasWeapon)
		{
            weapon.SetActive(false);
        }
		if (hasHat)
		{
            hat.SetActive(false);
		}
    }
    public void StopSpawnForce()
    {
        anim.applyRootMotion = false;
		agent.enabled = true;
        mesh.SetActive(true);
        if (hasWeapon)
        {
            weapon.SetActive(true);
        }
        if (hasHat)
        {
            hat.SetActive(true);
        }
        Invoke("TurnOffGravity", 0.5f);
    }
    public void TurnOffGravity()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        isSpawning = false;
    }
    public void SetWeapon1Active()
	{
        weapon.SetActive(true);
	}
    public void SetWeapon2Active()
    {
        secondWeapon.SetActive(true);
    }
}