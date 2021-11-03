using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShoot : MonoBehaviour
{
    public Rigidbody Bullet;
    public Transform bulletSpawnLocation;
    public float damage, attackSpeed, bulletSpeed;
    private float nextAttack, attackCooldown;

    private void Start()
    {
        attackCooldown = attackSpeed / (attackSpeed * attackSpeed);
    }
    void Update()
    {
        if (Time.time >= nextAttack)
        {
            nextAttack = attackCooldown + Time.time;
            ShootBullet();
        }
    }
    public void ShootBullet()
    {
        Rigidbody spawnedBulled = Instantiate(Bullet, bulletSpawnLocation.position, transform.rotation);
        spawnedBulled.velocity = spawnedBulled.transform.forward * bulletSpeed;
        spawnedBulled.GetComponent<Bullet>().damage = damage;
    }
}
