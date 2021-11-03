using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TrowState : AttackState
{
    public GameObject curveObject;
    public GameObject trowObject;
    public GameObject handObject;
    public TrowObject objectScript;
    public Rigidbody objectRb;
    public GameObject enemie;
    public string trowObjectName;
    public float trowCooldownTime;
    private GameObject newTrowable;
    private bool isTrowing;
    
    public override State RunCurrentState()
    {
        if (sm.isDead)
        {
            return this;
        }
		if (sm.trowCoolDown&& !isTrowing)
		{
            sm.anim.SetBool("Attack3", false);
            return trigger;
		}
        float dist = Vector3.Distance(transform.position, sm.target.transform.position);
        if (dist > 0.35)
        {
            FaceTarget(sm.target.transform.position);
        }
        if (dist <= sm.triggerRange/1.5f)
        {
            if (!sm.doAttack)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = sm.target.transform.position - transform.position;
                int randomI = Random.Range(0, sm.attackStates.Length);
                if (!sm.trowCoolDown)
                {
					if (sm.curentState == this)
					{
                        DoAttack();
					}
                }
                return this;
            }
            else
            {
                sm.doAttack = false;
                return trigger;
            }
		}
		else
		{
            return trigger;
        }
    }
    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - enemie.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        enemie.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, sm.AttrotationSpeed);
    }
    public void DoAttack()
    {
        isTrowing = true;
        sm.ResetAnim();
        sm.anim.SetBool(animationName, true);
        sm.CoolDownTrow(trowCooldownTime);
    }
    [PunRPC]
    public void SpawnNewHamer()
    {
        newTrowable = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", trowObjectName), trowObject.transform.position, trowObject.transform.rotation);
        newTrowable.transform.parent = trowObject.transform.parent;
    }
    public void Trow()
	{
    }
}
