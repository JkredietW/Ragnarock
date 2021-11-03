using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BasicAttack : AttackState
{
    public GameObject enemie;
    public Vector3 lookOffset;
    public bool StandingAttack;
    public AudioSource attackSound;
    public override State RunCurrentState()
    {
        if (sm.isDead)
		{
            return this; 
		}
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
		if (dist > 0.35)
		{
            FaceTarget(sm.target.transform.position);
		}
        if (dist <= sm.triggerRange)
        {
            if (dist <= sm.attackRange)
            {
                if (!sm.doAttack)
                {

                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    Vector3 toOther = sm.target.transform.position - transform.position;
                    if (Vector3.Dot(forward, toOther) >0)
                    {
                        int randomI = Random.Range(0, sm.attackStates.Length);
                        DoAttack();
                        return this;
                    }
                }
				else
				{
                    sm.doAttack = false;
                    sm.stopWalking = false;
                    return trigger;
				}
            }
            else
            {
                if (!sm.doAttack)
                {
                    if (sm.spawned)
                    {
                        if (!sm.anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                        {
                            agent.speed = sm.movementSpeed;
                            sm.idleRange = 1000f;
                            sm.stopWalking = false;
                            return trigger;                   
                        }
                    }
                }
            }
        }
        else
        {
            if (!sm.anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                agent.speed = sm.movementSpeed;
                sm.idleRange = 1000f;
                sm.stopWalking = false;
                return idle;
            }
        }
        return this;
    }
    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - enemie.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos+lookOffset);
        enemie.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, sm.AttrotationSpeed);
    }
    public void DoAttack()
	{
        if (PhotonNetwork.IsMasterClient)
        {
            if (StandingAttack)
            {
                agent.destination = enemie.transform.position;
                sm.stopWalking = true;
            }
            else
            {
                agent.destination = sm.target.transform.position;
                sm.stopWalking = false;
            }
        }
        sm.ResetAnim();
        sm.anim.SetBool(animationName, true);
		if (!attackSound.isPlaying)
		{
            attackSound.Play();
		}
        agent.speed = sm.attackMovementSpeed;
    }
}