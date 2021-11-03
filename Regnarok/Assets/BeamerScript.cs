using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BeamerScript : MonoBehaviour
{
    public float cooldownTime;
    public float damage;
    private bool cooldown;
    void Update()
    {
		if (PhotonNetwork.IsMasterClient)
		{
			if (!cooldown)
			{
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
			        if (hitInfo.transform.CompareTag("Player"))
			        {
                        hitInfo.transform.GetComponent<PlayerHealth>().TakeDamage(damage, false, 0,0,0, Vector3.zero);
                        StartCoroutine(StartCoolDown());
                    }
		        }
            }
        }
    }
    public IEnumerator StartCoolDown()
	{
        cooldown = true;
		yield return new WaitForSeconds(cooldownTime);
        cooldown = false;

    }
}
