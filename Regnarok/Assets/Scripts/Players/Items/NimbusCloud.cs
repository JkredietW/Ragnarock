using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NimbusCloud : MonoBehaviour
{
    public Vector3 hitBoxScale, hitboxCenter;
    public float damage;
    public float attackSpeed = 0.5f;

    public IEnumerator StartNimbus(float timeAlive, float _damage)
    {
        damage = _damage;
        StartCoroutine(DamageToEnemies());
        yield return new WaitForSeconds(timeAlive);
        PhotonNetwork.Destroy(gameObject);
    }
    public IEnumerator DamageToEnemies()
    {
        ActualDamage();
        yield return new WaitForSeconds(attackSpeed);
        StartCoroutine(DamageToEnemies());
    }
    void ActualDamage()
    {
        Collider[] hitObjects = Physics.OverlapBox(hitboxCenter + transform.position, hitBoxScale * transform.localScale.x);
        foreach (Collider enemy in hitObjects)
        {
            if(enemy.GetComponent<EnemieHealth>())
            {
                enemy.GetComponent<EnemieHealth>().TakeDamage(damage,false,0,0,0,Vector3.zero);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hitboxCenter + transform.position, hitBoxScale * transform.localScale.x);
    }
}
