using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackingState : State
{
    public IdleState idle;
    public TriggerState trigger;
    [Space(5)]
    public StateManager sm;
    public NavMeshAgent agent;
    public int attackAmount;
	public float[] attackCoolDown;
	private bool isAttacking = false;
    public override State RunCurrentState()
    {
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        if (dist <= sm.triggerRange)
        {
            if (dist <= sm.attackRange)
            {
                Vector3 lookat = new Vector3(sm.target.transform.position.x, 0, sm.target.transform.position.z);//player pos without height

                transform.LookAt(lookat);

                if (!isAttacking)
                {
                    Attack();
                }
            }
            else
            {
                if (!isAttacking)
                {
                    sm.doAttack = false;
                    sm.ResetAnim();
                    return trigger;
                }
            }
        }
        else
        {
            sm.idleRange = 1000f;
            return idle;
        }
		if (sm.spawned)
		{
            if (PhotonNetwork.IsMasterClient)
            {
                agent.destination = transform.position;
            }
		}
        return this;
    }
    public void Attack()
	{
        sm.ResetAnim();
        isAttacking = true;
        sm.doAttack = true;
        int RandomInt = Random.Range(1, attackAmount+1);
		if (RandomInt == 1)
		{
            sm.anim.SetBool("Attack1", true);
        }
        else if (RandomInt == 2)
		{
            sm.anim.SetBool("Attack2", true);
        }
        else if (RandomInt == 3)
        {
            sm.anim.SetBool("Attack3", true);
        }
        else if (RandomInt == 4)
        {
            sm.anim.SetBool("Attack4", true);
        }
        else if (RandomInt == 5)
        {
            sm.anim.SetBool("Attack5", true);
        }
        StartCoroutine(DoDamage(attackCoolDown[RandomInt-1]));
    }
    public IEnumerator DoDamage(float waittime)
	{
       yield return new WaitForSeconds(waittime);
        isAttacking = false;
        sm.doAttack = false;
    }
}